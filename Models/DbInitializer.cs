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
            

            //Datos de prueba para Entidad Transaction
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

            //Data de prueba para Entidad Loan
            if (!context.Loans.Any()) 
            {
                var loans = new Loan[]
                {
                    //Se crean 3 prestamos: Hipotecario, Personal y Automotriz.
                    new Loan
                    {
                        Name = "Hipotecario",
                        MaxAmount = 500000,
                        Payments = "12,24,36,48,60"
                    },

                    new Loan
                    {
                        Name = "Personal",
                        MaxAmount = 100000,
                        Payments = "6,12,24"
                    },

                    new Loan
                    {
                        Name = "Automotriz",
                        MaxAmount = 300000,
                        Payments = "6,12,24,36"
                    },
                };

                foreach (Loan loan in loans) 
                {
                    context.Loans.Add(loan);
                }

                context.SaveChanges();
            }

            //Data de prueba para entidad ClientLoan
            //A los clientes cargados hasta el momento (2) se les agrega un prestamo de cada tipo.
            if (!context.ClientLoans.Any())
            {
                var clientMartin = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                var loanH = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                var loanP = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                var loanA = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");

                if (clientMartin != null && loanH != null && loanP != null && loanA != null)
                {
                    var clientLoanH = new ClientLoan
                    {
                        Amount = 400000,
                        Payments = "60",
                        ClientId = clientMartin.Id,
                        LoanId = loanH.Id
                    };

                    var clientLoanP = new ClientLoan
                    {
                        Amount = 50000,
                        Payments = "12",
                        ClientId = clientMartin.Id,
                        LoanId = loanP.Id
                    };

                    var clientLoanA = new ClientLoan
                    {
                        Amount = 100000,
                        Payments = "24",
                        ClientId = clientMartin.Id,
                        LoanId = loanA.Id
                    };

                    context.ClientLoans.Add(clientLoanH);
                    context.ClientLoans.Add(clientLoanP);
                    context.ClientLoans.Add(clientLoanA);

                }

                var clientVictor = context.Clients.FirstOrDefault(c => c.Email == "vcoronado@gmail.com");
                var loanH2 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                var loanP2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                var loanA2= context.Loans.FirstOrDefault(l => l.Name == "Automotriz");

                if (clientVictor != null && loanH2 != null && loanP2 != null && loanA2 != null)
                {
                    var clientLoanH2 = new ClientLoan
                    {
                        Amount = 350000,
                        Payments = "36",
                        ClientId = clientVictor.Id,
                        LoanId = loanH2.Id
                    };

                    var clientLoanP2 = new ClientLoan
                    {
                        Amount = 87000,
                        Payments = "24",
                        ClientId = clientVictor.Id,
                        LoanId = loanP2.Id
                    };

                    var clientLoanA2 = new ClientLoan
                    {
                        Amount = 10000,
                        Payments = "6",
                        ClientId = clientVictor.Id,
                        LoanId = loanA2.Id
                    };

                    context.ClientLoans.Add(clientLoanH2);
                    context.ClientLoans.Add(clientLoanP2);
                    context.ClientLoans.Add(clientLoanA2);
                }
                context.SaveChanges();
            }

            //Datos de prueba para entidad Card:
            if (!context.Cards.Any())
            {
                var clientMartin = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                if (clientMartin != null)
                {
                    var cards = new Card[]
                    {
                        new Card
                        {
                            CardHolder = clientMartin.FirstName + " " + clientMartin.LastName,
                            Type = CardType.DEBIT.ToString(),
                            Color = CardColor.GOLD.ToString(),
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(4),
                            ClientId = clientMartin.Id
                        },

                        new Card
                        {
                            CardHolder = clientMartin.FirstName + " " + clientMartin.LastName,
                            Type = CardType.CREDIT.ToString(),
                            Color = CardColor.TITANIUM.ToString(),
                            Number = "2234-6745-552-7888",
                            Cvv = 750,
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(5),
                            ClientId = clientMartin.Id
                        },

                    };
                    foreach (Card card in cards)
                    {
                        context.Cards.Add(card);
                    }
                    context.SaveChanges();
                }
            }

        }
    }
}
