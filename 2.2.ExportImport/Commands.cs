using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ExportImport
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CommandExport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var filePath = @"C:\TempFiles\walls.csv";
            var allElements = ElementSearch.GetAllElements(doc, BuiltInCategory.OST_Walls);
            Exporter.ToCsv(filePath, allElements);

            return Result.Succeeded;
        }
    }
}
