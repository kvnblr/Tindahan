namespace Api.Entities.ExceptionModel;

public sealed class InvalidPathBadRequest() : BadRequestException("Could not generate the product endpoint URL.") { }
