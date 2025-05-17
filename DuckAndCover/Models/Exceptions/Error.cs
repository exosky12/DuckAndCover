using Models.Enums;

namespace Models.Exceptions
{
    public class Error : Exception
    {
        public ErrorCodes ErrorCode { get; }

        public Error(ErrorCodes errorCode, string message = "") : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}