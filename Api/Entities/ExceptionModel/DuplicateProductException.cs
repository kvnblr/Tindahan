namespace Api.Entities.ExceptionModel;

public sealed class DuplicateProductException()
    : ConflictException("Product with this name already exists.")
{ }
