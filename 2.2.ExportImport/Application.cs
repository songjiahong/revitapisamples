using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExportImport
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            Assembly myAssembly = typeof(Application).Assembly;
            string assemblyPath = myAssembly.Location;

            // Create Ribbon Tab
            application.CreateRibbonTab("EIAPP");

            // Create Ribbon Panel to host the button
            RibbonPanel panel = application.CreateRibbonPanel("EIAPP", "MyPanel");

            // Create the push button
            PushButton button = panel.AddItem(new PushButtonData("Export", "ExportElements", assemblyPath, "ExportImport.CommandExport")) as PushButton;
            button.LargeImage = GetEmbeddedImage(myAssembly, "ExportImport.Icons.Export32.png");
            button.Image = GetEmbeddedImage(myAssembly, "ExportImport.Icons.Export16.png");
            button.ToolTip = "Export all elements in current document to a CSV file";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
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
