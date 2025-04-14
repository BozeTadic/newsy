using Newsy.Api.Features.Auth.Login;
using System.Net.Http.Json;
using Newsy.Api.Features.Auth.Register;

namespace Newsy.Api.Tests.Integration.Utilities;

public static class AuthExtensions
{
    public static async Task<string?> RegisterAndLoginUserAsync(this HttpClient client)
    {
        var username = $"{Guid.NewGuid():N}";
        var password = "Strong_Password123.";

        var registerRequest = new RegisterRequest(username, password);
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest(username, password);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        return loginResult?.Token;
    }
}