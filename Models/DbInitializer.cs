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

                    foreach(Account account in accounts)
                    {
                        context.Accounts.Add(account);
                    }
                    context.SaveChanges();
                }
            }
        
        }
    }
}
