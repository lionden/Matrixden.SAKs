using Matrixden.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrixden.Utils.Web.Models
{
    /// <summary>
    /// 接口结果
    /// </summary>
    public class ApiResult
    {
        public const ushort SUCCESS_SUCCESS = 1;
        public const ushort SUCCESS_FAIL = 0;
        const string MESSAGE_SUCCESS = "SUCCESS";
        const string MESSAGE_ERROR = "ERROR";

        public int code { get; set; }
        public ushort isSuccess { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }

        /// <summary>
        /// 失败结果初始化
        /// </summary>
        public ApiResult()
        {
            this.isSuccess = SUCCESS_FAIL;
            this.message = MESSAGE_ERROR;
        }

        /// <summary>
        /// 将错误信息, 转为接口结果
        /// </summary>
        /// <param name="errorMsg"></param>
        public ApiResult(string errorMsg)
        {
            this.isSuccess = SUCCESS_FAIL;
            this.message = errorMsg;
        }

        /// <summary>
        /// 根据数据实体, 返回接口结果
        /// </summary>
        /// <param name="data"></param>
        public ApiResult(dynamic data)
        {
            code = data == default(dynamic) ? 0 : 1;
            this.isSuccess = SUCCESS_SUCCESS;
            this.message = MESSAGE_SUCCESS;
        }

        /// <summary>
        /// 将<c>OperationResult</c>结果转为<c>ApiResult</c>
        /// </summary>
        /// <param name="result"></param>
        public ApiResult(OperationResult result)
        {
            message = result.Message;
            if (result.Result)
            {
                isSuccess = SUCCESS_SUCCESS;
                code = 1;
                data = result.Data;
            }
        }
    }
}