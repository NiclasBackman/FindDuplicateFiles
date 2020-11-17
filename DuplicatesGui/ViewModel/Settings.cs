using System.Collections.Generic;

namespace DuplicatesGui.ViewModel
{
    public class Settings
    {
        public Settings()
        {
        }

        public Settings(string filter)
        {
            Filter = filter;
        }

        public string Filter { get; set; }
        
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Settings s = (Settings)obj;
                return (Filter == s.Filter);
            }
        }
        
        public override int GetHashCode()
        {
            var lst = new List<object> { Filter };
            return lst.GetHashCode();
        }
    }
}
