using AutoMapper;
using BankApplication.Core.Services.Implementation;
using BankApplication.Core.Services.Interface;
using BankApplication.Data.DTO;
using BankApplication.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BankApplication.API.Controllers
{
    [Route("api/TransactionAPI")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;
        IMapper _mapper;

        public TransactionController(ITransactionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("make_deposit")]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            try
            {
                if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                {
                    return BadRequest("Account Number must be 10-digit");
                }

                //Check if the deposit amount exceeds the maximum value for an int
                if (Amount > int.MaxValue)
                {
                    return BadRequest("Amount is too large. Please enter a smaller amount.");
                }

                Response response = _service.MakeDeposit(AccountNumber, Amount, TransactionPin);

                if (response.ResponseCode == "03")
                {
                    return BadRequest("Invalid username or pin");
                }
                else if (response.ResponseCode == "05")
                {
                    return BadRequest("Insufficient balance for deposit");
                }
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred");
            }


        }

        [HttpPost("make_withdrawal")]
        public IActionResult MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            try
            {
                if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                {
                    return BadRequest("Invalid Account number or pin");
                }

                Response response = _service.MakeWithdrawal(AccountNumber, Amount, TransactionPin);

                if (response.ResponseCode == "01")
                {
                    return BadRequest("Insufficient balance");
                }

                if (response.ResponseCode == "02")
                {
                    return BadRequest("Transaction failed");
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred");
            }
        }

        [HttpPost("make_funds_transfer")]
        public IActionResult MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            try
            {
                if (!Regex.IsMatch(FromAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
                {
                    return BadRequest("Account Number must be 10-digit");
                }

                // Check if the transfer amount exceeds the range for an int
                if (Amount > int.MaxValue)
                {
                    return BadRequest("Amount is too large, input a smaller amount");
                }

                Response response = _service.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);

                if (response.ResponseCode == "03")
                {
                    return BadRequest("Invalid Account number or pin");
                }

                if (response.ResponseCode == "05")
                {
                    return BadRequest("Insufficient balance for transfer");
                }

                return Ok(response);
            }
            catch (ApplicationException)
            {
                return BadRequest("Invalid Account number or pin");
            }
        }

        [HttpGet("find_by_date")]
        public IActionResult FindTransactionByDate(DateTime date)
        {

            var response = _service.FindTransactionByDate(date);

            if (response.ResponseCode == "00")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }

        }
        [HttpGet]
        public IActionResult GetAllTransactions()
        {

            var response = _service.GetAll();

            if (response.ResponseCode == "00")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
