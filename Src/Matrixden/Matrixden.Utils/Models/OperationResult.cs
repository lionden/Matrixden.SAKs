/**
 * 一些复合工具实体类型.
 */

namespace Matrixden.Utils.Models
{
    using Matrixden.Utils.Serialization;
    using System;

    public class OperationResult
    {
        private bool _operationResult;
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result
        {
            get
            {
                return _operationResult;
            }
            set
            {
                _operationResult = value;
                if (_operationResult)
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
        public object Data { get; set; }

        public OperationResult() { }

        public OperationResult(string errorMsg)
        {
            this.Message = errorMsg;
        }

        public OperationResult(bool result)
        {
            this._operationResult = result;
        }

        public OperationResult(object data)
        {
            if (data != default(object))
            {
                this._operationResult = true;
                this.Message = string.Empty;
                this.Data = data;
            }
        }

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
