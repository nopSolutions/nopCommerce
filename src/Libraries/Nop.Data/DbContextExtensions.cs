using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Nop.Core;

namespace Nop.Data {
    public static class DbContextExtensions {
        /// <summary>
        /// Loads the database copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadDatabaseCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity {
            return InnerGetCopy(context, currentCopy, e => e.GetDatabaseValues());
        }

        private static T InnerGetCopy<T>(IDbContext context, T currentCopy, Func<DbEntityEntry<T>, DbPropertyValues> func) where T : BaseEntity {
            //Get the database context
            DbContext dbContext = CastOrThrow(context);

            //Get the entity tracking object
            DbEntityEntry<T> entry = GetEntityOrReturnNull(currentCopy, dbContext);

            //The output 
            T output = null;

            //Try and get the values
            if (entry != null) {
                DbPropertyValues dbPropertyValues = func(entry);
                if(dbPropertyValues != null) {
                    output = dbPropertyValues.ToObject() as T;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the entity or return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentCopy">The current copy.</param>
        /// <param name="dbContext">The db context.</param>
        /// <returns></returns>
        private static DbEntityEntry<T> GetEntityOrReturnNull<T>(T currentCopy, DbContext dbContext) where T : BaseEntity {
            return dbContext.ChangeTracker.Entries<T>().FirstOrDefault(e => e.Entity == currentCopy);
        }

        private static DbContext CastOrThrow(IDbContext context) {
            DbContext output = (context as DbContext);

            if(output == null) {
                throw new InvalidOperationException("Context does not support operation.");
            }

            return output;
        }

        /// <summary>
        /// Loads the original copy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="currentCopy">The current copy.</param>
        /// <returns></returns>
        public static T LoadOriginalCopy<T>(this IDbContext context, T currentCopy) where T : BaseEntity {
            return InnerGetCopy(context, currentCopy, e => e.OriginalValues);
        }
    }
}