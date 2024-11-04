using BankApplication.Data.Context;
using BankApplication.Data.Models;
using BankApplication.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Data.Repository.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            var account = _db.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null || !VerifyPinHash(Pin, account.PinHash, account.PinSalt))
            {
                return null;
            }

            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");

            using(var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            if (_db.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An account already exists with this email");

            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins do not match", "Pin");

            //Now that all validation passes, let us create account
            //we re hashing/encrypting pin first 
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);
            
            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            _db.Accounts.Add(account);
            _db.SaveChanges();
            return account;
            
        }
        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public bool Delete(int Id)
        {
            
                var account = _db.Accounts.Find(Id);
                if (account != null)
                {
                    _db.Remove(account);
                _db.SaveChanges();
                return true;
                }
            
            return false;

        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _db.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _db.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null)
            {
                throw new ArgumentException("Account number is not correct.");
            }

            return account;
        }

        public Account GetById(int Id)
        {
            var account = _db.Accounts.Where(x => x.Id == Id).FirstOrDefault();
            if (account == null) return null;

            return account;
        }
        public void Update(Account account, string Pin = null)
        {
            var accountToBeUpdated = _db.Accounts.Find(account.Id);
            if (accountToBeUpdated == null)
            {
                throw new ApplicationException("Account does not exist");
            }


            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                if (_db.Accounts.Any(x => x.Email == account.Email && x.Id != account.Id))
                {
                    throw new ApplicationException("This Email " + account.Email + " already exists");
                }

                // Update email for the person
                accountToBeUpdated.Email = account.Email;
            }

            // Check if you want to update the PhoneNumber
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                // Check if the phone number you are trying to change to is already taken
                if (_db.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber && x.Id != account.Id))
                {
                    throw new ApplicationException("This Phone number " + account.PhoneNumber + " already exists");
                }

                // Update phone number for the person
                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }

            // Check if you want to update the Pin
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                // Update the Pin
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);
                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }

            accountToBeUpdated.DateLastUpdated = DateTime.UtcNow;

            _db.Accounts.Update(accountToBeUpdated);
            _db.SaveChanges();
        }

    }
}
