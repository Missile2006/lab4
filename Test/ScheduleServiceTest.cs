using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Museum.BLL.Models;
using Museum.BLL.Services;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.DAL.UoW;

public class ScheduleServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IScheduleRepository> _mockScheduleRepo;
    private readonly Mock<IExhibitionRepository> _mockExhibitionRepo;
    private readonly IMapper _mapper;
    private readonly ScheduleService _scheduleService;

    public ScheduleServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockScheduleRepo = new Mock<IScheduleRepository>();
        _mockExhibitionRepo = new Mock<IExhibitionRepository>();

        _mockUnitOfWork.Setup(u => u.Schedules).Returns(_mockScheduleRepo.Object);
        _mockUnitOfWork.Setup(u => u.Exhibitions).Returns(_mockExhibitionRepo.Object);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ScheduleModel, Schedule>().ReverseMap();
        });
        _mapper = config.CreateMapper();

        _scheduleService = new ScheduleService(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public void AddSchedule_ValidModel_AddsSchedule()
    {
        var model = new ScheduleModel
        {
            ExhibitionId = 1,
            ScheduledDate = DateTime.Today,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(11, 0, 0)
        };

        _mockExhibitionRepo.Setup(x => x.GetById(1)).Returns(new Exhibition());

        _scheduleService.AddSchedule(model);

        _mockScheduleRepo.Verify(r => r.Add(It.IsAny<Schedule>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AddSchedule_NullModel_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _scheduleService.AddSchedule(null));
    }

    [Fact]
    public void AddSchedule_EndTimeBeforeStartTime_ThrowsException()
    {
        var model = new ScheduleModel
        {
            ExhibitionId = 1,
            ScheduledDate = DateTime.Today,
            StartTime = new TimeSpan(11, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };

        Assert.Throws<ArgumentException>(() => _scheduleService.AddSchedule(model));
    }

    [Fact]
    public void DeleteSchedule_ValidId_DeletesSchedule()
    {
        _mockScheduleRepo.Setup(r => r.GetById(1)).Returns(new Schedule());

        _scheduleService.DeleteSchedule(1);

        _mockScheduleRepo.Verify(r => r.Delete(1), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void DeleteSchedule_NotFound_ThrowsException()
    {
        _mockScheduleRepo.Setup(r => r.GetById(1)).Returns((Schedule)null);

        Assert.Throws<KeyNotFoundException>(() => _scheduleService.DeleteSchedule(1));
    }

    [Fact]
    public void GetSchedule_ValidId_ReturnsSchedule()
    {
        var schedule = new Schedule { ExhibitionId = 1 };
        _mockScheduleRepo.Setup(r => r.GetById(1)).Returns(schedule);

        var result = _scheduleService.GetSchedule(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.ExhibitionId);
    }

    [Fact]
    public void IsTimeSlotAvailable_NoConflicts_ReturnsTrue()
    {
        _mockScheduleRepo.Setup(r => r.GetByExhibitionId(1)).Returns(new List<Schedule>());

        var result = _scheduleService.IsTimeSlotAvailable(1, DateTime.Today, new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0));

        Assert.True(result);
    }

    [Fact]
    public void IsTimeSlotAvailable_WithConflict_ReturnsFalse()
    {
        _mockScheduleRepo.Setup(r => r.GetByExhibitionId(1)).Returns(new List<Schedule>
        {
            new Schedule
            {
                Date = DateTime.Today,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0)
            }
        });

        var result = _scheduleService.IsTimeSlotAvailable(1, DateTime.Today, new TimeSpan(10, 30, 0), new TimeSpan(11, 30, 0));

        Assert.False(result);
    }
}
