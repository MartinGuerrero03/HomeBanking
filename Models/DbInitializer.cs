using System.Linq;

namespace HomeBanking.Models
{
    public class DbInitializer
    {
        public static void DbInitialize(HomeBankingContext context) 
        {
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

                //guardamos
                context.SaveChanges();
            }
        
        }
    }
}
