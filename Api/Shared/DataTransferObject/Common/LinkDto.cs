namespace Api.Shared.DataTransferObject.Common;

public sealed record LinkDto
{
    public string Href { get; init; } = string.Empty;
    public string Rel { get; init; } = string.Empty;
    public string Method { get; init; } = HttpMethods.Get;
}
