using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridCreation
{
   /// <summary>
   /// Interaction logic for FormCreation.xaml
   /// </summary>
   public partial class FormCreation : Window
   {
      Autodesk.Revit.UI.UIApplication _revitApp;

      public FormCreation(Autodesk.Revit.UI.UIApplication revitApp)
      {
         InitializeComponent();
         _revitApp = revitApp;
      }

      private void btnCreate_Click(object sender, RoutedEventArgs e)
      {
         // check input first
         try
         {
            List<double> horiSpaces = parseSpaces(HoriSpaces.Text);
            List<double> vertSpaces = parseSpaces(VertSpaces.Text);
            string horiName = HoriStart.Text;
            if (string.IsNullOrEmpty(horiName))
            {
               horiName = "A";
            }
            string vertName = VertStart.Text;
            if (string.IsNullOrEmpty(vertName))
            {
               vertName = "1";
            }
            Visibility = System.Windows.Visibility.Hidden;
            XYZ basePoint = selectBasePoint();
            if (basePoint == null)
            {
               Visibility = System.Windows.Visibility.Visible;
               return;
            }
            createGrids(basePoint, horiSpaces, horiName, true);
            createGrids(basePoint, vertSpaces, vertName, false);
            Close();
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error: " + ex.Message);
            Visibility = System.Windows.Visibility.Visible;
         }
      }

      /// <summary>
      /// Create parallel grids (horizontal or vertical)
      /// </summary>
      /// <param name="basePoint">Top left base point</param>
      /// <param name="spaces">space distances between grids</param>
      /// <param name="startName">Name of the first grid</param>
      /// <param name="isHorizontal">whether this is horizontal or not</param>
      private void createGrids(XYZ basePoint, List<double> spaces, string startName, bool isHorizontal)
      {
         Document doc = _revitApp.ActiveUIDocument.Document;
         double gridLength = 30, extLength = 3; // Unit: feet
         using (Transaction t = new Transaction(doc, "Create Grids"))
         {
            t.Start();
            XYZ offsetDir = isHorizontal ? XYZ.BasisY.Multiply(-1) : XYZ.BasisX;
            XYZ startPoint = isHorizontal ? basePoint.Add(XYZ.BasisX.Multiply(-extLength)) : basePoint.Add(XYZ.BasisY.Multiply(extLength));
            XYZ endPoint = isHorizontal ? startPoint.Add(XYZ.BasisX.Multiply(gridLength)) : startPoint.Add(XYZ.BasisY.Multiply(-gridLength));
            Autodesk.Revit.DB.Line geoLine = Autodesk.Revit.DB.Line.CreateBound(startPoint, endPoint);
            Autodesk.Revit.DB.Grid grid = Autodesk.Revit.DB.Grid.Create(doc, geoLine);
            grid.Name = startName;
            foreach (double space in spaces)
            {
               startPoint = startPoint.Add(offsetDir.Multiply(space));
               endPoint = endPoint.Add(offsetDir.Multiply(space));
               geoLine = Autodesk.Revit.DB.Line.CreateBound(startPoint, endPoint);
               Autodesk.Revit.DB.Grid.Create(doc, geoLine);
            }
            t.Commit();
         }
      }

      private XYZ selectBasePoint()
      {
         try
         {
            return _revitApp.ActiveUIDocument.Selection.PickPoint("Please select a base point!");
         }
         catch (Exception)
         {
            return null;
         }
      }

      private List<double> parseSpaces(string value)
      {
         if (string.IsNullOrEmpty(value))
         {
            throw new Exception("Invalid spaces input!");
         }
         string[] sps = value.Split(',');
         List<double> spaces = new List<double>();
         foreach (string s in sps)
         {
            double val = Convert.ToDouble(s);
            if (val <= 0)
            {
               throw new Exception("Non positive space!");
            }
            spaces.Add(val);
         }
         return spaces;
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
