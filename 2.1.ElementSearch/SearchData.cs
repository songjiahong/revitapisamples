using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElementSearch
{
   public class ElementInfo : NotifyObjectBase
   {
#region Member variables
      private int _elementId;
      private string _name;
#endregion

      public ElementInfo()
      {
         Id = -1;
         Name = "";
      }

      public ElementInfo(Element element)
      {
         Id = element.Id.IntegerValue;
         Name = element.Name;
      }

      public int Id
      {
         get
         {
            return _elementId;
         }
         set
         {
            _elementId = value;
            OnPropertyChanged("Id");
         }
      }

      public string Name
      {
         get
         {
            return _name;
         }
         set
         {
            _name = value;
            OnPropertyChanged("Name");
         }
      }
   }

   public class SearchData : NotifyObjectBase
   {
#region Member variables
      private ObservableCollection<ElementInfo> _elements;
      private ObservableCollection<ElementInfo> _categories;
      private ElementInfo _selectElement;
      private ElementInfo _selectCategory;
      #endregion

      public ObservableCollection<ElementInfo> Elements
      {
         get
         {
            return _elements;
         }
         set
         {
            _elements = value;
            OnPropertyChanged("Elements");
         }
      }

      public ObservableCollection<ElementInfo> Categories
      {
         get
         {
            return _categories;
         }
         set
         {
            _categories = value;
            OnPropertyChanged("Categories");
         }
      }

      public ElementInfo SelectElement
      {
         get
         {
            return _selectElement;
         }
         set
         {
            _selectElement = value;
            OnPropertyChanged("SelectElement");
         }
      }

      public ElementInfo SelectCategory
      {
         get
         {
            return _selectCategory;
         }
         set
         {
            _selectCategory = value;
            OnPropertyChanged("SelectCategory");
         }
      }

      public SearchData()
      {
         Elements = new ObservableCollection<ElementInfo>();
         Categories = new ObservableCollection<ElementInfo>();
         // add and select default one : all
         ElementInfo info = new ElementInfo();
         info.Name = "All";
         Categories.Add(info);
         SelectCategory = info;
      }

      public void Clear()
      {
         Elements.Clear();
      }

      public void AddElement(Element element)
      {
         Elements.Add(new ElementInfo(element));
      }

      public void AddCategory(Category category)
      {
         ElementInfo info = new ElementInfo();
         info.Id = category.Id.IntegerValue;
         info.Name = category.Name;
         Categories.Add(info);
      }
   }
}
