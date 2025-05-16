using Xunit;
using Moq;
using AutoMapper;
using Museum.BLL.Services;
using Museum.BLL.Models;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using System;
using Museum.BLL.Strategies.Models;

public class TourServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TourService _tourService;

    public TourServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _tourService = new TourService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void PlanGroupTour_ValidTour_AddsTourSuccessfully()
    {
        // Arrange
        var model = new TourModel
        {
            ExhibitionId = 1,
            GuideName = "Олег",
            TourDate = DateTime.Today.AddDays(1),
            ParticipantsCount = 10,
            Price = 500
        };

        var creationModel = new TourCreationModel
        {
            ExhibitionId = model.ExhibitionId,
            GuideName = model.GuideName,
            TourDate = model.TourDate,
            Price = model.Price,
            ParticipantsCount = model.ParticipantsCount
        };

        var tour = new Tour
        {
            ExhibitionId = model.ExhibitionId,
            GuideName = model.GuideName,
            TourDate = model.TourDate,
            Price = model.Price
        };

        _mapperMock.Setup(m => m.Map<TourCreationModel>(model)).Returns(creationModel);
        _unitOfWorkMock.Setup(u => u.Exhibitions.GetById(model.ExhibitionId)).Returns(new Exhibition());
        _unitOfWorkMock.Setup(u => u.Tours.Add(It.IsAny<Tour>()));
        _unitOfWorkMock.Setup(u => u.SaveChanges());

        // Підміна мапінгу для TourModel → Tour
        _mapperMock.Setup(m => m.Map<Tour>(It.IsAny<TourModel>())).Returns(tour);

        // Act
        _tourService.PlanGroupTour(model);

        // Assert
        _unitOfWorkMock.Verify(u => u.Tours.Add(It.IsAny<Tour>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
    }
}
