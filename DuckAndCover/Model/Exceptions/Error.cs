namespace Model.Exceptions
{
    public class Error : Exception
    {
        public int ErrorCode { get; }

        public Error(int errorCode, string message = "") : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}