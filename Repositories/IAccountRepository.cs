using HomeBanking.Models;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public interface IAccountRepository
    {
        Account FindById(long id);
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
    }
}
