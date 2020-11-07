using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DuplicatesGui.ViewModel
{
    public class AttributeContainer<T> : INotifyPropertyChanged
    {
        private T attrValue;
        private readonly ValidationRule rule;
        public event PropertyChangedEventHandler PropertyChanged;

        public AttributeContainer(T attribute, ValidationRule validationRule)
        {
            attrValue = attribute;
            rule = validationRule;
        }

        public T Value
        {
            get
            {
                return attrValue;
            }
            set
            {
                attrValue = value;
                OnPropertyChanged("Value");
            }
        }

        public ValidationRule Rule => rule;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
