namespace AccountBalance.Reactive
{
    using System;
    public sealed class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        { }
    }
}
