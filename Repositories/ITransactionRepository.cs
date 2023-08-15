using HomeBanking.Models;
using System.Collections;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
        Transaction FindByNumber(long id);
        IEnumerable<Transaction> FindAllTransactions();
    }
}
