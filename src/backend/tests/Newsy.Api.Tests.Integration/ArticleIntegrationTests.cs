using Newsy.Api.Features.Articles;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Newsy.Api.Tests.Integration.Utilities;

namespace Newsy.Api.Tests.Integration;

public class ArticleIntegrationTests : IClassFixture<TestApplicationFactory>
{
    private readonly HttpClient _client;

    public ArticleIntegrationTests(TestApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Valid_Request_ShouldReturn_Created()
    {
        var jwt = await _client.RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var createArticleRequest = new CreateArticleRequest("Clickbait title", "No actual content");
        var response = await _client.PostAsJsonAsync("/api/article", createArticleRequest);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Anonymous_Cannot_Create_Article()
    {
        var createArticleRequest = new CreateArticleRequest("Clickbait title", "No actual content");

        var response = await _client.PostAsJsonAsync("/api/article", createArticleRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Validation_Should_Fail_On_Empty_Content()
    {
        var jwt = await _client.RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var createArticleRequest = new CreateArticleRequest("Clickbait title", "");
        var response = await _client.PostAsJsonAsync("/api/article", createArticleRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Anonymous_Can_Get_Articles()
    {
        var jwt = await _client.RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var createArticleRequest = new CreateArticleRequest("Clickbait title", "No actual content");
        var createArticleResponse = await _client.PostAsJsonAsync("/api/article", createArticleRequest);
        Assert.Equal(HttpStatusCode.Created, createArticleResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/articles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}