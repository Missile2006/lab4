
namespace lab4.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int ExhibitionId { get; set; }
        public Exhibition Exhibition { get; set; }
    }
}
