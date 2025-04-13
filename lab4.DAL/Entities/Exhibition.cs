namespace Museum.DAL.Entities
{
    public class Exhibition
    {
        public int ExhibitionId { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public string TargetAudience { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}