using System;

namespace Museum.DAL.Entities
{
    public class Visit
    {
        public int VisitId { get; set; }
        public string VisitorName { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal TicketPrice { get; set; }

        public int ExhibitionId { get; set; }
        public virtual Exhibition Exhibition { get; set; }
    }
}