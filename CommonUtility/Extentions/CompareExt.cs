using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtility.Extentions
{
    public static class CompareExt
    {
        public static (IList<(S src, T newitem)> updateList, IList<S> deleteList, IList<T> addList) CompareToList<S, T>(this ICollection<S> source, ICollection<T> newList, Func<S, T, bool> isEqual)
        {
            return FindChangeToList(source, newList, isEqual);
        }
        public static (IList<(S src, T newitem)> updateList, IList<S> deleteList, IList<T> addList) FindChangeToList<S, T>(this ICollection<S> source, ICollection<T> newList, Func<S, T, bool> isEqual)
        {
            var updateModel = new List<(S, T)>();
            var updateList = source.Where(e => newList.Any(f =>
            {
                var result = isEqual(e, f);
                if (result)
                {
                    updateModel.Add((e, f));
                }
                return result;
            })).ToList();



            var deleteList = source.Where(e => newList.All(f => !isEqual(e, f))).ToList();
            var addList = newList.Where(f => source.All(e => !isEqual(e, f))).ToList();

            return (updateModel, deleteList, addList);
        }
    }
}
