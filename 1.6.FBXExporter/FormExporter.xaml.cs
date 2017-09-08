using System.Windows;
using WinForms = System.Windows.Forms;
using System.IO;
using Autodesk.Revit.DB;
using System.Linq;
using System;

namespace FBXExporter
{
   /// <summary>
   /// Interaction logic for FormExporter.xaml
   /// </summary>
   public partial class FormExporter : Window
   {
      Autodesk.Revit.ApplicationServices.Application _revitApp;
      public FormExporter(Autodesk.Revit.ApplicationServices.Application revitApp)
      {
         InitializeComponent();
         _revitApp = revitApp;
      }

      private void btnFolder_Click(object sender, RoutedEventArgs e)
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

      private void btnExport_Click(object sender, RoutedEventArgs e)
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
         string targetFolder = TargetFolder.Text;
         if (Directory.Exists(targetFolder) == false)
         {
            Directory.CreateDirectory(targetFolder);
         }
         // find all rvt files including sub directories
         var files = Directory.EnumerateFiles(SourceFolder.Text, "*.rvt", SearchOption.AllDirectories);
         int count = files.Count(), success = 0;
         double curStep = 0, current = 0, tempVal;
         foreach (var f in files)
         {
            if (exportOneFile(f, targetFolder))
            {
               success++;
            }
            current++;
            // show progress when more than 5 percent
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
         // show summary information
         MessageBox.Show("Export " + success + " files successfully, " + (count - success) + " files failed.");
         Close();
      }

      private bool exportOneFile(string file, string targetFolder)
      {
         string fileName = Path.GetFileNameWithoutExtension(file);
         try
         {
            Document doc = _revitApp.OpenDocumentFile(file);
            View3D view = ViewCreator.Create3DView(doc);
            if (view != null)
            {
               FBXExporter.Export(doc, view, targetFolder, fileName);
            }
            doc.Close(false);

            return true;
         }
         catch (Exception ex)
         {
            // write error message into log file
            logError(targetFolder, fileName, ex.Message);
            return false;
         }
      }

      private void btnClose_Click(object sender, RoutedEventArgs e)
      {
         Close();
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
   }
}
