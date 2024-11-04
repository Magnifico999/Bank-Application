using BankApplication.Data.Enums;
using BankApplication.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    FirstName = "YouBank",
                    LastName = "settlement Account",
                    AccountType = (AccountType)3,
                    PhoneNumber = "08035064624",
                    Email = "settlement@youbank.com",
                    AccountNumberGenerated = "9053769810",
                    CurrentAccountBalance = "999999999",
                    DateCreated = DateTime.Now,
                    DateLastUpdated = DateTime.Now,
                });
        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
