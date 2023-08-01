using System;
using System.Linq;
using System.Security.Principal;

namespace HomeBanking.Models
{
    public class DbInitializer
    {
        public static void DbInitialize(HomeBankingContext context) 
        {
            //Datos de prueba para Entidad Client 
            if (!context.Clients.Any())
            {
                var clients = new Client[] {
                new Client
                {
                    Email = "martin@gmail.com",
                    FirstName = "Martin",
                    LastName = "Guerrero",
                    Password = "12345"
                }};

                foreach (Client client in clients)
                {
                    context.Clients.Add(client);
                }

                //guardar
                context.SaveChanges();
            }
            

            //Datos de prueba para Entidad Account
            if (!context.Accounts.Any())
            {
                var clientMartin = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                if (clientMartin != null)
                {
                    var accounts = new Account[]
                    {
                        new Account
                        {
                            ClientId = clientMartin.Id,
                            CreationDate = DateTime.Now,
                            Number = string.Empty,
                            Balance = 0
                        }
                    };

                    foreach (Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }
            }
            

            //Datos de prueba para Entidad Transactions
            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");

                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction
                        {
                            AccountId= account1.Id,
                            Amount = 10000,
                            Date= DateTime.Now.AddHours(-5),
                            Description = "Transferencia reccibida",
                            Type = TransactionType.CREDIT.ToString()
                        },

                        new Transaction
                        {
                            AccountId = account1.Id,
                            Amount = -2000,
                            Date = DateTime.Now.AddHours(-6),
                            Description = "Compra en tienda mercado libre",
                            Type = TransactionType.DEBIT.ToString()
                        },

                        new Transaction
                        {
                            AccountId= account1.Id,
                            Amount = -3000,
                            Date= DateTime.Now.AddHours(-7),
                            Description = "Compra en tienda xxxx",
                            Type = TransactionType.DEBIT.ToString()
                        },
                    };
                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }

                    context.SaveChanges();
                }
            }

        }
    }
}
