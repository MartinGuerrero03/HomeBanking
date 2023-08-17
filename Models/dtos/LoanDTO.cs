using System.Collections.Generic;

namespace HomeBanking.Models.dtos
{
    public class LoanDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double MaxAmount { get; set; }
        public string Payments { get; set; }
        //public ICollection<LoanApplicationDTO> LoansAppDTO { get; set; }
    }
}
