using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Web.Enums
{
    /// <summary>
    /// A helper class for retrieving standard HTTP methods.
    /// </summary>
    [Flags]
    public enum HttpRequestMethod
    {
        /// <summary>
        /// Represents an HTTP GET protocol method.
        /// </summary>
        Get,
        /// <summary>
        /// Represents an HTTP POST protocol method.
        /// </summary>
        Post,
        /// <summary>
        /// Represents an HTTP PUT protocol method.
        /// </summary>
        Put,
        /// <summary>
        /// Represents an HTTP DELETE protocol method.
        /// </summary>
        Delete,
        /// <summary>
        /// Represents an HTTP OPTIONS protocol method.
        /// </summary>
        Options,
        /// <summary>
        /// Represents an HTTP TRACE protocol method.
        /// </summary>
        Trace,
        /// <summary>
        /// Represents an HTTP HEAD protocol method. The HEAD method is identical to GET except that the server only returns message-headers in the response, without a message-body.
        /// </summary>
        Head
    }
}
