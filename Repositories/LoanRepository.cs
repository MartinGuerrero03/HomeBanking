using HomeBanking.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll().ToList();
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id).FirstOrDefault();
        }
    }
}
