namespace ExpenseTracker.API.DTO
{
    public record Result
    {
        public bool Success { get; init; }
        public ErrorMessage? ErrorMessage { get; init; }
        public StatusCode StatusCode { get; init; }

        private Result(bool isSuccess, StatusCode statusCode, ErrorMessage? errorMessage = null)
        {
            Success = isSuccess;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public static Result NoContentResult()
        {
            return new(true, StatusCode.NoContent);
        }

        public static Result BadRequestResult(ErrorMessage errorMessage)
        {
            return new(false, StatusCode.BadRequest, errorMessage);
        }

        public static Result NotFoundResult(ErrorMessage errorMessage)
        {
            return new Result(false, StatusCode.NotFound, errorMessage);
        }

        public static Result UnauthorizeResult()
        {
            return new Result(false, StatusCode.Unauthorize);
        }
    }

    public record Result<T>
    {
        public bool Success { get; init; }
        public ErrorMessage? ErrorMessage { get; init; }
        public T? Data { get; init; }
        public StatusCode StatusCode { get; init; }

        private Result(bool isSuccess, StatusCode statusCode, T? data = default, ErrorMessage? errorMessage = null)
        {
            Success = isSuccess;
            StatusCode = statusCode;
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static Result<T> OkResult(T data)
        {
            return new Result<T>(true, StatusCode.Ok, data);
        }

        public static Result<T> CreatedResult(T data)
        {
            return new Result<T>(true, StatusCode.Created, data);
        }

        public static Result<T> BadRequestResult(ErrorMessage errorMessage)
        {
            return new Result<T>(false, StatusCode.BadRequest, errorMessage: errorMessage);
        }

        public static Result<T> NotFoundResult(ErrorMessage errorMessage)
        {
            return new Result<T>(false, StatusCode.NotFound, errorMessage: errorMessage);
        }

        public static Result<T> UnauthorizeResult()
        {
            return new Result<T>(false, StatusCode.Unauthorize);
        }
        
        public static Result<T> UnauthorizeResult(ErrorMessage errorMessage)
        {
            return new Result<T>(false, StatusCode.Unauthorize, errorMessage: errorMessage);
        }
    }

    public enum StatusCode
    {
        Ok, Created, NoContent, NotFound, BadRequest, Unauthorize
    }
}
