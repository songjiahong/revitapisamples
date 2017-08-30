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

namespace BatchUpgrader
{
   /// <summary>
   /// Interaction logic for FormUpgrade.xaml
   /// </summary>
   public partial class FormUpgrade : Window
   {
      Autodesk.Revit.ApplicationServices.Application _revitApp;
      public FormUpgrade(Autodesk.Revit.ApplicationServices.Application revitApp)
      {
         InitializeComponent();
         _revitApp = revitApp;
      }

      private void btnSource_Click(object sender, RoutedEventArgs e)
      {
         var dialog = new WinForms.FolderBrowserDialog();
         if (WinForms.DialogResult.OK == dialog.ShowDialog())
         {
            SourceFolder.Text = dialog.SelectedPath;
         }
      }

      private void btnTarget_Click(object sender, RoutedEventArgs e)
      {
         var dialog = new WinForms.FolderBrowserDialog();
         if (WinForms.DialogResult.OK == dialog.ShowDialog())
         {
            TargetFolder.Text = dialog.SelectedPath;
         }
      }

      private void btnUpgrade_Click(object sender, RoutedEventArgs e)
      {
         // check input first
         if (string.IsNullOrEmpty(SourceFolder.Text))
         {
            MessageBox.Show("Please select source folder!");
            SourceFolder.Focus();
            return;
         }
         if (Directory.Exists(SourceFolder.Text) == false)
         {
            MessageBox.Show("Source folder does not exist!");
            SourceFolder.Focus();
            return;
         }
         if (string.IsNullOrEmpty(TargetFolder.Text))
         {
            MessageBox.Show("Please select target folder!");
            TargetFolder.Focus();
            return;
         }
         string sourceFolder = SourceFolder.Text.TrimEnd('\\'), targetFolder = TargetFolder.Text.TrimEnd('\\');
         if (sourceFolder.IndexOf(targetFolder, StringComparison.OrdinalIgnoreCase) == 0 
            || targetFolder.IndexOf(sourceFolder, StringComparison.OrdinalIgnoreCase) == 0)
         {
            string extra = "";
            if (sourceFolder.Length > targetFolder.Length)
            {
               extra = sourceFolder.Substring(targetFolder.Length);
            }
            else
            {
               extra = targetFolder.Substring(sourceFolder.Length);
            }
            if (extra.Length == 0 || extra[0] == '\\')
            {
               MessageBox.Show("Target folder cannot be a parent or child folder of source folder!");
               TargetFolder.Focus();
               return;
            }
         }
         if (cbRvt.IsChecked == false && cbRfa.IsChecked == false)
         {
            MessageBox.Show("At least check one Revit file type!");
            return;
         }
         upgradeFolder(sourceFolder, targetFolder, (cbRvt.IsChecked == true), (cbRfa.IsChecked == true));
      }

      private void upgradeFolder(string sourceFolder, string targetFolder, bool includeRvt, bool includeRfa)
      {
         if (Directory.Exists(targetFolder) == false)
         {
            Directory.CreateDirectory(targetFolder);
         }
         var files = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.AllDirectories)
                        .Where(s => (includeRvt && s.EndsWith(".rvt", StringComparison.OrdinalIgnoreCase))
                                       || (includeRfa && s.EndsWith(".rfa", StringComparison.OrdinalIgnoreCase)));
         int count = files.Count(), success = 0, failed = 0;
         double curStep = 0, current = 0, tempVal;
         foreach (string file in files)
         {
            if (upgradeOneFile(sourceFolder, targetFolder, file))
            {
               listDetail.Items.Insert(0, System.IO.Path.GetFileName(file) + " upgrades successfully!");
               success++;
            }
            else
            {
               listDetail.Items.Insert(0, System.IO.Path.GetFileName(file) + " upgrades failed!");
               failed++;
            }
            current++;
            tempVal = current * 100 / count;
            if (tempVal - 5 > curStep)
            {
               curStep = tempVal;
               progressBar.Value = curStep;
               RAPWPF.WpfApplication.DoEvents();
            }
         }
         progressBar.Value = 100;
         RAPWPF.WpfApplication.DoEvents();
         MessageBox.Show("Upgrade " + success + " files successfully, " + failed + " files failed.");
         Close();
      }

      private bool upgradeOneFile(string sourceFolder, string targetFolder, string file)
      {
         string fileName = file.Substring(sourceFolder.Length).TrimStart('\\');
         try
         {
            Document doc = _revitApp.OpenDocumentFile(file);
            string targetFile = System.IO.Path.Combine(targetFolder, fileName);
            string folder = System.IO.Path.GetDirectoryName(targetFile);
            if (Directory.Exists(folder) == false)
            {
               Directory.CreateDirectory(folder);
            }
            SaveAsOptions option = new SaveAsOptions();
            option.Compact = true;
            option.OverwriteExistingFile = true;
            // set preview view id
            var setting = doc.GetDocumentPreviewSettings();
            if (setting.PreviewViewId != ElementId.InvalidElementId)
            {
               option.PreviewViewId = setting.PreviewViewId;
            }
            else
            {
               // Find a candidate 3D view
               ElementId idView = findPreviewId<View3D>(doc, setting);
               if (idView == ElementId.InvalidElementId)
               {
                  // no 3d view, try a plan view
                  idView = findPreviewId<ViewPlan>(doc, setting);
               }
               if (idView != ElementId.InvalidElementId)
               {
                  option.PreviewViewId = idView;
               }
            }
            doc.SaveAs(targetFile, option);
            doc.Close();

            return true;
         }
         catch (Exception ex)
         {
            logError(targetFolder, fileName, ex.Message);
            return false;
         }
      }

      private ElementId findPreviewId<T>(Document doc, DocumentPreviewSettings setting) where T : View
      {
         FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(T));
         Func<T, bool> isValidForPreview = v => setting.IsViewIdValidForPreview(v.Id);
         T viewForPreview = collector.OfType<T>().First<T>(isValidForPreview);
         if (viewForPreview != null)
         {
            return viewForPreview.Id;
         }
         else
         {
            return ElementId.InvalidElementId;
         }
      }

      /// <summary>
      /// Log errors in the root of target folder, file named error.log
      /// </summary>
      /// <param name="targetFolder"></param>
      /// <param name="error"></param>
      private void logError(string targetFolder, string fileName, string error)
      {
         string logFile = System.IO.Path.Combine(targetFolder, "error.log");
         using (StreamWriter sw = new StreamWriter(logFile, true))
         {
            sw.WriteLine("File: " + fileName + " upgrades fail: " + error);
         }
      }

      private void btnClose_Click(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
