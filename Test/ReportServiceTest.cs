using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Museum.BLL.Services;
using Museum.BLL.Models.Reports;
using Museum.BLL.Models;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;

namespace Museum.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IVisitRepository> _visitRepoMock;
        private readonly Mock<IExhibitionRepository> _exhibitionRepoMock;
        private readonly IMapper _mapper;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _visitRepoMock = new Mock<IVisitRepository>();
            _exhibitionRepoMock = new Mock<IExhibitionRepository>();

            _unitOfWorkMock.Setup(u => u.Visits).Returns(_visitRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Exhibitions).Returns(_exhibitionRepoMock.Object);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Exhibition, ExhibitionModel>();
            });
            _mapper = config.CreateMapper();

            _reportService = new ReportService(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public void GenerateVisitsReport_ValidRange_ReturnsCorrectReport()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            var visits = new List<Visit>
            {
                new Visit { VisitId = 1, ExhibitionId = 1, TicketPrice = 50 },
                new Visit { VisitId = 2, ExhibitionId = 1, TicketPrice = 100 },
                new Visit { VisitId = 3, ExhibitionId = 2, TicketPrice = 75 }
            };

            var exhibition1 = new Exhibition { ExhibitionId = 1, Title = "Art" };
            var exhibition2 = new Exhibition { ExhibitionId = 2, Title = "History" };

            _visitRepoMock.Setup(r => r.GetByDateRange(startDate, endDate)).Returns(visits);
            _exhibitionRepoMock.Setup(r => r.GetById(1)).Returns(exhibition1);
            _exhibitionRepoMock.Setup(r => r.GetById(2)).Returns(exhibition2);

            // Act
            var report = _reportService.GenerateVisitsReport(startDate, endDate);

            // Assert
            Assert.Equal(3, report.TotalVisits);
            Assert.Equal(225, report.TotalIncome);
            Assert.Equal(2, report.ExhibitionStats.Count);

            var stat1 = report.ExhibitionStats.First(x => x.Exhibition.ExhibitionId == 1);
            Assert.Equal(2, stat1.VisitCount);
            Assert.Equal(150, stat1.TotalIncome);

            var stat2 = report.ExhibitionStats.First(x => x.Exhibition.ExhibitionId == 2);
            Assert.Equal(1, stat2.VisitCount);
            Assert.Equal(75, stat2.TotalIncome);
        }
    }
}
