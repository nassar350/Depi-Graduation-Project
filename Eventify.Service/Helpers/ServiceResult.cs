namespace Eventify.Service.Helpers
{
    public class ServiceResult<T>
    {
        public bool Success { get; private set; }
        public List<ValidationError> Errors { get; private set; } = new();
        public T Data { get; private set; }

        public static ServiceResult<T> Ok(T data)
            => new ServiceResult<T> { Success = true, Data = data };

        public static ServiceResult<T> Fail(string field, string message)
            => new ServiceResult<T>
            {
                Success = false,
                Errors = new List<ValidationError>
                   {
                   new ValidationError { Field = field, Message = message }
                   }
            };

        public static ServiceResult<T> Fail(List<ValidationError> errors)
            => new ServiceResult<T>
            {
                Success = false,
                Errors = errors
            };
    }

    public class ValidationError
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

}
