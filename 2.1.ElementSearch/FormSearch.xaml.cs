using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
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

namespace ElementSearch
{
   /// <summary>
   /// Interaction logic for FormSearch.xaml
   /// </summary>
   public partial class FormSearch : Window
   {
      private UIDocument _uiDoc;
      private SearchData _data;
      private bool _searchClicked, _showClicked;
      private const int MaxCount = 50;

      public FormSearch(UIDocument uiDoc)
      {
         InitializeComponent();

         _uiDoc = uiDoc;
         _data = new SearchData();
         _searchClicked = _showClicked = false;
      }

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         Application.IdlingHandlers += OnIdling;
         DataContext = _data;

         // Initial all categories
         foreach (Category cat in _uiDoc.Document.Settings.Categories)
         {
            _data.AddCategory(cat);
         }
      }

      private void OnIdling(object sender, IdlingEventArgs e)
      {
         if (_searchClicked)
         {
            _searchClicked = false;
            SearchElements();
         }
         else if (_showClicked)
         {
            _showClicked = false;
            if (_data.SelectElement != null)
            {
               _uiDoc.ShowElements(new ElementId(_data.SelectElement.Id));
            }
         }
      }

      private void SearchElements()
      {
         _data.Clear();
         string name = tbName.Text;
         Document doc = _uiDoc.Document;
         FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
         if (_data.SelectCategory != null && _data.SelectCategory.Id != -1)
         {
            collector = collector.OfCategoryId(new ElementId(_data.SelectCategory.Id));
         }
         var elements = collector.Where(x => x.Name.Contains(name)).Take(MaxCount);
         foreach (var element in elements)
         {
            _data.AddElement(element);
         }
      }

      private void Search_Click(object sender, RoutedEventArgs e)
      {
         if (string.IsNullOrEmpty(tbName.Text))
         {
            MessageBox.Show("Please input name filter text!");
            tbName.Focus();
            return;
         }
         _searchClicked = true;
      }

      private void Show_Click(object sender, RoutedEventArgs e)
      {
         if (_searchClicked || _data.SelectElement == null)
            return;

         _showClicked = true;
      }

      private void tbName_KeyUp(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Enter)
         {
            Search_Click(sender, e);
         }
      }

      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         Application.IdlingHandlers -= OnIdling;
         Command.searchDialog = null;
      }
   }
}
