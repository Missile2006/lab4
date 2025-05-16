using AutoMapper;
using Moq;
using Museum.BLL.Mapping;
using Museum.BLL.Models;
using Museum.BLL.Services;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.DAL.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests
{
    public class ExhibitionServiceTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IExhibitionRepository> _exhibitions;
        private readonly Mock<IScheduleRepository> _schedules;
        private readonly Mock<IVisitRepository> _visits;
        private readonly Mock<ITourRepository> _tours;
        private readonly ExhibitionService _exhibitionService;

        public ExhibitionServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _exhibitions = new Mock<IExhibitionRepository>();
            _schedules = new Mock<IScheduleRepository>();
            _visits = new Mock<IVisitRepository>();
            _tours = new Mock<ITourRepository>();

            // Прив'язуємо моки до UnitOfWork
            _mockUnitOfWork.Setup(u => u.Exhibitions).Returns(_exhibitions.Object);
            _mockUnitOfWork.Setup(u => u.Schedules).Returns(_schedules.Object);
            _mockUnitOfWork.Setup(u => u.Visits).Returns(_visits.Object);
            _mockUnitOfWork.Setup(u => u.Tours).Returns(_tours.Object);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MuseumProfile>());
            var mapper = mapperConfig.CreateMapper();

            _exhibitionService = new ExhibitionService(_mockUnitOfWork.Object, mapper);
        }

        [Fact]
        // Тест на додавання валідної виставки
        public void AddExhibition_ValidData_ShouldCallAddAndSave() 
        {
            var model = new ExhibitionModel
            {
                Title = "Test",
                Theme = "Art",
                TargetAudience = "Adults",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            _exhibitionService.AddExhibition(model);

            _exhibitions.Verify(r => r.Add(It.IsAny<Exhibition>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        // Тест: якщо передано null — очікується ArgumentNullException
        public void AddExhibition_Null_ThrowsException() 
        {
            Assert.Throws<ArgumentNullException>(() => _exhibitionService.AddExhibition(null));
        }

        [Fact]
        // Тест: якщо заголовок порожній — має бути ArgumentException
        public void AddExhibition_EmptyTitle_ThrowsException()
        {
            var model = new ExhibitionModel
            {
                Title = "",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            Assert.Throws<ArgumentException>(() => _exhibitionService.AddExhibition(model));
        }

        [Fact]
        // Тест: якщо кінець раніше за початок — очікується ArgumentException
        public void AddExhibition_InvalidDates_ThrowsException() 
        {
            var model = new ExhibitionModel
            {
                Title = "Test",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(-1)
            };

            Assert.Throws<ArgumentException>(() => _exhibitionService.AddExhibition(model));
        }

        [Fact]
        // Тест оновлення: якщо виставку не знайдено — очікується KeyNotFoundException
        public void UpdateExhibition_NotFound_ThrowsException()
        {
            var model = new ExhibitionModel { ExhibitionId = 1 };
            _exhibitions.Setup(e => e.GetById(1)).Returns((Exhibition)null);

            Assert.Throws<KeyNotFoundException>(() => _exhibitionService.UpdateExhibition(model));
        }

        [Fact]
        // Тест оновлення валідної виставки — має викликатись Update і SaveChanges
        public void UpdateExhibition_ValidData_ShouldUpdateAndSave()
        {
            var existing = new Exhibition { ExhibitionId = 1 };
            var model = new ExhibitionModel
            {
                ExhibitionId = 1,
                Title = "New",
                Theme = "Theme",
                TargetAudience = "Kids",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            _exhibitions.Setup(e => e.GetById(1)).Returns(existing);

            _exhibitionService.UpdateExhibition(model);

            _exhibitions.Verify(e => e.Update(It.IsAny<Exhibition>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        // Тест видалення: якщо у виставки є пов’язані записи — має викинути InvalidOperationException
        public void DeleteExhibition_WithRelations_ThrowsException()
        {
            _exhibitions.Setup(e => e.GetById(1)).Returns(new Exhibition { ExhibitionId = 1 });
            _schedules.Setup(s => s.GetByExhibitionId(1)).Returns(new List<Schedule> { new Schedule() });
            _visits.Setup(v => v.GetByExhibitionId(1)).Returns(new List<Visit>());
            _tours.Setup(t => t.GetByExhibitionId(1)).Returns(new List<Tour>());

            Assert.Throws<InvalidOperationException>(() => _exhibitionService.DeleteExhibition(1));
        }

        [Fact]
        // Тест видалення: якщо немає зв'язків — має відбутись Delete і SaveChanges
        public void DeleteExhibition_ValidData_ShouldDelete()
        {
            _exhibitions.Setup(e => e.GetById(1)).Returns(new Exhibition { ExhibitionId = 1 });
            _schedules.Setup(s => s.GetByExhibitionId(1)).Returns(new List<Schedule>());
            _visits.Setup(v => v.GetByExhibitionId(1)).Returns(new List<Visit>());
            _tours.Setup(t => t.GetByExhibitionId(1)).Returns(new List<Tour>());

            _exhibitionService.DeleteExhibition(1);

            _exhibitions.Verify(e => e.Delete(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        // Тест отримання однієї виставки — перевірка, що мапінг працює
        public void GetExhibition_ShouldReturnMappedModel()
        {
            _exhibitions.Setup(e => e.GetById(1)).Returns(new Exhibition { ExhibitionId = 1, Title = "Test" });

            var result = _exhibitionService.GetExhibition(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        // Тест отримання всіх виставок — перевірка, що повертається список з мапінгом
        public void GetAllExhibitions_ShouldReturnMappedModels()
        {
            _exhibitions.Setup(e => e.GetAll()).Returns(new List<Exhibition>
            {
                new Exhibition { ExhibitionId = 1, Title = "One" },
                new Exhibition { ExhibitionId = 2, Title = "Two" }
            });

            var result = _exhibitionService.GetAllExhibitions().ToList();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        // Тест пошуку за темою — має повернути знайдені виставки
        public void SearchByTheme_ShouldReturnFiltered()
        {
            _exhibitions.Setup(e => e.GetByTheme("Art")).Returns(new List<Exhibition>
            {
                new Exhibition { Title = "Art" }
            });

            var result = _exhibitionService.SearchExhibitionsByTheme("Art").ToList();

            Assert.Single(result);
        }

        [Fact]
        // Тест пошуку за аудиторією: якщо вхід порожній — повертається порожній список
        public void SearchByAudience_EmptyInput_ShouldReturnEmpty()
        {
            var result = _exhibitionService.SearchExhibitionsByAudience(null);
            Assert.Empty(result);
        }
    }
}
