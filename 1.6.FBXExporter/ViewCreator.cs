using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBXExporter
{
   public class ViewCreator
   {
      public static View3D Create3DView(Document doc)
      {
         View3D view = null;
         try
         {
            using (Transaction t = new Transaction(doc, "Create view"))
            {
               t.Start();
               //Find a 3D view type
               var collector = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType));
               var viewFamilyTypes = from elem in collector
                                     let vftype = elem as ViewFamilyType
                                     where vftype.ViewFamily == ViewFamily.ThreeDimensional
                                     select vftype;
               // Create a new View3D
               if (viewFamilyTypes.Count() == 0)
               {
                  return null;
               }
               view = View3D.CreateIsometric(doc, viewFamilyTypes.First().Id);

               if (view.CanModifyViewDiscipline())
                  view.Discipline = ViewDiscipline.Coordination;
               if (view.CanModifyDetailLevel())
                  view.DetailLevel = ViewDetailLevel.Fine;
               if (view.CanModifyDisplayStyle())
                  view.DisplayStyle = DisplayStyle.Realistic;

               // show all categories
               foreach (Category cat in doc.Settings.Categories)
               {
                  if (view.GetCategoryHidden(cat.Id))
                  {
                     view.SetCategoryHidden(cat.Id, false);
                  }
               }
               // hide annotations
               view.AreAnnotationCategoriesHidden = true;

               // disable the crop box or section box
               view.CropBoxActive = false;
               view.CropBoxVisible = false;
               view.IsSectionBoxActive = false;

               t.Commit();
            }
            return view;
         }
         catch (Exception )
         {
            return null;
         }
      }
   }
}
