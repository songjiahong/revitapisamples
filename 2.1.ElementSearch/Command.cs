using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementSearch
{
   [Transaction(TransactionMode.Manual)]
   [Regeneration(RegenerationOption.Manual)]
   public class Command : IExternalCommand
   {
      public static FormSearch searchDialog = null;

      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         if (searchDialog == null)
         {
            searchDialog = new FormSearch(commandData.Application.ActiveUIDocument);
            searchDialog.Show();
         }
         return Result.Succeeded;
      }
   }
}
