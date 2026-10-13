namespace Api.Shared.DataTransferObject;

public record ProductDto : BaseProductDto
{
    public int Id { get; init; }
}
