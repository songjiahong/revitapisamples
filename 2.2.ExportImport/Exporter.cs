using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExportImport
{
    public class Exporter
    {
        private const string Separator = ",";

        public static void ToCsv(string filePath, IList<Element> elements)
        {
            if (elements == null || elements.Count == 0)
            {
                return;
            }

            // iterate all elements to get the parameter values and all parameter names
            // put Id, Name, CategoryName for all elements
            var paramNames = new List<string>()
            {
                "Id", "Name", "CategoryName"
            };

            var values = new List<Dictionary<string, string>>();
            foreach (var elem in elements)
            {
                var oneRow = new Dictionary<string, string>();
                oneRow.Add("Id", elem.Id.ToString());
                oneRow.Add("Name", elem.Name);
                oneRow.Add("CategoryName", elem.Category?.Name);

                foreach (Parameter param in elem.Parameters)
                {
                    var paramName = param.Definition.Name;
                    if (paramNames.Contains(paramName) == false)
                    {
                        paramNames.Add(paramName);
                    }
                    oneRow.Add(paramName, Csv.Escape(GetParamValue(param)));
                }
                values.Add(oneRow);
            }
            using (var textWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // write header
                textWriter.WriteLine(string.Join(Separator, paramNames));
                // write rows
                foreach (var row in values)
                {
                    var lstValue = new List<string>();
                    foreach (var s in paramNames)
                    {
                        if (row.ContainsKey(s))
                        {
                            lstValue.Add(row[s] ?? string.Empty);
                        }
                        else
                        {
                            lstValue.Add(string.Empty);
                        }
                    }
                    textWriter.WriteLine(string.Join(Separator, lstValue));
                }
            }
        }

        private static string GetParamValue(Parameter param)
        {
            switch (param.StorageType)
            {
                case StorageType.Double:
                    return param.AsValueString();
                case StorageType.ElementId:
                    return param.AsElementId().IntegerValue.ToString();
                case StorageType.Integer:
                    return param.AsInteger().ToString();
                case StorageType.String:
                    return param.AsString();
                default:
                    return string.Empty;
            }
        }
    }
}
