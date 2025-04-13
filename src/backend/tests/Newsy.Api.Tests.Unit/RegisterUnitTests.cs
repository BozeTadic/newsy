using FastEndpoints;
using Moq;
using Newsy.Api.Domain;
using Newsy.Api.Features.Auth.Register;
using Newsy.Api.Infrastructure.Persistence.UnitOfWork;

namespace Newsy.Api.Tests.Unit;

public class RegisterUnitTests
{
    [Fact]
    public async Task Anonymous_Can_Register()
    {
        //Arrange
        var mockOfWork = new Mock<IUnitOfWork>();
        var request = new RegisterRequest("New_User_123", "Strong_Password.");

        mockOfWork.Setup(unitOfWork => unitOfWork.AuthorRepository.GetAsync(request.Username)).ReturnsAsync((Author?)null);
        mockOfWork.Setup(unitOfWork => unitOfWork.AuthorRepository.CreateAsync(It.IsAny<Author>()))
            .ReturnsAsync(true);

        var endpoint = Factory.Create<RegisterEndpoint>(mockOfWork.Object);

        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);
        var response = endpoint.HttpContext.Response;

        //Assert
        Assert.NotNull(response);
        Assert.Equal(201, response.StatusCode);
    }

    [Fact]
    public async Task Username_Is_Unique_And_Cannot_Register_Again()
    {
        //Arrange
        var mockOfWork = new Mock<IUnitOfWork>();
        var request = new RegisterRequest("New_User_123", "Strong_Password.");

        mockOfWork.Setup(unitOfWork => unitOfWork.AuthorRepository.GetAsync(request.Username)).ReturnsAsync(new Author
        {
            Username = "New_User_123"
        });

        var endpoint = Factory.Create<RegisterEndpoint>(mockOfWork.Object);


        // Act
        await endpoint.HandleAsync(request, CancellationToken.None);
        var response = endpoint.HttpContext.Response;


        //Assert
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode);
    }
}