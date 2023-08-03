namespace HomeBanking.Models.dtos
{
    public class ClientLoanDTO
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string Name { set; get; }
        public double Amount { get; set; }
        public int Payments { get; set; }
    }
}
