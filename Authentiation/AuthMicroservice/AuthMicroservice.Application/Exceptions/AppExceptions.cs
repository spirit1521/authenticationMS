namespace AuthMicroservice.Application.Exceptions
{
    // Thrown by services, converted to 404 / 400 by the middleware in Program.cs
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
