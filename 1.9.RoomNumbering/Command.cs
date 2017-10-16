using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomNumbering
{
   [Transaction(TransactionMode.Manual)]
   [Regeneration(RegenerationOption.Manual)]
   public class Command : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         Document doc = commandData.Application.ActiveUIDocument.Document;
         RoomFilter filter = new RoomFilter(doc);
         List<Room> rooms = filter.GetAllRooms();
         if (rooms == null || rooms.Count == 0)
         {
            return Result.Cancelled;
         }
         RoomUtil util = new RoomUtil(doc);
         util.Numbering(rooms);
         return Result.Succeeded;
      }
   }
}
