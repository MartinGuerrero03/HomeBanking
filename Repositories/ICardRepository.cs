using HomeBanking.Models;
using System.Collections;
using System.Collections.Generic;

namespace HomeBanking.Repositories
{
    public interface ICardRepository
    {
        public Card FindByNumber(string number);
        void Save(Card card);
        
    }
}
