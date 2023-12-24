using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static partial class MEnumerable
    {
        public static IEnumerable<int> Range(int start, int end)
        {
            if (end >= start)
                return Enumerable.Range(start, end - start + 1);
            else
                return Enumerable.Range(end, start - end + 1).Reverse();
        }
    }
}
