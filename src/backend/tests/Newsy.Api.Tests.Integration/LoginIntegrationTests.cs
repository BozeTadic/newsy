using System.Net.Http.Json;
using System.Net;
using Newsy.Api.Tests.Integration.Utilities;
using Newsy.Api.Features.Auth.Login;

namespace Newsy.Api.Tests.Integration;

public class LoginIntegrationTests : IClassFixture<TestApplicationFactory>
{
    private readonly HttpClient _client;

    public LoginIntegrationTests(TestApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Should_Fail_For_Non_Existing_User_Or_Invalid_Password()
    {
        var request = new LoginRequest("Malicious_User", "Brute_Force_12345678!?.");
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}