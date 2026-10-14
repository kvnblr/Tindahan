namespace Api.Entities.ExceptionModel;

public sealed class SortByArgumentException()
    : BadRequestException("Sort must be name, price, or id")
{ }
