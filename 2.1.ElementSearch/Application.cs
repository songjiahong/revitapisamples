using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ElementSearch
{
   [Transaction(TransactionMode.Manual)]
   [Regeneration(RegenerationOption.Manual)]
   public class Application : IExternalApplication
   {
      public static event EventHandler<IdlingEventArgs> IdlingHandlers;

      public Result OnStartup(UIControlledApplication application)
      {
         Assembly myAssembly = typeof(Application).Assembly;
         string assemblyPath = myAssembly.Location;

         // Create Ribbon Tab
         application.CreateRibbonTab("MyAPP");

         // Create Ribbon Panel to host the button
         RibbonPanel panel = application.CreateRibbonPanel("MyAPP", "MyPanel");

         // Create the push button
         PushButton button = panel.AddItem(new PushButtonData("Search", "SearchElements", assemblyPath, "ElementSearch.Command")) as PushButton;
         button.LargeImage = GetEmbeddedImage(myAssembly, "ElementSearch.Icons.search-icon32.png");
         button.Image = GetEmbeddedImage(myAssembly, "ElementSearch.Icons.search-icon16.png");
         button.ToolTip = "Search and show element in a document based on category and name";

         application.Idling += OnIdling;
         return Result.Succeeded;
      }

      private void OnIdling(object sender, IdlingEventArgs e)
      {
         if (IdlingHandlers != null)
         {
            IdlingHandlers(sender, e);
         }
      }

      public Result OnShutdown(UIControlledApplication application)
      {
         if (Command.searchDialog != null)
         {
            Command.searchDialog.Close();
         }
         application.Idling -= OnIdling;
         return Result.Succeeded;
      }

      private ImageSource GetEmbeddedImage(System.Reflection.Assembly assemb, string imageName)
      {
         System.IO.Stream file = assemb.GetManifestResourceStream(imageName);
         PngBitmapDecoder bd = new PngBitmapDecoder(file, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

         return bd.Frames[0];
      }
   }
}
