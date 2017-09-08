using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXExporter
{
   public class FBXExporter
   {
      /// <summary>
      /// export a 3d view in a document to a fbx file
      /// </summary>
      /// <param name="doc">The model document</param>
      /// <param name="view">The 3D view will be exported</param>
      /// <param name="folder">result file container folder</param>
      /// <param name="fileName">result file without extension</param>
      /// <returns>true if success, false otherwise</returns>
      public static bool Export(Document doc, View3D view, string folder, string fileName)
      {
         try
         {
            ViewSet set = new ViewSet();
            set.Insert(view);
            return doc.Export(folder, fileName, set, new FBXExportOptions());
         }
         catch (Exception)
         {
            return false;
         }
      }
   }
}
