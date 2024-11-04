using System;
using System.Collections.Generic;
using AutoMapper;
using BankApplication.API.Controllers;
using BankApplication.Core.Services.Interface;
using BankApplication.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BankApplication.Tests.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public void RegisterNewAccount_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            var newAccount = new RegisterNewAccountModel
            {
                // Provide valid data for newAccount
            };

            // Act
            var result = controller.RegisterNewAccount(newAccount);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void RegisterNewAccount_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            controller.ModelState.AddModelError("Key", "Error Message"); // Simulate ModelState error
            var newAccount = new RegisterNewAccountModel
            {
                // Provide invalid data for newAccount
            };

            // Act
            var result = controller.RegisterNewAccount(newAccount);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Authenticate_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            var authenticateModel = new AuthenticateModel
            {
                // Provide valid data for authentication
            };

            // Act
            var result = controller.Authenticate(authenticateModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void Authenticate_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            controller.ModelState.AddModelError("Key", "Error Message"); // Simulate ModelState error
            var authenticateModel = new AuthenticateModel
            {
                // Provide invalid data for authentication
            };

            // Act
            var result = controller.Authenticate(authenticateModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetAllAccounts_ReturnsOkResult()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);

            // Act
            var result = controller.GetAllAccounts();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetByAccountNumber_ValidAccountNumber_ReturnsOkResult()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            var validAccountNumber = "1234567890"; // Provide a valid account number

            // Act
            var result = controller.GetByAccountNumber(validAccountNumber);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetByAccountNumber_InvalidAccountNumber_ReturnsBadRequest()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            var invalidAccountNumber = "123"; // Provide an invalid account number

            // Act
            var result = controller.GetByAccountNumber(invalidAccountNumber);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetAccountById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);

            int existingId = 1; // Provide an existing ID

            var dummyAccount = new Account
            {
                // Populate dummy account properties as needed
            };
            mockAccountService.Setup(service => service.GetById(existingId)).Returns(dummyAccount);

            // Act
            var result = controller.GetAccountById(existingId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void GetAccountById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var mockAccountService = new Mock<IAccountService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
            var nonExistingId = 999; // Provide a non-existing ID

            // Act
            var result = controller.GetAccountById(nonExistingId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        //[Fact]
        //public void UpdateAccount_ValidModel_ReturnsOkResult()
        //{
        //    // Arrange
        //    var mockAccountService = new Mock<IAccountService>();
        //    var mockMapper = new Mock<IMapper>();
        //    var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
        //    var updateAccountModel = new UpdateAccountModel
        //    {
        //        // Provide valid data for updating an account
        //    };

        //    // Act
        //    var result = controller.UpdateAccount(updateAccountModel);

        //    // Assert
        //    Assert.IsType<OkResult>(result);
        //}

        //[Fact]
        //public void UpdateAccount_InvalidModel_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var mockAccountService = new Mock<IAccountService>();
        //    var mockMapper = new Mock<IMapper>();
        //    var controller = new AccountController(mockAccountService.Object, mockMapper.Object);
        //    controller.ModelState.AddModelError("Key", "Error Message"); // Simulate ModelState error
        //    var updateAccountModel = new UpdateAccountModel
        //    {
        //        // Provide invalid data for updating an account
        //    };

        //    // Act
        //    var result = controller.UpdateAccount(updateAccountModel);

        //    // Assert
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}
    }
}
