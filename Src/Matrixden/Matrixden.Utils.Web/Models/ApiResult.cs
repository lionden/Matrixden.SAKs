using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Web.Models
{
    public class ApiResult
    {
        public const UInt16 SUCCESS_SUCCESS = 1;
        public const UInt16 SUCCESS_FAIL = 0;
        const string MESSAGE_SUCCESS = "SUCCESS";
        const string MESSAGE_ERROR = "ERROR";

        public int code { get; set; }
        public UInt16 isSuccess { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }

        public ApiResult()
        {
            this.isSuccess = SUCCESS_FAIL;
            this.message = MESSAGE_ERROR;
        }

        public ApiResult(string errorMsg)
        {
            this.isSuccess = SUCCESS_FAIL;
            this.message = errorMsg;
        }

        public ApiResult(dynamic data)
        {
            code = data == default(dynamic) ? 0 : 1;
            this.isSuccess = SUCCESS_SUCCESS;
            this.message = MESSAGE_SUCCESS;
        }
    }
}