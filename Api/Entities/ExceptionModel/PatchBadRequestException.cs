namespace Api.Entities.ExceptionModel;

public sealed class PatchBadRequestException(string message) : BadRequestException(message) { }
