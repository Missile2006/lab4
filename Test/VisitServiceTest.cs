using Xunit;
using Moq;
using AutoMapper;
using System;
using Museum.BLL.Services;
using Museum.BLL.Models;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using System.Collections.Generic;

namespace Museum.Tests.Services
{
    public class VisitServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IVisitRepository> _visitRepoMock;
        private readonly Mock<IExhibitionRepository> _exhibitionRepoMock;
        private readonly IMapper _mapper;
        private readonly VisitService _visitService;

        public VisitServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _visitRepoMock = new Mock<IVisitRepository>();
            _exhibitionRepoMock = new Mock<IExhibitionRepository>();

            // Підключення підставних репозиторіїв до unitOfWork
            _unitOfWorkMock.Setup(u => u.Visits).Returns(_visitRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Exhibitions).Returns(_exhibitionRepoMock.Object);

            // Mapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VisitModel, Visit>();
                cfg.CreateMap<Visit, VisitModel>();
            });
            _mapper = config.CreateMapper();

            _visitService = new VisitService(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public void AddVisit_ValidModel_AddsVisitAndSavesChanges()
        {
            // Arrange
            var visitModel = new VisitModel
            {
                ExhibitionId = 1,
                VisitorName = "Олександр",
                VisitDate = DateTime.Today.AddDays(1),
                TicketPrice = 100
            };

            _exhibitionRepoMock.Setup(repo => repo.GetById(1)).Returns(new Exhibition { ExhibitionId = 1 });

            // Act
            _visitService.AddVisit(visitModel);

            // Assert
            _visitRepoMock.Verify(r => r.Add(It.IsAny<Visit>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }
    }
}
