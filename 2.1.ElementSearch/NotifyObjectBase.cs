using System.ComponentModel;

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
