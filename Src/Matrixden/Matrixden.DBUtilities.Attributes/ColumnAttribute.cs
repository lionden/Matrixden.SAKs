using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.DBUtilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public bool IsPK { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="pk">是否是主键</param>
        public ColumnAttribute(string name, bool pk)
        {
            this.Name = name;
            this.IsPK = pk;
        }

        public ColumnAttribute(string name) : this(name, false) { }
        public ColumnAttribute(bool pk) : this(string.Empty, pk) { }
    }
}
