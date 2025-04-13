using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Moq;
using Newsy.Api.Domain;
using Newsy.Api.Features.Auth.Login;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Tests.Unit;

public class LoginUnitTests
{
    [Fact]
    public async Task Wrong_User_Pass_Combination_Cannot_Login()
    {
        //Arrange
        var mockOfWork = new Mock<IUnitOfWork>();
        var mockConfiguration = new Mock<IConfiguration>();
        var request = new LoginRequest("Random_User_123", "Random_Password.");

        mockOfWork.Setup(unitOfWork => unitOfWork.AuthorRepository.GetAsync(request.Username)).ReturnsAsync((Author?)null);

        var endpoint = Factory.Create<LoginEndpoint>(mockOfWork.Object, mockConfiguration.Object);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);
        var response = endpoint.HttpContext.Response;

        //Assert
        Assert.NotNull(response);
        Assert.Equal(403, response.StatusCode);
    }
}