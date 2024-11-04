using BankApplication.Core.Services.Interface;
using BankApplication.Data.Models;
using BankApplication.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;
using BankApplication.Data.DTO;

namespace BankApplication.Tests.Controllers
{
    public class TransactionControllerTests
    {
        [Fact]
        public void MakeDeposit_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.MakeDeposit(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                       .Returns(new Response { ResponseCode = "00" }); // Simulate a successful deposit

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeDeposit("1234567890", 100.0m, "1234");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MakeDeposit_InvalidAccountNumber_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeDeposit("invalid_account", 100.0m, "1234");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeDeposit_AmountTooLarge_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeDeposit("1234567890", decimal.MaxValue, "1234");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeDeposit_InvalidUsernameOrPin_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.MakeDeposit(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                       .Returns(new Response { ResponseCode = "03" }); // Simulate invalid username or pin

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeDeposit("1234567890", 100.0m, "1234");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeDeposit_InsufficientBalance_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.MakeDeposit(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                       .Returns(new Response { ResponseCode = "05" }); // Simulate insufficient balance

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeDeposit("1234567890", 100.0m, "1234");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeWithdrawal_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.MakeWithdrawal(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                       .Returns(new Response { ResponseCode = "00" }); // Simulate a successful withdrawal

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeWithdrawal("1234567890", 50.0m, "1234");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void MakeWithdrawal_InvalidAccountNumber_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeWithdrawal("invalid_account", 50.0m, "1234");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void MakeWithdrawal_InsufficientBalance_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.MakeWithdrawal(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
                       .Returns(new Response { ResponseCode = "01" }); // Simulate insufficient balance

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.MakeWithdrawal("1234567890", 100.0m, "1234");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void FindTransactionByDate_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.FindTransactionByDate(It.IsAny<DateTime>()))
                       .Returns(new Response { ResponseCode = "00" }); // Simulate successful transaction retrieval

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.FindTransactionByDate(DateTime.Now);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void FindTransactionByDate_NoTransactionsFound_ReturnsBadRequest()
        {
            // Arrange
            var mockService = new Mock<ITransactionService>();
            mockService.Setup(service => service.FindTransactionByDate(It.IsAny<DateTime>()))
                       .Returns(new Response { ResponseCode = "01" }); // Simulate no transactions found

            var controller = new TransactionController(mockService.Object, null);

            // Act
            var result = controller.FindTransactionByDate(DateTime.Now);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
