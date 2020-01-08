using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Core.Caching
{
    /// <summary>
    /// Distributed memory cache that synchronizes results at the end of a request.
    /// </summary>
    public class DistributedMemoryCacheManager : ICacheMessageQueueProvider, IDistributedCacheManager
    {
        private Timer _expirationScanTimer;

        /// <summary>
        /// A concurrent dictionary to store the cache entries.
        /// </summary>
        private readonly ConcurrentDictionary<string, CacheEntry> _concurrentDictionary = new ConcurrentDictionary<string, CacheEntry>();
        private readonly Action<string> _logWarningMethod;

        /// <summary>
        /// Gets a queue with cache messages to process.
        /// </summary>
        public ConcurrentQueue<CacheMessage> CacheMessageQueue { get; } = new ConcurrentQueue<CacheMessage>();

        /// <summary>
        /// Constructor initializes a new instance of the <see cref="DistributedMemoryCacheManager"/> class.
        /// </summary>
        public DistributedMemoryCacheManager(NopConfig nopConfig, Action<string> logWarningMethod)
        {
            // anything less than 60 seconds is useless.
            var expirationInterval = Math.Min(60, nopConfig.DistributedExpirationScanInterval);

            _expirationScanTimer = new Timer((_) => ScanForExpiredItems(), null, TimeSpan.FromSeconds(new Random().Next(0, expirationInterval)), TimeSpan.FromSeconds(expirationInterval));
            _logWarningMethod = logWarningMethod ?? throw new ArgumentNullException(nameof(logWarningMethod));
        }

        /// <summary>
        /// Scans for expired items.
        /// </summary>
        /// <param name="state"></param>
        private void ScanForExpiredItems()
        {
            foreach (var entry in _concurrentDictionary.Where(x => x.Value.Expired))
            {
                this.Remove(entry.Key, null);
            }
        }

        /// <summary>
        /// Clears the memory cache.
        /// </summary>
        public void Clear()
        {
            Clear(true);
        }

        /// <summary>
        /// Disposes of unmanaged or shared resources.
        /// </summary>
        public void Dispose()
        {
            if (_expirationScanTimer != null)
            {
                _expirationScanTimer.Dispose();
                _expirationScanTimer = null;
            }
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time</param>
        /// <returns>The cached value associated with the specified key</returns>
        public T Get<T>(CacheKey key, Func<T> acquire)
        {

            // Do not cache.
            if (key.CacheTime <= 0)
            {
                if (_concurrentDictionary.TryGetValue(key.Key, out var result))
                {
                    if (!result.Expired)
                    {

                        if (result.Value != null && typeof(T) != result.Value.GetType() && result.Value is IConvertible)
                        {
                            return (T)Convert.ChangeType(result.Value, typeof(T));
                        }
                        else
                        {
                            return (T)result.Value;
                        }
                    }
                }
                return acquire();
            }

            // Do a thread safe add or update for the cache entry.
            CacheEntry entry = _concurrentDictionary.AddOrUpdate(key.Key,
                // add delegate
                (k) =>
                {
                    var result = acquire();

                    var innerEntry = new CacheEntry()
                    {
                        CacheTime = key.CacheTime,
                        UtcDateTime = DateTime.UtcNow,
                        Value = result,
                    };

                    if (IsSerializable(key.Key, result))
                    {
                        CacheMessageQueue.Enqueue(new CacheMessage()
                        {
                            Operation = nameof(Set),
                            Key = key.Key,
                            Entry = innerEntry
                        });
                    }

                    return innerEntry;
                },
                // update delegate
                (k, currentValue) =>
                {
                    if (!currentValue.Expired)
                    {
                        return currentValue;
                    }

                    var result = acquire();

                    var innerEnty = new CacheEntry()
                    {
                        CacheTime = key.CacheTime,
                        UtcDateTime = DateTime.UtcNow,
                        Value = result,
                    };

                    if (IsSerializable(key.Key, result))
                    {
                        CacheMessageQueue.Enqueue(new CacheMessage()
                        {
                            Operation = nameof(Set),
                            Key = key.Key,
                            Entry = innerEnty
                        });
                    }

                    return innerEnty;
                });

            if (entry.Value != null && typeof(T) != entry.Value.GetType() && entry.Value is IConvertible)
            {
                return (T)Convert.ChangeType(entry.Value, typeof(T));
            }
            else
            {
                return (T)entry.Value;
            }
        }

        /// <summary>
        /// Determines whether an entry is serializable.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <param name="value">The value to test.</param>
        /// <returns>True when it is serializable. False otherwise.</returns>
        private bool IsSerializable(string key, object value)
        {
            if (value != null && value.GetType().IsGenericType)
            {
                var genericType = value.GetType().GetGenericTypeDefinition();
                if (genericType.FullName.StartsWith("System.Linq.Enumerable"))
                {
                    // this is a Nomad (like Enumerable.Where or Enumberable.Select). It's impossible to cache. Convert it to a list.
                    _logWarningMethod($"The cache entry with key {key} is of type {value.GetType()}, it's a Function call and cannot be converted to JSON. Cache it as a list.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time</param>
        /// <returns>The cached value associated with the specified key</returns>
        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {

            // Do not cache.
            if (key.CacheTime <= 0)
            {
                if (_concurrentDictionary.TryGetValue(key.Key, out var result))
                {
                    if (!result.Expired)
                    {

                        if (result.Value != null && typeof(T) != result.Value.GetType() && result.Value is IConvertible)
                        {
                            return (T)Convert.ChangeType(result.Value, typeof(T));
                        }
                        else
                        {
                            return (T)result.Value;
                        }
                    }
                }
                return await acquire();
            }

            // Do a thread safe add or update for the cache entry.
            CacheEntry entry = _concurrentDictionary.AddOrUpdate(key.Key,
                // add delegate
                (k) =>
                {
                    var result = Task.Run(async () => await acquire()).GetAwaiter().GetResult();

                    var innerEntry = new CacheEntry()
                    {
                        CacheTime = key.CacheTime,
                        UtcDateTime = DateTime.UtcNow,
                        Value = result,
                    };

                    if (IsSerializable(key.Key, result))
                    {
                        CacheMessageQueue.Enqueue(new CacheMessage()
                        {
                            Operation = nameof(Set),
                            Key = key.Key,
                            Entry = innerEntry
                        });
                    }

                    return innerEntry;
                },
                // update delegate
                (k, currentValue) =>
                {
                    if (!currentValue.Expired)
                    {
                        return currentValue;
                    }

                    var result = Task.Run(async () => await acquire()).GetAwaiter().GetResult();

                    var innerEnty = new CacheEntry()
                    {
                        CacheTime = key.CacheTime,
                        UtcDateTime = DateTime.UtcNow,
                        Value = result,
                    };

                    if (IsSerializable(key.Key, result))
                    {
                        CacheMessageQueue.Enqueue(new CacheMessage()
                        {
                            Operation = nameof(Set),
                            Key = key.Key,
                            Entry = innerEnty
                        });
                    }

                    return innerEnty;
                });

            if (entry.Value != null && typeof(T) != entry.Value.GetType() && entry.Value is IConvertible)
            {
                return (T)Convert.ChangeType(entry.Value, typeof(T));
            }
            else
            {
                return (T)entry.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public bool IsSet(CacheKey key)
        {
            if (_concurrentDictionary.TryGetValue(key.Key, out var result))
            {
                if (!result.Expired)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public void Remove(CacheKey key)
        {
            Remove(key.Key, null);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="utcDateTime">The utc date time for the cache entry.</param>
        public void Remove(string key, DateTime? utcDateTime)
        {
            if (_concurrentDictionary.TryGetValue(key, out var entry))
            {
                if (!entry.Expired && entry.UtcDateTime > utcDateTime)
                {
                    // newer item in cache. 
                    return;
                }
            }

            if (_concurrentDictionary.TryRemove(key, out entry))
            {
                if (!entry.Expired && !utcDateTime.HasValue)
                {
                    CacheMessageQueue.Enqueue(new CacheMessage()
                    {
                        Operation = nameof(Remove),
                        Key = key,
                        Entry = entry
                    });
                }
            }
        }

        /// <summary>
        /// Removes items by key prefix.
        /// </summary>
        /// <param name="prefix">String key prefix.</param>
        public void RemoveByPrefix(string prefix)
        {
            RemoveByPrefix(prefix, null);
        }

        /// <summary>
        /// Removes items by key prefix.
        /// </summary>
        /// <param name="prefix">String key prefix.</param>
        /// <param name="utcDateTime">The utc date time for the cache entry.</param>
        public void RemoveByPrefix(string prefix, DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue)
            {
                CacheMessageQueue.Enqueue(new CacheMessage()
                {
                    Operation = nameof(RemoveByPrefix),
                    Key = prefix,
                    // record the time for the prefix removal operation.
                    Entry = new CacheEntry()
                    {
                        CacheTime = 0,
                        UtcDateTime = DateTime.UtcNow,
                        Value = null
                    }
                });
            }

            var keys = _concurrentDictionary.Keys;

            //get cache keys that matches pattern
            var regex = new Regex(prefix,
                RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchesKeys = keys.Where(key => regex.IsMatch(key)).ToList();

            //remove matching values
            foreach (var key in matchesKeys)
            {
                Remove(key, utcDateTime);
            }
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        public void Set(CacheKey key, object data)
        {
            Set(key, data, null);
        }

        /// <summary>
        /// Adds the specified key and object to the cache and optionally syncs it to the queue.
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="utcDateTime">The utc date time for the cache entry.</param>
        public void Set(CacheKey key, object data, DateTime? utcDateTime)
        {
            if (key.CacheTime <= 0)
                return;

            _concurrentDictionary.AddOrUpdate(key.Key,
                // add delegate
                (k) =>
                {
                    var entry = new CacheEntry()
                    {
                        CacheTime = key.CacheTime,
                        UtcDateTime = utcDateTime ?? DateTime.UtcNow,
                        Value = data,
                    };

                    if (!utcDateTime.HasValue && IsSerializable(key.Key, data))
                    {
                        CacheMessageQueue.Enqueue(new CacheMessage()
                        {
                            Operation = nameof(Set),
                            Key = key.Key,
                            Entry = entry
                        });
                    }

                    return entry;
                },
                // update delegate
                (k, currentValue) =>
                {
                    // value is the same and current value is not expired and not a sync value.
                    if (Equals(currentValue.Value, data) && !currentValue.Expired && !utcDateTime.HasValue)
                    {
                        return currentValue;
                    }

                    // current value is newer than provided entry.
                    if (currentValue.UtcDateTime > utcDateTime)
                    {
                        return currentValue;
                    }

                    var entry = new CacheEntry()
                    {
                        CacheTime = key.CacheTime,
                        UtcDateTime = utcDateTime ?? DateTime.UtcNow,
                        Value = data,
                    };

                    if (!utcDateTime.HasValue && IsSerializable(key.Key, data))
                    {
                        CacheMessageQueue.Enqueue(new CacheMessage()
                        {
                            Operation = nameof(Set),
                            Key = key.Key,
                            Entry = entry
                        });
                    }

                    return entry;
                });
        }

        /// <summary>
        /// Cleans the memory cache and optionally synchronizes the operation on the queue.
        /// </summary>
        /// <param name="synchronize">Whether to synchronize to the queue</param>
        public void Clear(bool synchronize)
        {
            _concurrentDictionary.Clear();
            if (synchronize)
            {
                // clear the queue, as we cleared the entire dictionary anyway.
                CacheMessageQueue.Clear();
                CacheMessageQueue.Enqueue(new CacheMessage()
                {
                    Operation = nameof(Clear)
                });
            }
        }
    }
}
