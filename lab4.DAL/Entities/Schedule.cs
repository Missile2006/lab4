using System;

namespace Museum.DAL.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int ExhibitionId { get; set; }
        public virtual Exhibition Exhibition { get; set; }
    }
}