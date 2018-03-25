using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RoomNumbering
{
   public class RoomUtil
   {
      private Document Document { get; set; }

      public RoomUtil(Document doc)
      {
         Document = doc;
      }

      public void Numbering(List<Room> rooms)
      {
         // group the rooms by level
         Dictionary<int, List<Room>> roomsByLevel = new Dictionary<int, List<Room>>();
         foreach (Room room in rooms)
         {
            int order = getOrderByLevel(room.Level);
            if (roomsByLevel.ContainsKey(order) == false)
            {
               roomsByLevel.Add(order, new List<Room>());
            }
            roomsByLevel[order].Add(room);
         }
         // rename the rooms
         int sign = 1;
         Transaction t = new Transaction(Document, "Rename Rooms");
         t.Start();
         foreach (var kv in roomsByLevel)
         {
            sign = kv.Key < 0 ? -1 : 1;
            int index = kv.Key * 100 + sign;
            foreach (Room r in kv.Value)
            {
               r.Name = index.ToString();
               index += sign;
            }
         }
         t.Commit();
      }

      private int getOrderByLevel(Level level)
      {
         if (level == null)
         {
            return 0;
         }
         string numberInString = Regex.Match(level.Name, @"-?\d+").Value;
         if (string.IsNullOrEmpty(numberInString))
         {
            return 0;
         }
         return Convert.ToInt32(numberInString);
      }
   }
}
