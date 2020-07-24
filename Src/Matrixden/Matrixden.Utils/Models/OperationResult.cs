/**
 * 一些复合工具实体类型.
 */

namespace Matrixden.Utils.Models
{
    using Matrixden.Utils.Serialization;
    using System;

    /// <summary>
    /// 操作结果实体
    /// </summary>
    public class OperationResult
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
        /// 错误代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 操作消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

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
        public OperationResult(object data)
        {
            if (data != default(object))
            {
                this._result = true;
                this.Message = string.Empty;
                this.Data = data;
            }
        }

        /// <summary>
        /// 失败结果
        /// </summary>
        public static OperationResult False => new OperationResult(false);

        /// <summary>
        /// 成功结果
        /// </summary>
        public static OperationResult True => new OperationResult(true);

        /// <summary>
        /// Logical AND operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static OperationResult operator &(OperationResult left, OperationResult right)
        {
            if (left == default(OperationResult) || right == default(OperationResult))
                return default(OperationResult);

            if (left.Result && right.Result)
                return new OperationResult
                {
                    Result = true,
                    Message = string.Empty,
                    Data = new
                    {
                        left = left.Data,
                        right = right.Data
                    }
                };

            return new OperationResult
            {
                Result = false,
                Message = JsonHelper.SerializeToJsonString(new { left = left.Message, right = right.Message }),
                Data = default(object)
            };
        }

        /// <summary>
        /// Logical OR operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static OperationResult operator |(OperationResult left, OperationResult right)
        {
            if (left == default(OperationResult) || right == default(OperationResult))
                return default(OperationResult);

            return new OperationResult
            {
                Result = left.Result || right.Result,
                Message = JsonHelper.SerializeToJsonString(new { left = left.Message, right = right.Message }),
                Data = new
                {
                    left = left.Data,
                    right = right.Data
                }
            };
        }
    }
}
