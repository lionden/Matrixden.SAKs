using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.SwissArmyKnives.Models
{
    /// <summary>
    /// An simple object with only Name and Value property.
    /// </summary>
    public class NameValueModel
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public NameValueModel() { }

        public NameValueModel(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }

        public override bool Equals(object obj)
        {
            if (obj is NameValueModel right)
            {
                return this.Name == right.Name && this.Value == right.Value;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}
