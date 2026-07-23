using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCore.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }

        public string? Error { get; private set; }
        public int StatusCode { get; private set; }

        private Result(bool isSuccess, T? value, string? error, int statusCode)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            StatusCode = statusCode;
        }

        public static Result<T> Success(T value)
            => new Result<T>(true, value, null, 200);

        public static Result<T> Failure(string error, int statusCode)
            => new Result<T>(false, default, error, statusCode);
    }
}
