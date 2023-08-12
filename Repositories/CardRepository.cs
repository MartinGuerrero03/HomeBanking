using HomeBanking.Models;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Card FindByNumber(string number)
        {
            return FindByCondition(cd => cd.Number == number).FirstOrDefault();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }

    }
}
