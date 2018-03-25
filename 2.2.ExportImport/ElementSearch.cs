using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ExportImport
{
    public class ElementSearch
    {
        public static IList<Element> GetAllElements(Document doc)
        {
            var collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().WhereElementIsViewIndependent();

            return collector.ToElements();
        }
    }
}
