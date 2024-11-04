using AutoMapper;
using BankApplication.Core.Services.Interface;
using BankApplication.Data.DTO;
using BankApplication.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace BankApplication.API.Controllers
{
    //[Authorize]
    [Route("api/AccountAPI")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;
        protected APIResponse _response;
        private readonly IMapper _mapper;

        public AccountController(IAccountService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("register_new_account")]
        public IActionResult RegisterNewAccount([FromBody] RegisterNewAccountModel newAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = _mapper.Map<Account>(newAccount);
            try
            {
                
                return Ok(_service.Create(account, newAccount.Pin, newAccount.ConfirmPin));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("An account already exists with this email"))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return BadRequest(ModelState);
                }
                return StatusCode(500, "An error occurred while creating the account");
            }
        }


        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            var accounts = _service.GetAllAccounts();
            var cleanAccounts = _mapper.Map<IList<GetAccountModel>>(accounts);
            return Ok(cleanAccounts);
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var account = _service.Authenticate(model.AccountNumber, model.Pin);

            if (account == null)
            {
                return BadRequest("One or more fields is/are incorrect. Please check again.");
            }

            return Ok(account);
        }

        [HttpGet("get_by_account_number")]
        public IActionResult GetByAccountNumber(string AccountNumber)
        {
            
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$"))
            {
                return BadRequest("Account number must be 10 digits.");
            }

            try
            {
                var account = _service.GetByAccountNumber(AccountNumber);
                var cleanAccount = _mapper.Map<GetAccountModel>(account);
                return Ok(cleanAccount);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{Id:int}", Name = "GetAccount")]
        public IActionResult GetAccountById(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var account = _service.GetById(Id);

                if (account == null)
                {
                    return NotFound($"Account with ID {Id} does not exist.");
                }

                var cleanAccount = _mapper.Map<GetAccountModel>(account);
                return Ok(cleanAccount);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while processing the request.");
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateAccount(int id, [FromBody] UpdateAccountModel modelDTO)
        {
            
            var _response = new APIResponse();

            if (modelDTO == null || id != modelDTO.Id)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            Account model = _mapper.Map<Account>(modelDTO);
            _service.Update(model);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;

            return Ok(_response);
        }




        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var deleted = _service.Delete(id);
                if (deleted)
                {
                    return NoContent(); 
                }
                else
                {
                    return NotFound(); 
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }

    }
}
