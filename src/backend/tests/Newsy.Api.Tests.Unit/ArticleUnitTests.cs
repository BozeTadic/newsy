using FastEndpoints;
using Moq;
using Newsy.Api.Features.Articles;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Tests.Unit;

public class ArticleUnitTests
{
    [Fact]
    public async Task Anonymous_Cannot_Create_Article()
    {
        //Arrange
        var mockOfWork = new Mock<IUnitOfWork>();
        var request = new CreateArticleRequest("Clickbait title", "No actual content");
        var endpoint = Factory.Create<CreateArticleEndpoint>(mockOfWork.Object);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);
        var response = endpoint.HttpContext.Response;

        //Assert
        Assert.NotNull(response);
        Assert.Equal(403, response.StatusCode);
    }
}