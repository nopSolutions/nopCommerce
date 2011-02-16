using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace Nop.Core.Configuration
{
    /// <summary>
    /// Allws to "remove" items not in the collection. The reader of the configuration must 
    /// implement "remove" semantics reading from the RemovedElements collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LazyRemovableCollection<T> : ConfigurationElementCollection, IEnumerable<T>, IEnumerable
        where T : ConfigurationElement, IIdentifiable, new()
    {
        List<T> removedElements = new List<T>();
        List<T> defaults = new List<T>();
        bool isCleared;

        /// <summary>Elements that were "removed".</summary>
        public IEnumerable<T> RemovedElements
        {
            get { return removedElements.AsReadOnly(); }
            set { removedElements = new List<T>(value ?? new T[0]); }
        }

        /// <summary>Elements that were "removed".</summary>
        public IEnumerable<T> Defaults
        {
            get { return defaults.AsReadOnly(); }
            set { defaults = new List<T>(value ?? new T[0]); }
        }

        /// <summary>All added elements except those that have been removed but not default elements.</summary>
        public IEnumerable<T> AddedElements
        {
            get
            {
                object[] removedKeys = RemovedElements.Select(e => e.ElementKey).ToArray();
                foreach (object key in base.BaseGetAllKeys())
                {
                    if (!removedKeys.Contains(key))
                        yield return base.BaseGet(key) as T;
                }
            }
        }

        /// <summary>All added elements except those that have been removed.</summary>
        public IEnumerable<T> AllElements
        {
            get
            {
                object[] removedKeys = RemovedElements.Select(e => e.ElementKey).ToArray();
                foreach (T element in Defaults)
                {
                    var key = ((IIdentifiable)element).ElementKey;
                    if (!removedKeys.Contains(key))
                        yield return element;
                }
                foreach (T element in AddedElements)
                {
                    yield return element;
                }
            }
            set
            {
                BaseClear();
                defaults.Clear();
                if (value == null) return;

                foreach (T element in value)
                    BaseAdd(element);
            }
        }

        public bool IsCleared
        {
            get { return isCleared; }
        }

        /// <summary>Adds an element to the collection of defaults.</summary>
        /// <param name="element">The element to add.</param>
        public void AddDefault(T element)
        {
            defaults.Add(element);
        }

        /// <summary>Adds an element to the collection.</summary>
        /// <param name="element">The element to add.</param>
        public void Add(T element)
        {
            BaseAdd(element);
        }

        /// <summary>"Removes" an element from the collection.</summary>
        /// <param name="element">The element to "remove".</param>
        public void Remove(T element)
        {
            removedElements.Add(element);
        }

        /// <summary>Clears all the elements in this collection.</summary>
        public void Clear()
        {
            isCleared = true;
            defaults.Clear();
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IIdentifiable)element).ElementKey;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (elementName == "remove")
            {
                T element = new T();
                element.ElementKey = reader.GetAttribute("name");

                OnDeserializeRemoveElement(element, reader);

                Remove(element);
                return true;
            }
            if (elementName == "clear")
            {
                Clear();
                return true;
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }

        protected virtual void OnDeserializeRemoveElement(T element, XmlReader reader)
        {
        }

        public new int Count
        {
            get { return AllElements.Count(); }
        }

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            return AllElements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AllElements.GetEnumerator();
        }

        #endregion
    }
}
