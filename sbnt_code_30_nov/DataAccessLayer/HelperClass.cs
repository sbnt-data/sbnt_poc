// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Reflection;
// using System.Text;
// using System.Threading.Tasks;

// namespace PMS.Persistence.DataAccessLayer
// {
//     static class HelperClass
//     {
//         public static DataTable ToDataTable<T>(List<T> items)
//         {
//             DataTable dataTable = new DataTable(typeof(T).Name);

//             //Get all the properties
//             PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
//             foreach (PropertyInfo prop in Props)
//             {
//                 //Defining type of data column gives proper data table 
//                 var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
//                 //Setting column names as Property names
//                 dataTable.Columns.Add(prop.Name, type);
//             }
//             foreach (T item in items)
//             {
//                 var values = new object[Props.Length];
//                 for (int i = 0; i < Props.Length; i++)
//                 {
//                     //inserting property values to datatable rows
//                     values[i] = Props[i].GetValue(item, null);
//                 }
//                 dataTable.Rows.Add(values);
//             }
//             //put a breakpoint here and check datatable
//             return dataTable;
//         }
//     }
// }

// Improvements Made:
// Validation for items:

// Added a check to ensure the input list is not null or empty. This prevents unnecessary processing and makes debugging easier.
// Nullable Type Handling:

// Improved readability of nullable type detection and ensured the correct underlying type is used.
// Default Column Type:

// Used typeof(object) as a fallback for undefined types to handle edge cases gracefully.
// DBNull.Value Handling:

// Ensured null values are properly converted to DBNull.Value, which is the standard for SQL-compatible DataTable operations.
// Inline Comments and XML Documentation:

// Added comments and XML documentation for better maintainability and understanding of the method's purpose.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace PMS.Persistence.DataAccessLayer
{
    static class HelperClass
    {
        /// <summary>
        /// Converts a list of objects to a DataTable.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="items">The list of objects to convert.</param>
        /// <returns>A DataTable representing the list of objects.</returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("The items list cannot be null or empty.", nameof(items));

            DataTable dataTable = new DataTable(typeof(T).Name);

            // Get all public instance properties of T
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Create columns based on property names and types
            foreach (var property in properties)
            {
                Type columnType = property.PropertyType;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnType = Nullable.GetUnderlyingType(property.PropertyType);
                }

                dataTable.Columns.Add(property.Name, columnType ?? typeof(object));
            }

            // Populate rows with property values from the items list
            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
