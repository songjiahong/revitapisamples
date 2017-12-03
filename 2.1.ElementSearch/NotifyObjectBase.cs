using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElementSearch
{
   public class NotifyObjectBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;
      protected void OnPropertyChanged(string propertyName)
      {
         if (this.PropertyChanged != null)
         {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }
      }
   }
}
