namespace Api.Entities.ExceptionModel;

public abstract class BadRequestException(string message) : Exception(message) { }
