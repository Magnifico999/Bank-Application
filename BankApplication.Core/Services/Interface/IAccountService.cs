using BankApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Core.Services.Interface
{
    public interface IAccountService
    {
        Account Authenticate(string AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        void Update(Account account, string Pin = null);
        //Task<T> UpdateAsync<T>(UpdateAccountModel dto, int id);
        bool Delete(int id);
        Account GetById(int Id);
        Account GetByAccountNumber(string AccountNumber);
    }
}
