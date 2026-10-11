namespace Api.Entities.ExceptionModel;

public abstract class ConflictException(string message)
    : Exception(message)
{ }
