using FastEndpoints;
using FluentValidation;

namespace Newsy.Api.Features.Article;

public class ArticleValidator : Validator<CreateArticleRequest>
{
    public ArticleValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("title is required")
            .MaximumLength(128)
            .WithMessage("title too long");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("what is the point")
            .MinimumLength(16)
            .WithMessage("content too short")
            .MaximumLength(4096)
            .WithMessage("enough for this on");
    }
}