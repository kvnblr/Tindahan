namespace Api.Entities.ExceptionModel;

public sealed class ProductNotFoundException(int id)
    : NotFoundException($"The product with id: {id} doesn't exist in the database.")
{ }
