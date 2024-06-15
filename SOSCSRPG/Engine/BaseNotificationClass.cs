using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Engine
{
    public class BaseNotificationClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {//CallerMemberName - explain: attribute look and see who called the OnPropertyChanged function ->
         //Then CallerMemberName will bring us the property name value...
         //it helps us write safer and cleaner code
         //we can call the function like that `OnPropertyChanged();`
         //and now necessary call with the 'nameof' like that `OnPropertyChanged(nameof(Name));`
         //because `[CallerMemberName]` will get name automatically

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
