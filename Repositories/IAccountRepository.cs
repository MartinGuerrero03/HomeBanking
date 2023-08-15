using HomeBanking.Models;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public interface IAccountRepository
    {
        Account FindById(long id);
        Account FindByNumber(string number);
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        //IEnumerable<Account> GetByClient(long clientId);
    }
}
