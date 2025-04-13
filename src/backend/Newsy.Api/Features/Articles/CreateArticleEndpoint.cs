﻿using FastEndpoints;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Hybrid;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Features.Articles;

public class CreateArticleEndpoint : EndpointWithMapper<CreateArticleRequest, ArticleRequestMapper>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public CreateArticleEndpoint(IUnitOfWork unitOfWork, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public override void Configure()
    {
        Post("/api/article");
    }

    public override async Task HandleAsync(CreateArticleRequest req, CancellationToken ct)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var article = Map.ToEntity(req, userId);

        var createdArticle = await _unitOfWork.ArticleRepository.CreateAsync(article);

        if (createdArticle == null)
        {
            AddError("Failed to save to database");
            return;
        }

        await _cache.RemoveByTagAsync(["articles"], ct);

        await SendCreatedAtAsync("GetArticleEndpoint", new { id = createdArticle.Id },
            new EmptyResponse(), true, ct);
    }
}