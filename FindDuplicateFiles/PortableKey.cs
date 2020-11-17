using System;
using System.Collections.Generic;
using System.Text;

namespace FindDuplicateFiles
{
    class PortableKey
    {
        public string Name { get; set; }
        public string Hash { get; set; }

        public override bool Equals(object obj)
        {
            PortableKey other = (PortableKey)obj;
            return other.Hash == this.Hash;
        }

        public override int GetHashCode()
        {
            string str = $"{this.Hash}";
            return str.GetHashCode();
        }
        public override string ToString()
        {
            return $"{this.Name} {this.Hash}";
        }
    }
}
