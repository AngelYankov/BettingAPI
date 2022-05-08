using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace BettingAPI.DataContext.Infrastructure
{
    public static class Extensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            DataTable dt = new DataTable("DataTable");
            Type t = typeof(T);
            PropertyInfo[] pia = t.GetProperties();

            //Inspect the properties and create the columns in the DataTable
            foreach (PropertyInfo pi in pia)
            {
                Type ColumnType = pi.PropertyType;
                if ((ColumnType.IsGenericType))
                {
                    ColumnType = ColumnType.GetGenericArguments()[0];
                }
                //if (pi.Name != "MatchesHistory" && pi.Name != "SportHistory" && pi.Name != "EventHistories")
                //{
                dt.Columns.Add(pi.Name, ColumnType);
                //}
            }

            //Populate the data table
            foreach (T item in collection)
            {
                DataRow dr = dt.NewRow();
                dr.BeginEdit();
                foreach (PropertyInfo pi in pia)
                {
                    if (pi.GetValue(item, null) != null)
                    {
                        //if (pi.Name != "MatchesHistory" && pi.Name != "SportHistory" && pi.Name != "EventHistories")
                        //{
                        dr[pi.Name] = pi.GetValue(item, null);
                        //}
                    }
                }
                dr.EndEdit();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
