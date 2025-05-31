using Museum.BLL.Models;
using System;
using System.Collections.Generic;

namespace Museum.BLL.Interfaces
{
    public interface IScheduleService
    {
        void AddSchedule(ScheduleModel scheduleModel);
        void DeleteSchedule(int id);
        ScheduleModel GetSchedule(int id);
        IEnumerable<ScheduleModel> GetAllSchedules();
        IEnumerable<ScheduleModel> GetExhibitionSchedules(int exhibitionId);
        IEnumerable<ScheduleModel> GetAvailableSchedules(DateTime date);
        bool IsTimeSlotAvailable(int exhibitionId, DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
}