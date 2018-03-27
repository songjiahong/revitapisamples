using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ExportImport
{
    public class ElementSearch
    {
        public static IList<Element> GetAllElements(Document doc, BuiltInCategory category)
        {
            var collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .OfCategory(category);

            return collector.ToElements();
        }
    }
}
