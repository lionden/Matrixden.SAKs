﻿using System;

namespace Matrixden.Utils.Models
{
    public class ApiResult : SwissArmyKnives.Models.ApiResult { }
}

namespace Matrixden.SwissArmyKnives.Models
{
    /// <summary>
    /// 接口结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// SUCCESS
        /// </summary>
        public const ushort SUCCESS_SUCCESS = 1;
        /// <summary>
        /// Fail
        /// </summary>
        public const ushort SUCCESS_FAIL = 0;
        const string MESSAGE_SUCCESS = "SUCCESS";
        const string MESSAGE_ERROR = "ERROR";

        /// <summary>
        /// code
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// isSuccess
        /// </summary>
        public ushort isSuccess { get; set; }
        /// <summary>
        /// message
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// data
        /// </summary>
        public dynamic data { get; set; }

        /// <summary>
        /// 失败结果初始化
        /// </summary>
        public ApiResult() { }

        /// <summary>
        /// 将错误信息, 转为接口结果
        /// </summary>
        /// <param name="errorMsg"></param>
        public ApiResult(string errorMsg) : this()
        {

            this.isSuccess = SUCCESS_FAIL;
            this.message = errorMsg;
        }

        /// <summary>
        /// 根据数据实体, 返回接口结果
        /// </summary>
        /// <param name="data"></param>
        public ApiResult(object data) : this()
        {

            code = data == default(dynamic) ? 0 : 1;
            this.isSuccess = SUCCESS_SUCCESS;
            this.message = MESSAGE_SUCCESS;
            this.data = data;
        }

        /// <summary>
        /// 将<c>OperationResult</c>结果转为<c>ApiResult</c>
        /// </summary>
        /// <param name="result"></param>
        public ApiResult(OperationResult result)
        {
            message = result.Message;
            code = result.Code;
            if (result.Result)
            {
                isSuccess = SUCCESS_SUCCESS;
                data = result.Data;
            }
        }
    }
}

namespace Matrixden.Utils.Web.Models
{
    /// <summary>
    /// 接口结果
    /// </summary>
    [Obsolete("Please use Matrixden.Utils.Models.ApiResult instead.")]
    public class ApiResult : Matrixden.Utils.Models.ApiResult
    {

    }
}