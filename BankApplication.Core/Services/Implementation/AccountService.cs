using BankApplication.Core.Services.Interface;
using BankApplication.Data.Models;
using BankApplication.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Core.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            return _repo.Authenticate(AccountNumber, Pin);
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            return _repo.Create(account, Pin, ConfirmPin);
        }

        public bool Delete(int Id)
        {
            return _repo.Delete(Id);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _repo.GetAllAccounts();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            return _repo.GetByAccountNumber(AccountNumber);
        }

        public Account GetById(int Id)
        {
            return _repo.GetById(Id);
        }

        //public Task<T> UpdateAsync<T>(UpdateAccountModel dto, int id)
        //{
        //    return _repo.Update(dto, id);
        //}

        public void Update(Account account, string Pin = null)
        {
            _repo.Update(account, Pin);
        }
    }
}
