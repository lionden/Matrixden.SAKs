using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils
{
    internal class Constants
    {
        #region Get Public Ip
        internal const string PublicIpVendor_Url = "http://pv.sohu.com/cityjson?ie=utf-8";
        internal const string PublicIpVendor_StartIdentity = "{\"cip\": \"";
        internal const int PublicIpVendor_StartIdentity_Length = 9;
        internal const string PublicIpVendor_EndIdentity = "\", \"cid\": \"";
        #endregion
    }
}
