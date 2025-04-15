using FastEndpoints;
using Newsy.Api.Common.Pagination;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Articles;

public class GetArticlesEndpoint : Endpoint<PaginationQuery, PaginationResult<ArticleResponse>, ArticleResponseMapper>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetArticlesEndpoint(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override void Configure()
    {
        Get("/api/articles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(PaginationQuery query, CancellationToken ct)
    {   
        var articles = await _unitOfWork.ArticleRepository.GetAllAsync(query.PageNumber, query.PageSize);

        if (articles.Count == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = new PaginationResult<ArticleResponse>(articles.Select(Map.FromEntity).ToList(),
            query.PageNumber,
            query.PageSize
        );

        await SendOkAsync(response, ct);
    }
}