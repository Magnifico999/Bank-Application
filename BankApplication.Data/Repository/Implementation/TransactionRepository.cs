using BankApplication.Data.Context;
using BankApplication.Data.DTO;
using BankApplication.Data.Enums;
using BankApplication.Data.Models;
using BankApplication.Data.Repository.Interface;
using BankApplication.Data.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Data.Repository.Implementation
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _db;
        ILogger<TransactionRepository> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementAccount;
        private readonly IAccountRepository _repo;
        public TransactionRepository(ApplicationDbContext db, ILogger<TransactionRepository> logger, IOptions<AppSettings> settings, IAccountRepository repo)
        {
            _db = db;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementAccount = _settings.OurBankSettlementAccount;
            _repo = repo;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            Response response = new Response();
            _db.Transactions.Add(transaction);
            _db.SaveChanges();
            response.ResponseCode = "00";
            response.ResponseMessage = "Transaction created successfully!";
            response.Data = null;

            return response;

        }



        public Response FindTransactionByDate(DateTime date)
        {
            Response response = new Response();
            var transactions = _db.Transactions.Where(x => x.TransactionDate == date).ToList();

            if (transactions.Count == 0)
            {
                response.ResponseCode = "400";  // Set the appropriate error code for not found
                response.ResponseMessage = "No transactions found for the specified date.";
            }
            else
            {
                response.ResponseCode = "00";
                response.ResponseMessage = "Transactions found successfully!";
                response.Data = transactions;
            }

            return response;
        }
        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // First check that the user account is valid then authenticate it by injecting IAccountRepository
            var authUser = _repo.Authenticate(FromAccount, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            // Validating it now
            try
            {
                // For a funds transfer, our bankSettlementAccount is the destination getting the money from the user's account
                sourceAccount = _repo.GetByAccountNumber(FromAccount);
                destinationAccount = _repo.GetByAccountNumber(ToAccount);

                // Parse the current balances as decimals with default values of 0 if null or empty
                decimal sourceBalance = decimal.Parse(sourceAccount.CurrentAccountBalance ?? "0");
                decimal destinationBalance = decimal.Parse(destinationAccount.CurrentAccountBalance ?? "0");

                // Check if the transfer amount is greater than the balance in the source account
                if (Amount > sourceBalance)
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "05"; // Use a specific code for insufficient balance for transfer
                    response.ResponseMessage = "Insufficient balance for transfer";
                    response.Data = null;
                }
                else
                {
                    // Subtract the Amount from the sourceBalance
                    sourceBalance -= Amount;

                    // Add the Amount to the destinationBalance
                    destinationBalance += Amount;

                    // Update the sourceAccount's balance as a string
                    sourceAccount.CurrentAccountBalance = sourceBalance.ToString();

                    // Update the destinationAccount's balance as a string
                    destinationAccount.CurrentAccountBalance = destinationBalance.ToString();

                    // Check if there is an update
                    if ((_db.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified &&
                         _db.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                    {
                        transaction.TransactionStatus = TranStatus.Success;
                        response.ResponseCode = "00";
                        response.ResponseMessage = "Transaction successful!";
                        response.Data = null;
                    }
                    else
                    {
                        transaction.TransactionStatus = TranStatus.Failed;
                        response.ResponseCode = "02";
                        response.ResponseMessage = "Transaction failed!";
                        response.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            try
            {
                var authUser = _repo.Authenticate(AccountNumber, TransactionPin);
                if (authUser == null) throw new ApplicationException("Invalid credentials");

                sourceAccount = _repo.GetByAccountNumber(_ourBankSettlementAccount);
                destinationAccount = _repo.GetByAccountNumber(AccountNumber);

                decimal currentBalance = decimal.Parse(sourceAccount.CurrentAccountBalance ?? "0");
                decimal destinationBalance = decimal.Parse(destinationAccount.CurrentAccountBalance ?? "0");

      
                if (Amount <= 0)
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "04"; // Use a specific code for invalid deposit amount
                    response.ResponseMessage = "Invalid deposit amount";
                    response.Data = null;
                }
                else if (Amount > currentBalance)
                {
                    transaction.TransactionStatus = TranStatus.Failed;
                    response.ResponseCode = "05"; // Use a specific code for insufficient balance for deposit
                    response.ResponseMessage = "Insufficient balance for deposit";
                    response.Data = null;
                }
                else
                {
                    // Update sourceAccount's balance
                    currentBalance -= Amount;
                    sourceAccount.CurrentAccountBalance = currentBalance.ToString();

                    // Update destinationAccount's balance by adding the Amount
                    destinationBalance += Amount;
                    destinationAccount.CurrentAccountBalance = destinationBalance.ToString();

                    // Check if there is an update
                    if ((_db.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified &&
                        _db.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                    {
                        transaction.TransactionStatus = TranStatus.Success;
                        response.ResponseCode = "00";
                        response.ResponseMessage = "Transaction successful!";
                        response.Data = null;
                    }
                    else
                    {
                        transaction.TransactionStatus = TranStatus.Failed;
                        response.ResponseCode = "02";
                        response.ResponseMessage = "Transaction failed!";
                        response.Data = null;
                    }
                }
            }
            catch (ApplicationException ex)
            {
                transaction.TransactionStatus = TranStatus.Failed;
                response.ResponseCode = "03"; // Use a specific code for invalid PIN or Account Number
                response.ResponseMessage = "Invalid username or pin";
                response.Data = null;
                _logger.LogError($"Invalid username or pin: {ex.Message}");
            }

            decimal transactionAmount = Amount;

            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementAccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {transactionAmount} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return response;
        }


        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Response response = new Response();
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // First check that the user account is valid then authenticate it by injecting IAccountRepository
            var authUser = _repo.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid credentials");

            // Validating it now
            try
            {
                // For a withdrawal, our bankSettlementAccount is the destination getting the money from the user's account
                sourceAccount = _repo.GetByAccountNumber(AccountNumber);
                destinationAccount = _repo.GetByAccountNumber(_ourBankSettlementAccount);

                if (decimal.TryParse(sourceAccount.CurrentAccountBalance, out decimal currentBalance))
                {
                    // Check if the withdrawal amount is greater than the balance
                    if (Amount > currentBalance)
                    {
                        transaction.TransactionStatus = TranStatus.Failed;
                        response.ResponseCode = "01"; // Use a specific code for insufficient balance
                        response.ResponseMessage = "Insufficient balance";
                        response.Data = null;
                        
                    }
                    else
                    {
                        currentBalance -= Amount;
                       
                        sourceAccount.CurrentAccountBalance = currentBalance.ToString();
                        
                        if (decimal.TryParse(destinationAccount.CurrentAccountBalance, out decimal destinationBalance))
                        {
                            destinationBalance += Amount;

                            destinationAccount.CurrentAccountBalance = destinationBalance.ToString();

                            transaction.TransactionStatus = TranStatus.Success;
                            response.ResponseCode = "00";
                            response.ResponseMessage = "Transaction successful!";
                            response.Data = null;
                        }
                        else
                        {
                            transaction.TransactionStatus = TranStatus.Failed;
                            response.ResponseCode = "02";
                            response.ResponseMessage = "Transaction failed!";
                            response.Data = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
            }

            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {transaction.TransactionType} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return response;
        }

        public Response GetAll()
        {
            Response response = new Response();

            try
            {
                var transactions = _db.Transactions.ToList();
                if (transactions.Count == 0)
                {
                    response.ResponseCode = "404"; 
                    response.ResponseMessage = "No transactions found.";
                    response.Data = null;
                }
                else
                {
                    response.ResponseCode = "00"; 
                    response.ResponseMessage = "Transactions retrieved successfully!";
                    response.Data = transactions;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURRED... => {ex.Message}");
                response.ResponseCode = "500"; 
                response.ResponseMessage = "An error occurred while retrieving transactions.";
                response.Data = null;
            }

            return response;
        }
    }
}
