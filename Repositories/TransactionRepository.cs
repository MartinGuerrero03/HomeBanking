using HomeBanking.Models;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository (HomeBankingContext repositoryContext) : base (repositoryContext) { }

        public Transaction FindByNumber (long id)
        {
            return FindByCondition(ts => ts.Id == id).FirstOrDefault();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }

        public IEnumerable<Transaction> FindAllTransactions() 
        {
            return FindAll().ToList();
        }
    }
}
