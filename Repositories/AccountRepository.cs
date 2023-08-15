using HomeBanking.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Account FindById(long id)
        {
            return FindByCondition(ac => ac.Id == id).Include(ac => ac.Transactions).FirstOrDefault();
        }
        public Account FindByNumber(string number) 
        {
            return FindByCondition(ac => ac.Number.ToUpper() == number.ToUpper()).Include(ac => ac.Transactions).FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll().Include(ac => ac.Transactions).ToList();
        }

        public void Save(Account account)
        {
            if (account.Id == 0)
            {
                Create(account);
            }
            else
            {
                Update(account);
            }

            SaveChanges();
        }

        //public IEnumerable<Account> GetByClient(long clientId) 
        //{
        //    return FindByCondition(ac => ac.ClientId == clientId)
        //        .Include(ac => ac.Transactions).ToList();
        //}
    }
}
