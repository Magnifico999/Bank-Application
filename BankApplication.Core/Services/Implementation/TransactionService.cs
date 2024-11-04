using BankApplication.Core.Services.Interface;
using BankApplication.Data.DTO;
using BankApplication.Data.Models;
using BankApplication.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankApplication.Core.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            return _repo.CreateNewTransaction(transaction);
        }

        public Response FindTransactionByDate(DateTime date)
        {
            return _repo.FindTransactionByDate(date);
        }

        public Response GetAll()
        {
            return _repo.GetAll();
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            return _repo.MakeDeposit(AccountNumber, Amount, TransactionPin);
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            return _repo.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);
        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            return _repo.MakeWithdrawal(AccountNumber, Amount, TransactionPin);
        }
    }
}
