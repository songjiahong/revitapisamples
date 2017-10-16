using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomNumbering
{
   public class RoomFilter
   {
      private Document Document { get; set; }

      public RoomFilter(Document doc)
      {
         Document = doc;
      }

      public List<Room> GetAllRooms()
      {
         FilteredElementCollector collector = new FilteredElementCollector(Document);
         var rooms = collector.OfCategory(BuiltInCategory.OST_Rooms).OfType<Room>();
         return rooms.ToList();
      }
   }
}
