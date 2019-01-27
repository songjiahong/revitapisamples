using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace FamilyLoader
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CommandLoader : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var filePath = @"C:\TempFiles\";

            try
            {
                var rfaPath = @"C:\TempFiles\Test_Fixed.rfa";
                var symbolName = "0610 x 1220mm";
                FamilySymbol symbol;

                using (var t = new Transaction(doc, "Load symbol"))
                {
                    t.Start();
                    var isSuccess = doc.LoadFamilySymbol(rfaPath, symbolName, new DefaultFamilyLoadOptions(), out symbol);

                    if (isSuccess)
                    {
                        t.Commit();
                        TaskDialog.Show("Info", "Load Symbol Success!" + symbol.Id.ToString());
                    }
                    else
                    {
                        t.RollBack();
                        TaskDialog.Show("Info", "Load Symbol fail");
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
