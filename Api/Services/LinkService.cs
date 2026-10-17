using System.Globalization;
using Api.Controllers;
using Api.Entities.ExceptionModel;
using Api.Services.Contracts;
using Api.Shared.DataTransferObject;
using Api.Shared.DataTransferObject.Common;
using Microsoft.AspNetCore.WebUtilities;

namespace Api.Services;

public sealed class LinkService(LinkGenerator linkGenerator) : ILinkService
{
    // public IEnumerable<LinkDto> TryGenerateLink(HttpContext httpContext, RequestParametersDto request, PagedResult<ProductDto> product)
    // {
    //     var links = new List<LinkDto>
    //     {
    //         CreateLink(httpContext, request, request.PageNumber, "self")
    //     };
    //
    //     if (request.PageNumber > 1)
    //     {
    //         links.Add(CreateLink(
    //             httpContext,
    //             request,
    //             1,
    //             "first"));
    //
    //         links.Add(CreateLink(
    //             httpContext,
    //             request,
    //             request.PageNumber - 1,
    //             "prev"));
    //     }
    //
    //     if (request.PageNumber < product.TotalPages)
    //     {
    //         links.Add(CreateLink(
    //             httpContext,
    //             request,
    //             product.TotalPages,
    //             "last"));
    //
    //         links.Add(CreateLink(
    //             httpContext,
    //             request,
    //             request.PageNumber + 1,
    //             "next"));
    //     }
    //
    //     return links;
    // }
    //
    // public IEnumerable<LinkDto> CreateProductLinks(HttpContext httpContext, int productId)
    // {
    //     var links = new List<LinkDto>
    //     {
    //         CreateActionLink(
    //             httpContext,
    //             nameof(ProductsController.GetProduct),
    //             new { id = productId },
    //             "self",
    //             HttpMethods.Get),
    //
    //         CreateActionLink(
    //             httpContext,
    //             nameof(ProductsController.GetAllProducts),
    //             values: null,
    //             "collection",
    //             HttpMethods.Get),
    //
    //         CreateActionLink(
    //             httpContext,
    //             nameof(ProductsController.UpdateProduct),
    //             new { id = productId },
    //             "update",
    //             HttpMethods.Put),
    //
    //         CreateActionLink(
    //             httpContext,
    //             nameof(ProductsController.DeleteProduct),
    //             new { id = productId },
    //             "delete",
    //             HttpMethods.Delete)
    //     };
    //
    //     return links;
    // }
    //
    // private LinkDto CreateLink(HttpContext httpContext, RequestParametersDto request, int pageNumber, string rel)
    // {
    //     var path = linkGenerator.GetPathByName(httpContext, nameof(ProductsController.GetAllProducts), values: null)
    //         ?? throw new InvalidPathBadRequest();
    //
    //     var queryParameters = new Dictionary<string, string?>
    //     {
    //         ["PageNumber"] = pageNumber.ToString(CultureInfo.InvariantCulture),
    //         ["PageSize"] = request.PageSize.ToString(CultureInfo.InvariantCulture),
    //         ["SearchTerm"] = request.SearchTerm,
    //         ["SortBy"] = request.SortBy,
    //         ["SortDescending"] = request.SortDescending.ToString().ToLowerInvariant(),
    //         ["MinimumPrice"] = request.MinimumPrice?.ToString(CultureInfo.InvariantCulture),
    //         ["MaxPrice"] = request.MaxPrice?.ToString(CultureInfo.InvariantCulture),
    //     };
    //
    //     var href = QueryHelpers.AddQueryString(path, queryParameters);
    //     return new LinkDto
    //     {
    //         Href = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{href}",
    //         Rel = rel,
    //         Method = HttpMethods.Get
    //     };
    // }
    //
    // private LinkDto CreateActionLink(HttpContext httpContext, string routeName, object? values, string rel, string method)
    // {
    //     var href = linkGenerator.GetUriByName(httpContext, routeName, values);
    //
    //     if (href is null)
    //         throw new InvalidPathBadRequest();
    //
    //     return new LinkDto
    //     {
    //         Href = href,
    //         Rel = rel,
    //         Method = method
    //     };
    // }

    public IReadOnlyList<LinkDto> CreatePaginationLinks(HttpContext httpContext, RequestParametersDto request, int totalPages)
    {
        var links = new List<LinkDto> { CreatePaginationLink(httpContext, request, request.PageNumber, "self") };

        if (request.PageNumber > 1)
        {
            links.Add(CreatePaginationLink(httpContext, request, 1, "first"));
            links.Add(CreatePaginationLink(httpContext, request, request.PageNumber - 1, "prev"));
        }

        if (request.PageNumber < totalPages)
        {
            links.Add(CreatePaginationLink(httpContext, request, totalPages, "last"));
            links.Add(CreatePaginationLink(httpContext, request, request.PageNumber + 1, "next"));
        }

        return links;
    }

    public IReadOnlyList<LinkDto> CreateProductLinks(HttpContext httpContext, int productId)
    {
        return
        [
            CreateActionLink(
                httpContext,
                nameof(ProductsController.GetProduct),
                new { id = productId },
                "self",
                HttpMethods.Get),

            CreateActionLink(
                httpContext,
                nameof(ProductsController.GetAllProducts),
                values: null,
                "collection",
                HttpMethods.Get),

            CreateActionLink(
                httpContext,
                nameof(ProductsController.UpdateProduct),
                new { id = productId },
                "update",
                HttpMethods.Put),

            CreateActionLink(
                httpContext,
                nameof(ProductsController.DeleteProduct),
                new { id = productId },
                "delete",
                HttpMethods.Delete)
        ];
    }

    private LinkDto CreatePaginationLink(HttpContext httpContext, RequestParametersDto request, int pageNumber, string rel)
    {
        var queryParameters = new Dictionary<string, string?>
        {
            ["PageNumber"] = pageNumber.ToString(CultureInfo.InvariantCulture),
            ["PageSize"] = request.PageSize.ToString(CultureInfo.InvariantCulture),
            ["SearchTerm"] = request.SearchTerm,
            ["SortBy"] = request.SortBy,
            ["SortDescending"] = request.SortDescending.ToString().ToLowerInvariant(),
            ["MinimumPrice"] = request.MinimumPrice?.ToString(CultureInfo.InvariantCulture),
            ["MaxPrice"] = request.MaxPrice?.ToString(CultureInfo.InvariantCulture),
        };
        return CreateActionLink(httpContext, nameof(ProductsController.GetAllProducts), queryParameters, rel, HttpMethods.Get);
    }

    private LinkDto CreateActionLink(HttpContext httpContext, string routeName, object? values, string rel, string method)
    {
        var href = linkGenerator.GetUriByName(httpContext, routeName, values)
            ?? throw new InvalidPathBadRequest();

        return new LinkDto
        {
            Href = href,
            Rel = rel,
            Method = method
        };
    }
}

