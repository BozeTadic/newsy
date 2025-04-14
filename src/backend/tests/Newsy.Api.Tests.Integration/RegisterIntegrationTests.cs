using System.Net.Http.Json;
using System.Net;
using Newsy.Api.Tests.Integration.Utilities;
using Newsy.Api.Features.Auth.Register;

namespace Newsy.Api.Tests.Integration;

public class RegisterIntegrationTests : IClassFixture<TestApplicationFactory>
{
    private readonly HttpClient _client;

    public RegisterIntegrationTests(TestApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_Fail_If_Validation_Fails()
    {
        var request = new RegisterRequest("New_User", "weak");

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}