using Api.Shared.DataTransferObject.Common;

namespace Api.Shared.DataTransferObject;

public sealed record ProductResourceDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }

    public IReadOnlyList<LinkDto> Links { get; init; } = [];
}
