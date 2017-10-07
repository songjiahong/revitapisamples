using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Autodesk.Revit.DB;

namespace BatchFamilyExport
{
   /// <summary>
   /// Interaction logic for FormExport.xaml
   /// </summary>
   public partial class FormExport : Window
   {
      private Document currentDoc;

      public FormExport(Document doc)
      {
         InitializeComponent();
         currentDoc = doc;
      }

      private void btnTarget_Click(object sender, RoutedEventArgs e)
      {
         var dialog = new WinForms.FolderBrowserDialog();
         if (WinForms.DialogResult.OK == dialog.ShowDialog())
         {
            TargetFolder.Text = dialog.SelectedPath;
         }
      }

      private void btnExport_Click(object sender, RoutedEventArgs e)
      {
         // check select target folder
         if (string.IsNullOrEmpty(TargetFolder.Text))
         {
            MessageBox.Show("Please select target folder!");
            TargetFolder.Focus();
            return;
         }
         string targetFolder = TargetFolder.Text;
         if (Directory.Exists(targetFolder) == false)
         {
            Directory.CreateDirectory(targetFolder);
         }
         int exported = exportFamilies(targetFolder, progressUpdater);
         MessageBox.Show(string.Format("Exported {0} families!", exported));
         Close();
      }

      private void progressUpdater(double percentage)
      {
         if (percentage - 5 > progressBar.Value || percentage == 100)
         {
            progressBar.Value = percentage;
            RAPWPF.WpfApplication.DoEvents();
         }
      }

      /// <summary>
      /// export all custom families in current document to a given folder
      /// </summary>
      /// <param name="targetFolder">target folder to store all families</param>
      /// <param name="progressUpdater">progress updater</param>
      /// <returns>number of exported families</returns>
private int exportFamilies(string targetFolder, Action<double> progressUpdater)
{
   FilteredElementCollector collector = new FilteredElementCollector(currentDoc).OfClass(typeof(Family));
   List<Family> list = collector.OfType<Family>().ToList();
   int count = list.Count, exported = 0;
   double index = 0;
   foreach (Family item in list)
   {
      if (item.IsEditable)
      {
         string fileName = System.IO.Path.Combine(targetFolder, item.Name + ".rfa");
         Document familyDoc = currentDoc.EditFamily(item);
         if (familyDoc != null)
         {
            // make sure it's a real family document
            if (familyDoc.IsFamilyDocument)
               familyDoc.SaveAs(fileName);
            familyDoc.Close(false);
         }
         ++exported;
      }
      progressUpdater(++index * 100 / count);
   }
   return exported;
}
   }
}
