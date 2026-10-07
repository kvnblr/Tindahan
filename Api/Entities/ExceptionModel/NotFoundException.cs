namespace Api.Entities.ExceptionModel;

public abstract class NotFoundException(string message)
    : Exception(message)
{ }
