

namespace lab4.Models
{
    public class Exhibition
    {
        public int ExhibitionId { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public string TargetAudience { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Tour> Tours { get; set; }
    }
}
