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
            DataTable dataTable = new DataTable("DataTable");
            Type t = typeof(T);
            PropertyInfo[] propertyInfos = t.GetProperties();

            //Inspect the properties and create the columns in the DataTable
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Type ColumnType = propertyInfo.PropertyType;
                if ((ColumnType.IsGenericType))
                {
                    ColumnType = ColumnType.GetGenericArguments()[0];
                }
                dataTable.Columns.Add(propertyInfo.Name, ColumnType);
            }

            //Populate the data table
            foreach (var item in collection)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow.BeginEdit();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetValue(item, null) != null)
                    {
                        dataRow[propertyInfo.Name] = propertyInfo.GetValue(item, null);
                    }
                }
                dataRow.EndEdit();
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
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
