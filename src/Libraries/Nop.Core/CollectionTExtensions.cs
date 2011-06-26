using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core
{
    public static class CollectionTExtensions
    {
        public static ICollection<T> SetupBeforeAndAfterActions<T>(
            this ICollection<T> value,
            Action<dynamic> setParent,
            Action<dynamic> setParentToNull)
            where T : class
        {
            if (!CommonHelper.OneToManyCollectionWrapperEnabled)
                return value;

            var list = value as IPersistentCollection<T> ?? new PersistentCollection<T>(value);
            list.BeforeAdd = (l, x) => l.BeforeAddItem(x, setParent);
            list.BeforeRemove = (l, x) => l.BeforeRemoveItem(x, setParentToNull);
            list.AfterAdd = AfterListChanges;
            list.AfterRemove = AfterListChanges;
            return list;
        }

        public static void AfterListChanges<T>(this ICollection<T> list) where T : class
        {
            // ...
        }

        public static bool BeforeAddItem<T>(this ICollection<T> list, T item, Action<T> setParent) where T : class
        {
            // ...
            setParent(item);
            if (list.Any(item.Equals))
            {
                return false;
            }
            return true;
        }

        public static bool BeforeRemoveItem<T>(this ICollection<T> list, T item, Action<T> setParentToNull) where T : class
        {
            setParentToNull(item);
            if (list.Any(item.Equals))
            {
                return true;
            }
            return false;
        }
    }
}
