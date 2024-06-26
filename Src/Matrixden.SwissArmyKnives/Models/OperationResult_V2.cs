﻿
using System;

/**
 * 一些复合工具实体类型.
 */
namespace Matrixden.Utils.Models
{
    [Obsolete("OBSOLETE! Please use Matrixden.SwissArmyKnives.Models.OperationResult<T> instead.")]
    public class OperationResult<T> : SwissArmyKnives.Models.OperationResult<T> where T : class, new()
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public OperationResult() : base() { }

        /// <summary>
        /// 带错误消息的构造函数
        /// </summary>
        /// <param name="errorMsg"></param>
        public OperationResult(string errorMsg) : base(errorMsg) { }

        /// <summary>
        /// 仅待失败结果的构造函数
        /// </summary>
        /// <param name="result"></param>
        public OperationResult(bool result) : base(result) { }

        /// <summary>
        /// 待数据返还的构造函数
        /// </summary>
        /// <param name="data"></param>
        public OperationResult(T data) : base(data) { }
    }
}

namespace Matrixden.SwissArmyKnives.Models
{
    /// <summary>
    /// 操作结果实体
    /// </summary>
    public class OperationResult<T> where T : class, new()
    {
        private bool _result;
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result
        {
            get => _result;
            set
            {
                _result = value;
                if (_result)
                {
                    Message = null;
                }
            }
        }

        /// <summary>
        /// 操作消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public OperationResult() { }

        /// <summary>
        /// 带错误消息的构造函数
        /// </summary>
        /// <param name="errorMsg"></param>
        public OperationResult(string errorMsg)
        {
            this.Message = errorMsg;
        }

        /// <summary>
        /// 仅待失败结果的构造函数
        /// </summary>
        /// <param name="result"></param>
        public OperationResult(bool result)
        {
            this._result = result;
        }

        /// <summary>
        /// 待数据返还的构造函数
        /// </summary>
        /// <param name="data"></param>
        public OperationResult(T data)
        {
            if (data != default(T))
            {
                this._result = true;
                this.Message = string.Empty;
                this.Data = data;
            }
        }

        /// <summary>
        /// 失败结果
        /// </summary>
        public static OperationResult<T> False => new OperationResult<T>(false);
    }
}
