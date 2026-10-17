using Api.Shared.DataTransferObject;
using Api.Shared.DataTransferObject.Common;

namespace Api.Services.Contracts;

public interface ILinkService
{
    // IEnumerable<LinkDto> TryGenerateLink(HttpContext httpContext, RequestParametersDto request, PagedResult<ProductDto> product);
    // IEnumerable<LinkDto> CreateProductLinks(HttpContext httpContext, int productId);

    IReadOnlyList<LinkDto> CreatePaginationLinks(HttpContext httpContext, RequestParametersDto request, int totalPages);
    IReadOnlyList<LinkDto> CreateProductLinks(HttpContext httpContext, int productId);
}
