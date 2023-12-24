using System;
using System.Linq;
using System.Reflection;
using Matrixden.Utils.Extensions;

namespace Matrixden.Zion.Models
{
    public class ZionModel
    {
        public virtual Guid ID { get; set; }

        public virtual string Remark { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual int CreateMan { get; set; }

        public virtual DateTime UpdateTime { get; set; }

        public virtual int UpdateMan { get; set; }

        public virtual short Status { get; set; }
    }
}