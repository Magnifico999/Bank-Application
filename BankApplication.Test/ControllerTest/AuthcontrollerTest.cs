using BankApplication.API.Controllers;
using BankApplication.Core.Services.Interface;
using BankApplication.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class AuthControllerTests
{
    [Fact]
    public async Task RegisterUser_ValidModel_ReturnsOkResult()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.RegisterUser(It.IsAny<RegisterUser>()))
            .ReturnsAsync(new ResponseDto<bool> { Result = true });

        var controller = new AuthController(authServiceMock.Object);

        // Act
        var user = new RegisterUser { /* Initialize with valid data */ };
        var result = await controller.RegisterUser(user);

        // Assert
        Assert.NotNull(result);
    }
    [Fact]
    public async Task RegisterUser_InvalidModel_ReturnsBadRequestResult()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.RegisterUser(It.IsAny<RegisterUser>()))
            .ReturnsAsync(new ResponseDto<bool> { Result = false, DisplayMessage = "Invalid input", StatusCode = 400 });

        var controller = new AuthController(authServiceMock.Object);

        // Create a RegisterUser object with invalid data
        var invalidUser = new RegisterUser
        {
        };

        // Act
        var result = await controller.RegisterUser(invalidUser);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = Assert.IsType<ResponseDto<string>>(badRequestResult.Value);
        Assert.Equal("Invalid input", errorResponse.DisplayMessage);
        Assert.Equal(400, errorResponse.StatusCode);
    }





    [Fact]
    public async Task Login_ValidModel_ReturnsOkResultWithToken()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.Login(It.IsAny<LoginUser>()))
            .ReturnsAsync(new ResponseDto<bool> { Result = true });
        authServiceMock.Setup(service => service.GenerateTokenString(It.IsAny<LoginUser>()))
            .Returns("fakeTokenString"); 

        var controller = new AuthController(authServiceMock.Object);

        // Act
        var user = new LoginUser {
            Email = "johndoe@example.com",
            Password = "Password123",
        };
        var result = await controller.Login(user) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("fakeTokenString", result.Value); // Ensure the token is returned
    }

    [Fact]
    public async Task Login_InvalidModel_ReturnsBadRequestResult()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthService>();
        var controller = new AuthController(authServiceMock.Object);
        controller.ModelState.AddModelError("PropertyName", "Some error message");

        // Act
        var user = new LoginUser {
           
            Email = "johndoe@example.com",
            Password = "Password123",
        };
        var result = await controller.Login(user) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        var responseDto = result.Value as ResponseDto<bool>;
        Assert.NotNull(responseDto);
        Assert.False(responseDto.Result);
        Assert.Equal("Invalid input", responseDto.DisplayMessage);
    }
}
