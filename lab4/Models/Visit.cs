
namespace lab4.Models
{
    public class Visit
    {
        public int VisitId { get; set; }
        public string VisitorName { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal TicketPrice { get; set; }

        public int ExhibitionId { get; set; }
        public Exhibition Exhibition { get; set; }
    }
}
