namespace Dtos.DTO
{
    public class VisitDto
    {
        public int VisitId { get; set; }
        public int ExhibitionId { get; set; }
        public string VisitorName { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal TicketPrice { get; set; }
    }
}