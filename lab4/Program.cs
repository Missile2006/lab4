using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Interfaces;
using Museum.DAL.Repositories;
using Museum.DAL.Entities;
using Museum.BLL.Services;
using Museum.BLL.Strategies.Models;
using System.Diagnostics;
using System.Globalization;
using Museum.DAL.UoW;
using AutoMapper;
using Museum.BLL.Mapping;
using Museum.BLL.Models;
using Microsoft.Extensions.DependencyInjection;


namespace Museum.ConsoleUI
{
    class Program
    {
        private static IUnitOfWork _unitOfWork;
        private static ExhibitionService _exhibitionService;
        private static ScheduleService _scheduleService;
        private static VisitService _visitService;
        private static TourService _tourService;
        private static ReportService _reportService;
        private static IMapper _mapper;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InitializeServices();
            InitializeMapper();         

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Система управління музеєм ===");
                Console.WriteLine("1. Управління експозиціями");
                Console.WriteLine("2. Управління розкладами");
                Console.WriteLine("3. Управління візитами");
                Console.WriteLine("4. Управління екскурсіями");
                Console.WriteLine("5. Звіти");
                Console.WriteLine("6. Вийти");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ManageExhibitions();
                        break;
                    case "2":
                        ManageSchedules();
                        break;
                    case "3":
                        ManageVisits();
                        break;
                    case "4":
                        ManageTours();
                        break;
                    case "5":
                        ShowReports();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        public static void InitializeServices()
        {
            var services = new ServiceCollection();

            // Налаштування бази даних
            services.AddDbContext<MuseumContext>(options =>
                options.UseSqlite("Data Source=Museum.db"));

            // AutoMapper
            services.AddAutoMapper(typeof(MuseumProfile));

            // Реєстрація репозиторіїв та сервісів
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ExhibitionService>();
            services.AddScoped<ScheduleService>();
            services.AddScoped<VisitService>();
            services.AddScoped<TourService>();
            services.AddScoped<ReportService>();

            var serviceProvider = services.BuildServiceProvider();

            // Отримуємо екземпляри сервісів
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _exhibitionService = serviceProvider.GetRequiredService<ExhibitionService>();
            _scheduleService = serviceProvider.GetRequiredService<ScheduleService>();
            _visitService = serviceProvider.GetRequiredService<VisitService>();
            _tourService = serviceProvider.GetRequiredService<TourService>();
            _reportService = serviceProvider.GetRequiredService<ReportService>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();

            // Ініціалізація тестових даних
            InitializeSampleData(serviceProvider.GetRequiredService<MuseumContext>());
        }

        private static void InitializeSampleData(MuseumContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Exhibitions.Any())
            {
                // Додавання тестових даних
                var exhibitions = new List<Exhibition>
        {
            new Exhibition { Title = "Сучасне мистецтво", Theme = "Сучасне", TargetAudience = "Дорослі", StartDate = DateTime.Parse("2025-04-29"), EndDate = DateTime.Parse("2025-05-29") },
            new Exhibition { Title = "Історія стародавнього світу", Theme = "Історичні", TargetAudience = "Всі", StartDate = DateTime.Parse("2025-04-24"), EndDate = DateTime.Parse("2025-05-24") },
            new Exhibition { Title = "Ліс", Theme = "Природа", TargetAudience = "Дорослі", StartDate = DateTime.Parse("2025-05-09"), EndDate = DateTime.Parse("2025-06-05") }
        };

                context.Exhibitions.AddRange(exhibitions);
                context.SaveChanges();
            }
        }

        static void InitializeDatabase()
        {
            var options = new DbContextOptionsBuilder<MuseumContext>()
                .UseSqlite("Data Source=Museum.db")
                .Options;

            var context = new MuseumContext(options);
            context.Database.EnsureCreated();
            context.SaveChanges();

            // Ініціалізація AutoMapper
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Exhibition, ExhibitionModel>().ReverseMap();
                // Додайте інші мапінги, які використовуються
            });
            IMapper mapper = mapperConfiguration.CreateMapper();

            var unitOfWork = new UnitOfWork(context);

            // Тепер передаємо і unitOfWork, і mapper
            _exhibitionService = new ExhibitionService(unitOfWork, mapper);
            _scheduleService = new ScheduleService(unitOfWork, mapper);
            _visitService = new VisitService(unitOfWork, mapper);
            _tourService = new TourService(unitOfWork, mapper);
            _reportService = new ReportService(unitOfWork, mapper);

            // Додамо тестові дані при першому запуску
            if (!_exhibitionService.GetAllExhibitions().Any())
            {
                AddSampleData();
            }
        }
        static void InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
        {
        cfg.AddProfile<MuseumProfile>(); // ТУТ додається твій MuseumProfile
        });

        _mapper = config.CreateMapper();
        }

        static void AddSampleData()
        {
            // Додаємо тестові експозиції
            var exhibition1 = new ExhibitionModel
            {
                Title = "Сучасне мистецтво",
                Theme = "Сучасне",
                TargetAudience = "Дорослі",
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(25)
            };

            var exhibition2 = new ExhibitionModel
            {
                Title = "Історія стародавнього світу",
                Theme = "Історичне",
                TargetAudience = "Всі",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(20)
            };

            _exhibitionService.AddExhibition(exhibition1);
            _exhibitionService.AddExhibition(exhibition2);

            // Додаємо розклади
            var schedule1 = new ScheduleModel
            {
                ScheduledDate = DateTime.Today,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                ExhibitionId = exhibition1.ExhibitionId // Після додавання потрібно отримати справжній ID!
            };

            var schedule2 = new ScheduleModel
            {
                ScheduledDate = DateTime.Today.AddDays(1),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                ExhibitionId = exhibition2.ExhibitionId
            };

            _scheduleService.AddSchedule(schedule1);
            _scheduleService.AddSchedule(schedule2);

            Console.WriteLine("Додано тестові дані для демонстрації.");
        }


        #region Exhibition Management
        static void ManageExhibitions()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управління експозиціями ===");
                Console.WriteLine("1. Переглянути всі експозиції");
                Console.WriteLine("2. Додати нову експозицію");
                Console.WriteLine("3. Редагувати експозицію");
                Console.WriteLine("4. Видалити експозицію");
                Console.WriteLine("5. Пошук за темою");
                Console.WriteLine("6. Пошук за цільовою аудиторією");
                Console.WriteLine("7. Повернутися до головного меню");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllExhibitions();
                        break;
                    case "2":
                        AddExhibition();
                        break;
                    case "3":
                        EditExhibition();
                        break;
                    case "4":
                        DeleteExhibition();
                        break;
                    case "5":
                        SearchExhibitionsByTheme();
                        break;
                    case "6":
                        SearchExhibitionsByAudience();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void ShowAllExhibitions()
        {
            var exhibitions = _exhibitionService.GetAllExhibitions();

            Console.WriteLine("\nСписок всіх експозицій:");
            Console.WriteLine("ID\tНазва\t\tТема\tАудиторія\tПочаток\t\tКінець");
            foreach (var ex in exhibitions)
            {
                Console.WriteLine($"{ex.ExhibitionId}\t{ex.Title}\t{ex.Theme}\t{ex.TargetAudience}\t" +
                                $"{ex.StartDate.ToShortDateString()}\t{ex.EndDate.ToShortDateString()}");
            }
        }

        static void AddExhibition()
        {
            Console.WriteLine("\nДодавання нової експозиції:");

            Console.Write("Назва: ");
            var title = Console.ReadLine();

            Console.Write("Тема: ");
            var theme = Console.ReadLine();

            Console.Write("Цільова аудиторія: ");
            var audience = Console.ReadLine();

            Console.Write("Дата початку (рррр-мм-дд): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Дата завершення (рррр-мм-дд): ");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate < startDate)
            {
                Console.Write("Невірний формат дати або дата раніше початку. Спробуйте ще раз (рррр-мм-дд): ");
            }

            // !!! Змінено: створюємо ExhibitionModel, а не Exhibition
            var exhibitionModel = new ExhibitionModel
            {
                Title = title,
                Theme = theme,
                TargetAudience = audience,
                StartDate = startDate,
                EndDate = endDate
            };

            try
            {
                _exhibitionService.AddExhibition(exhibitionModel); // Передаємо правильну модель
                Console.WriteLine("Експозицію успішно додано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }


        static void EditExhibition()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції для редагування: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                var exhibition = _exhibitionService.GetExhibition(id);
                if (exhibition == null)
                {
                    Console.WriteLine("Експозиція з таким ID не знайдена.");
                    return;
                }

                Console.WriteLine("\nПоточні дані:");
                Console.WriteLine($"Назва: {exhibition.Title}");
                Console.WriteLine($"Тема: {exhibition.Theme}");
                Console.WriteLine($"Аудиторія: {exhibition.TargetAudience}");
                Console.WriteLine($"Початок: {exhibition.StartDate.ToShortDateString()}");
                Console.WriteLine($"Кінець: {exhibition.EndDate.ToShortDateString()}");

                Console.WriteLine("\nВведіть нові дані (залиште порожнім, щоб не змінювати):");
                Console.Write("Нова назва: ");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrEmpty(newTitle))
                    exhibition.Title = newTitle;

                Console.Write("Нова тема: ");
                var newTheme = Console.ReadLine();
                if (!string.IsNullOrEmpty(newTheme))
                    exhibition.Theme = newTheme;

                Console.Write("Нова аудиторія: ");
                var newAudience = Console.ReadLine();
                if (!string.IsNullOrEmpty(newAudience))
                    exhibition.TargetAudience = newAudience;

                Console.Write("Нова дата початку (рррр-мм-дд): ");
                var startDateInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(startDateInput) && DateTime.TryParse(startDateInput, out DateTime newStartDate))
                    exhibition.StartDate = newStartDate;

                Console.Write("Нова дата завершення (рррр-мм-дд): ");
                var endDateInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(endDateInput) && DateTime.TryParse(endDateInput, out DateTime newEndDate))
                    exhibition.EndDate = newEndDate;

                _exhibitionService.UpdateExhibition(exhibition);
                Console.WriteLine("Дані експозиції успішно оновлено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void DeleteExhibition()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції для видалення: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                _exhibitionService.DeleteExhibition(id);
                Console.WriteLine("Експозицію успішно видалено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void SearchExhibitionsByTheme()
        {
            Console.Write("\nВведіть тему для пошуку: ");
            var theme = Console.ReadLine();

            try
            {
                var exhibitions = _exhibitionService.SearchExhibitionsByTheme(theme);
                Console.WriteLine("\nРезультати пошуку:");
                foreach (var ex in exhibitions)
                {
                    Console.WriteLine($"{ex.ExhibitionId}: {ex.Title} ({ex.Theme})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void SearchExhibitionsByAudience()
        {
            Console.Write("\nВведіть цільову аудиторію для пошуку: ");
            var audience = Console.ReadLine();

            try
            {
                var exhibitions = _exhibitionService.SearchExhibitionsByAudience(audience);
                Console.WriteLine("\nРезультати пошуку:");
                foreach (var ex in exhibitions)
                {
                    Console.WriteLine($"{ex.ExhibitionId}: {ex.Title} (для {ex.TargetAudience})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
        #endregion

        #region Schedule Management
        static void ManageSchedules()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управління розкладами ===");
                Console.WriteLine("1. Переглянути всі розклади");
                Console.WriteLine("2. Додати новий розклад");
                Console.WriteLine("3. Видалити розклад");
                Console.WriteLine("4. Переглянути розклад конкретної експозиції");
                Console.WriteLine("5. Переглянути доступні часові слоти на дату");
                Console.WriteLine("6. Повернутися до головного меню");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllSchedules();
                        break;
                    case "2":
                        AddSchedule();
                        break;
                    case "3":
                        DeleteSchedule();
                        break;
                    case "4":
                        ShowExhibitionSchedules();
                        break;
                    case "5":
                        ShowAvailableTimeSlots();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void ShowAllSchedules()
        {
            var schedules = _scheduleService.GetAllSchedules();
            Console.WriteLine("\nСписок всіх розкладів:");
            Console.WriteLine("ID\tДата\t\tЧас\t\tЕкспозиція");

            foreach (var s in schedules)
            {
                var exhibition = _exhibitionService.GetExhibition(s.ExhibitionId);
                Console.WriteLine($"{s.ScheduleId}\t{s.ScheduledDate.ToShortDateString()}\t" +
                    $"{s.StartTime}-{s.EndTime}\t{exhibition.Title}");
            }
        }


        static void AddSchedule()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції: ");

            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                var exhibition = _exhibitionService.GetExhibition(exhibitionId);
                if (exhibition == null)
                {
                    Console.WriteLine("Експозиція з таким ID не знайдена.");
                    return;
                }

                Console.Write("Дата (рррр-мм-дд): ");
                DateTime date;
                while (!DateTime.TryParse(Console.ReadLine(), out date))
                {
                    Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
                }

                Console.Write("Час початку (гг:хх): ");
                TimeSpan startTime;
                while (!TimeSpan.TryParse(Console.ReadLine(), out startTime))
                {
                    Console.Write("Невірний формат часу. Спробуйте ще раз (гг:хх): ");
                }

                Console.Write("Час завершення (гг:хх): ");
                TimeSpan endTime;
                while (!TimeSpan.TryParse(Console.ReadLine(), out endTime) || endTime <= startTime)
                {
                    Console.Write("Невірний формат часу або час раніше початку. Спробуйте ще раз (гг:хх): ");
                }

                // Створюємо ScheduleModel замість Schedule (якщо ваш сервіс використовує BLL модель)
                var scheduleModel = new ScheduleModel
                {
                    ScheduledDate = date,
                    StartTime = startTime,
                    EndTime = endTime,
                    ExhibitionId = exhibitionId
                };

                // Перевірка доступності часу для ScheduleModel
                if (!_scheduleService.IsTimeSlotAvailable(exhibitionId, date, startTime, endTime))
                {
                    Console.WriteLine("Цей часовий слот вже зайнятий.");
                    return;
                }

                // Додавання ScheduleModel в сервіс
                _scheduleService.AddSchedule(scheduleModel);
                Console.WriteLine("Розклад успішно додано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }


        static void DeleteSchedule()
        {
            ShowAllSchedules();
            Console.Write("\nВведіть ID розкладу для видалення: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                _scheduleService.DeleteSchedule(id);
                Console.WriteLine("Розклад успішно видалено!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowExhibitionSchedules()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                var schedules = _scheduleService.GetExhibitionSchedules(id);
                var exhibition = _exhibitionService.GetExhibition(id);
                Console.WriteLine($"\nРозклад для експозиції '{exhibition.Title}':");

                foreach (var s in schedules)
                {
                    Console.WriteLine($"{s.ScheduledDate.ToShortDateString()}: {s.StartTime}-{s.EndTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowAvailableTimeSlots()
        {
            Console.Write("\nВведіть дату для перегляду доступних слотів (рррр-мм-дд): ");
            DateTime date;
            while (!DateTime.TryParse(Console.ReadLine(), out date))
            {
                Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
            }

            try
            {
                // Отримуємо всі слоти без сортування в SQL
                var slots = _scheduleService.GetAvailableSchedules(date);

                // Сортуємо на стороні клієнта
                var sortedSlots = slots.OrderBy(s => s.StartTime).ToList();

                Console.WriteLine("\nДоступні часові слоти:");

                foreach (var s in sortedSlots)
                {
                    var exhibition = _exhibitionService.GetExhibition(s.ExhibitionId);
                    Console.WriteLine($"{s.StartTime}-{s.EndTime} - {exhibition.Title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        #region Visit Management
        static void ManageVisits()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управління візитами ===");
                Console.WriteLine("1. Переглянути всі візити");
                Console.WriteLine("2. Запланувати новий візит");
                Console.WriteLine("3. Скасувати візит");
                Console.WriteLine("4. Пошук візитів за ім'ям відвідувача");
                Console.WriteLine("5. Пошук візитів за датою");
                Console.WriteLine("6. Повернутися до головного меню");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllVisits();
                        break;
                    case "2":
                        AddVisit();
                        break;
                    case "3":
                        DeleteVisit();
                        break;
                    case "4":
                        SearchVisitsByName();
                        break;
                    case "5":
                        SearchVisitsByDate();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void ShowAllVisits()
        {
            var visits = _visitService.GetAllVisits();
            Console.WriteLine("\nСписок всіх візитів:");
            Console.WriteLine("ID\tІм'я\t\tДата\t\tЦіна\tЕкспозиція");

            foreach (var v in visits)
            {
                var exhibition = _exhibitionService.GetExhibition(v.ExhibitionId);
                Console.WriteLine($"{v.VisitId}\t{v.VisitorName}\t{v.VisitDate.ToShortDateString()}\t" +
                    $"{v.TicketPrice}\t{exhibition.Title}");
            }
        }

        static void AddVisit()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції: ");

            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                var exhibition = _exhibitionService.GetExhibition(exhibitionId);
                if (exhibition == null)
                {
                    Console.WriteLine("Експозиція з таким ID не знайдена.");
                    return;
                }

                Console.Write("Ім'я відвідувача: ");
                var name = Console.ReadLine();

                Console.Write("Дата візиту (рррр-мм-дд): ");
                DateTime visitDate;
                while (!DateTime.TryParseExact(Console.ReadLine(),
                       "yyyy-MM-dd",
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None,
                       out visitDate) || visitDate < DateTime.Today)
                {
                    Console.Write("Невірний формат (очікується рррр-мм-дд) або дата в минулому. Спробуйте ще раз: ");
                }

                Console.Write("Ціна квитка: ");
                decimal price;
                while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
                {
                    Console.Write("Невірний формат ціни. Спробуйте ще раз: ");
                }

                var visitModel = new VisitModel
                {
                    VisitorName = name,
                    VisitDate = visitDate,
                    TicketPrice = price,
                    ExhibitionId = exhibitionId
                };

                _visitService.AddVisit(visitModel);
                Console.WriteLine("Візит успішно заплановано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }


        static void DeleteVisit()
        {
            ShowAllVisits();
            Console.Write("\nВведіть ID візиту для скасування: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                _visitService.DeleteVisit(id);
                Console.WriteLine("Візит успішно скасовано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void SearchVisitsByName()
        {
            Console.Write("\nВведіть ім'я або частину імені відвідувача: ");
            var name = Console.ReadLine();

            try
            {
                var visits = _visitService.SearchVisitsByName(name);
                Console.WriteLine("\nРезультати пошуку:");

                foreach (var v in visits)
                {
                    var exhibition = _exhibitionService.GetExhibition(v.ExhibitionId);
                    Console.WriteLine($"{v.VisitDate.ToShortDateString()}: {v.VisitorName} - {exhibition.Title} ({v.TicketPrice} грн)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void SearchVisitsByDate()
        {
            Console.Write("\nВведіть початкову дату (рррр-мм-дд): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Введіть кінцеву дату (рррр-мм-дд): ");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate < startDate)
            {
                Console.Write("Невірний формат дати або дата раніше початку. Спробуйте ще раз (рррр-мм-дд): ");
            }

            try
            {
                var visits = _visitService.SearchVisitsByDateRange(startDate, endDate);
                Console.WriteLine("\nРезультати пошуку:");

                foreach (var v in visits)
                {
                    var exhibition = _exhibitionService.GetExhibition(v.ExhibitionId);
                    Console.WriteLine($"{v.VisitDate.ToShortDateString()}: {v.VisitorName} - {exhibition.Title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
        #endregion

        #region Tour Management
        static void ManageTours()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управління екскурсіями ===");
                Console.WriteLine("1. Переглянути всі екскурсії (ліниво)");
                Console.WriteLine("2. Переглянути всі екскурсії (жадібно)");
                Console.WriteLine("3. Додати нову екскурсію");
                Console.WriteLine("4. Скасувати екскурсію");
                Console.WriteLine("5. Переглянути екскурсії експозиції");
                Console.WriteLine("6. Переглянути приватні екскурсії");
                Console.WriteLine("7. Переглянути заплановані екскурсії");
                Console.WriteLine("8. Пошук екскурсій за ім'ям гіда");
                Console.WriteLine("9. Деталі туру (ліниво)");
                Console.WriteLine("10. Деталі туру (жадібно)");
                Console.WriteLine("11. Порівняння підходів завантаження");
                Console.WriteLine("12. Повернутися до головного меню");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowAllTours(false); // Лінивий підхід
                        break;
                    case "2":
                        ShowAllTours(true); // Жадібний підхід
                        break;
                    case "3":
                        AddTour();
                        break;
                    case "4":
                        DeleteTour();
                        break;
                    case "5":
                        ShowExhibitionTours();
                        break;
                    case "6":
                        ShowPrivateTours();
                        break;
                    case "7":
                        ShowScheduledTours();
                        break;
                    case "8":
                        SearchToursByGuide();
                        break;
                    case "9":
                        Console.Write("Введіть ID туру для деталей (ліниво): ");
                        if (int.TryParse(Console.ReadLine(), out int lazyId))
                            ShowTourDetails(lazyId, false);
                        else
                            Console.WriteLine("Невірний формат ID");
                        break;
                    case "10":
                        Console.Write("Введіть ID туру для деталей (жадібно): ");
                        if (int.TryParse(Console.ReadLine(), out int eagerId))
                            ShowTourDetails(eagerId, true);
                        else
                            Console.WriteLine("Невірний формат ID");
                        break;
                    case "11":
                        ShowLoadingApproachesComparison();
                        break;
                    case "12":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void ShowAllTours(bool eagerLoading)
        {
            Console.WriteLine("\n=== Список всіх екскурсій ===");
            Console.WriteLine("Завантаження: " + (eagerLoading ? "Жадібне" : "Ліниве"));

            IEnumerable<TourModel> tours;
            if (eagerLoading)
                tours = _tourService.GetAllToursWithExhibitions();  // Потрібно реалізувати цей метод
            else
                tours = _tourService.GetAllTours();

            Console.WriteLine("ID\tДата\t\tГід\t\tТип\tЦіна\t" + (eagerLoading ? "Експозиція" : ""));

            foreach (var t in tours)
            {
                var tourType = t.IsPrivate ? "Приватна" : "Групова";
                Console.Write($"{t.TourId}\t{t.TourDate.ToShortDateString()}\t{t.GuideName}\t{tourType}\t{t.Price}");

                if (eagerLoading)
                    Console.WriteLine($"\t{t.Exhibition?.Title}");  // Треба, щоб у TourModel була властивість ExhibitionModel Exhibition
                else
                    Console.WriteLine();
            }
        }


        static void ShowTourDetails(int tourId, bool eagerLoading)
        {
            Console.WriteLine("\n=== Деталі туру ===");
            Console.WriteLine("Завантаження: " + (eagerLoading ? "Жадібне" : "Ліниве"));

            TourModel tour; // Змінено Tour -> TourModel
            tour = _tourService.GetTour(tourId, eagerLoading); // Використовуємо один метод GetTour

            if (tour == null)
            {
                Console.WriteLine("Тур з вказаним ID не знайдено");
                return;
            }

            Console.WriteLine($"ID: {tour.TourId}");
            Console.WriteLine($"Гід: {tour.GuideName}");
            Console.WriteLine($"Дата: {tour.TourDate.ToShortDateString()}");
            Console.WriteLine($"Тип: {(tour.IsPrivate ? "Приватна" : "Групова")}");
            Console.WriteLine($"Ціна: {tour.Price} грн");

            if (eagerLoading)
            {
                Console.WriteLine($"Експозиція: {tour.Exhibition?.Title ?? "Невідомо"}");
                Console.WriteLine($"Тема: {tour.Exhibition?.Theme ?? "Невідомо"}");
            }
            else
            {
                Console.WriteLine("\nДля перегляду деталей експозиції використовуйте жадібне завантаження");
            }
        }


        static void ShowLoadingApproachesComparison()
        {
            Console.WriteLine("\n=== Порівняння підходів завантаження ===");

            // Тестування для списків
            Console.WriteLine("\nТестування для списків (10 записів):");

            var sw = Stopwatch.StartNew();
            var lazyTours = _tourService.GetAllTours().Take(10).ToList();
            sw.Stop();
            Console.WriteLine($"Ліниве завантаження: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            var eagerTours = _tourService.GetAllToursWithExhibitions().Take(10).ToList();
            sw.Stop();
            Console.WriteLine($"Жадібне завантаження: {sw.ElapsedMilliseconds}ms");

            // Тестування для одиночних записів
            Console.WriteLine("\nТестування для одиночних записів:");

            sw.Restart();
            var lazyTour = _tourService.GetTour(1); // Ліниве завантаження
            Console.WriteLine($"Ліниве завантаження (1 запит): {sw.ElapsedMilliseconds}ms");

            // Додатковий запит при доступі до зв'язаних даних
            sw.Restart();
            var exhibitionTitle = _exhibitionService.GetExhibition(lazyTour.ExhibitionId)?.Title;
            sw.Stop();
            Console.WriteLine($" + Додатковий запит для виставки: {sw.ElapsedMilliseconds}ms");

            sw.Restart();
            var eagerTour = _tourService.GetTour(1, true); // Жадібне завантаження
            sw.Stop();
            Console.WriteLine($"Жадібне завантаження (1 запит): {sw.ElapsedMilliseconds}ms");
        }


        static void ShowAllTours()
        {
            var tours = _tourService.GetAllTours();
            Console.WriteLine("\nСписок всіх екскурсій:");
            Console.WriteLine("ID\tДата\t\tГід\t\tТип\tЦіна\tЕкспозиція");

            foreach (var t in tours)
            {
                var exhibition = _exhibitionService.GetExhibition(t.ExhibitionId);
                var tourType = t.IsPrivate ? "Приватна" : "Групова";
                Console.WriteLine($"{t.TourId}\t{t.TourDate.ToShortDateString()}\t{t.GuideName}\t" +
                    $"{tourType}\t{t.Price}\t{exhibition.Title}");
            }
        }

        static void AddTour()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції: ");

            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                var exhibition = _exhibitionService.GetExhibition(exhibitionId);
                if (exhibition == null)
                {
                    Console.WriteLine("Експозиція з таким ID не знайдена.");
                    return;
                }

                Console.Write("Ім'я гіда: ");
                var guideName = Console.ReadLine();
                Console.Write("Дата екскурсії (рррр-мм-дд): ");

                DateTime tourDate;
                while (!DateTime.TryParse(Console.ReadLine(), out tourDate) || tourDate < DateTime.Today)
                {
                    Console.Write("Невірний формат дати або дата в минулому. Спробуйте ще раз (рррр-мм-дд): ");
                }

                Console.Write("Це приватна екскурсія? (так/ні): ");
                var isPrivate = Console.ReadLine().ToLower() == "так";

                Console.Write("Ціна: ");
                decimal price;
                while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
                {
                    Console.Write("Невірний формат ціни. Спробуйте ще раз: ");
                }

                // Створюємо TourModel для передачі в BLL
                var tourModel = new TourModel
                {
                    TourDate = tourDate,
                    GuideName = guideName,
                    IsPrivate = isPrivate,
                    Price = price,
                    ExhibitionId = exhibitionId
                };

                // Використовуємо AutoMapper або створюємо відповідний сервіс для додавання екскурсії
                _tourService.AddTour(tourModel); // Передаємо TourModel у метод сервісу

                Console.WriteLine("Екскурсію успішно додано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void DeleteTour()
        {
            ShowAllTours();
            Console.Write("\nВведіть ID екскурсії для скасування: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                _tourService.DeleteTour(id);
                Console.WriteLine("Екскурсію успішно скасовано!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowExhibitionTours()
        {
            ShowAllExhibitions();
            Console.Write("\nВведіть ID експозиції: ");

            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            try
            {
                var tours = _tourService.GetExhibitionTours(id);
                var exhibition = _exhibitionService.GetExhibition(id);
                Console.WriteLine($"\nЕкскурсії для експозиції '{exhibition.Title}':");

                foreach (var t in tours)
                {
                    var tourType = t.IsPrivate ? "Приватна" : "Групова";
                    Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} ({tourType}) - {t.Price} грн");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowPrivateTours()
        {
            try
            {
                var tours = _tourService.GetPrivateTours();
                Console.WriteLine("\nПриватні екскурсії:");

                foreach (var t in tours)
                {
                    var exhibition = _exhibitionService.GetExhibition(t.ExhibitionId);
                    Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} - {exhibition.Title} ({t.Price} грн)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowScheduledTours()
        {
            try
            {
                var tours = _tourService.GetScheduledTours();
                Console.WriteLine("\nЗаплановані екскурсії:");

                foreach (var t in tours)
                {
                    var exhibition = _exhibitionService.GetExhibition(t.ExhibitionId);
                    var tourType = t.IsPrivate ? "Приватна" : "Групова";
                    Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} - {exhibition.Title} ({tourType})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void SearchToursByGuide()
        {
            Console.Write("\nВведіть ім'я або частину імені гіда: ");
            var guideName = Console.ReadLine();

            try
            {
                var tours = _tourService.SearchToursByGuide(guideName);
                Console.WriteLine("\nРезультати пошуку:");

                foreach (var t in tours)
                {
                    var exhibition = _exhibitionService.GetExhibition(t.ExhibitionId);
                    var tourType = t.IsPrivate ? "Приватна" : "Групова";
                    Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} - {exhibition.Title} ({tourType})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
        #endregion

        #region Reports
        static void ShowReports()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Звіти ===");
                Console.WriteLine("1. Поточні експозиції");
                Console.WriteLine("2. Візити за період");
                Console.WriteLine("3. Доходи від екскурсій");
                Console.WriteLine("4. Найпопулярніші експозиції");
                Console.WriteLine("5. Повернутися до головного меню");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowCurrentExhibitionsReport();
                        break;
                    case "2":
                        ShowVisitsReport();
                        break;
                    case "3":
                        ShowToursIncomeReport();
                        break;
                    case "4":
                        ShowPopularExhibitionsReport();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void ShowCurrentExhibitionsReport()
        {
            try
            {
                var currentExhibitions = _reportService.GetCurrentExhibitionsReport();
                Console.WriteLine("\nПоточні експозиції (на " + DateTime.Now.ToShortDateString() + "):");
                Console.WriteLine("Назва\t\tТема\tАудиторія\tЗакінчується");

                foreach (var ex in currentExhibitions)
                {
                    Console.WriteLine($"{ex.Title}\t{ex.Theme}\t{ex.TargetAudience}\t{ex.EndDate.ToShortDateString()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowVisitsReport()
        {
            Console.Write("\nВведіть початкову дату (рррр-мм-дд): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Введіть кінцеву дату (рррр-мм-дд): ");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate < startDate)
            {
                Console.Write("Невірний формат дати або дата раніше початку. Спробуйте ще раз (рррр-мм-дд): ");
            }

            try
            {
                var report = _reportService.GenerateVisitsReport(startDate, endDate);
                Console.WriteLine($"\nЗвіт по візитам за період {report.StartDate.ToShortDateString()} - {report.EndDate.ToShortDateString()}:");
                Console.WriteLine($"Кількість візитів: {report.TotalVisits}");
                Console.WriteLine($"Загальний дохід: {report.TotalIncome} грн");

                Console.WriteLine("\nРозподіл по експозиціях:");
                foreach (var item in report.ExhibitionStats)
                {
                    Console.WriteLine($"{item.Exhibition.Title}: {item.VisitCount} візитів, {item.TotalIncome} грн");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowToursIncomeReport()
        {
            Console.Write("\nВведіть початкову дату (рррр-мм-дд): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Введіть кінцеву дату (рррр-мм-дд): ");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate < startDate)
            {
                Console.Write("Невірний формат дати або дата раніше початку. Спробуйте ще раз (рррр-мм-дд): ");
            }

            try
            {
                var report = _reportService.GenerateToursReport(startDate, endDate);
                Console.WriteLine($"\nЗвіт по екскурсіях за період {report.StartDate.ToShortDateString()} - {report.EndDate.ToShortDateString()}:");
                Console.WriteLine($"Загальний дохід: {report.TotalIncome} грн");
                Console.WriteLine($"Кількість приватних екскурсій: {report.PrivateToursCount}");
                Console.WriteLine($"Кількість групових екскурсій: {report.GroupToursCount}");

                Console.WriteLine("\nРозподіл по гідах:");
                foreach (var item in report.GuideStats)
                {
                    Console.WriteLine($"{item.GuideName}: {item.TourCount} екскурсій, {item.TotalIncome} грн доходу");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ShowPopularExhibitionsReport()
        {
            Console.Write("\nВведіть початкову дату (рррр-мм-дд): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.Write("Невірний формат дати. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Введіть кінцеву дату (рррр-мм-дд): ");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate) || endDate < startDate)
            {
                Console.Write("Невірний формат дати або дата раніше початку. Спробуйте ще раз (рррр-мм-дд): ");
            }

            try
            {
                var report = _reportService.GeneratePopularExhibitionsReport(startDate, endDate);
                Console.WriteLine($"\nРейтинг популярності експозицій за період {report.StartDate.ToShortDateString()} - {report.EndDate.ToShortDateString()}:");
                Console.WriteLine("Місце\tНазва\t\tВізити\tЕкскурсії\tЗагальний бал");

                int place = 1;
                foreach (var item in report.Exhibitions)
                {
                    Console.WriteLine($"{place++}\t{item.Exhibition.Title}\t{item.VisitsCount}\t{item.ToursCount}\t\t{item.TotalScore}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
        #endregion

        #region Додаткові методи для роботи з екскурсіями
        static void PlanGroupTour()
        {
            Console.WriteLine("\nПланування групової екскурсії");
            var model = GetCommonTourData();
            if (model == null) return;

            Console.Write("Кількість учасників: ");
            if (!int.TryParse(Console.ReadLine(), out int participantsCount) || participantsCount <= 0)
            {
                Console.WriteLine("Невірна кількість учасників.");
                return;
            }

            // Створюємо TourModel
            var tourModel = new TourModel
            {
                ExhibitionId = model.ExhibitionId,
                TourDate = model.TourDate,
                GuideName = model.GuideName,
                IsPrivate = false, // Групова екскурсія
            };

            // Якщо потрібно зберігати додаткові дані, наприклад, кількість учасників
            try
            {
                _tourService.PlanGroupTour(tourModel);
                Console.WriteLine($"\nГрупова екскурсія успішно запланована на {tourModel.TourDate.ToShortDateString()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
            }
        }

        static void PlanPrivateTour()
        {
            Console.WriteLine("\nПланування приватної екскурсії");
            var model = GetCommonTourData();
            if (model == null) return;

            Console.Write("Тривалість екскурсії (години): ");
            if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
            {
                Console.WriteLine("Невірна тривалість.");
                return;
            }

            // Створюємо TourModel
            var tourModel = new TourModel
            {
                ExhibitionId = model.ExhibitionId,
                TourDate = model.TourDate,
                GuideName = model.GuideName,
                IsPrivate = true // Приватна екскурсія
            };

            // Якщо потрібно зберігати додаткові дані (наприклад, кількість учасників чи тривалість), зберігайте їх окремо
            int participantsCount = 0; // Якщо необхідно, додайте логіку для кількості учасників

            try
            {
                _tourService.PlanPrivateTour(tourModel);
                Console.WriteLine($"\nПриватна екскурсія успішно запланована на {tourModel.TourDate.ToShortDateString()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nПомилка: {ex.Message}");
            }
        }
        static TourCreationModel GetCommonTourData()
        {
            ShowCurrentExhibitionsReport();

            Console.Write("\nВведіть ID експозиції: ");
            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return null;
            }

            var exhibition = _exhibitionService.GetExhibition(exhibitionId);
            if (exhibition == null)
            {
                Console.WriteLine("Експозиція з таким ID не знайдена.");
                return null;
            }

            Console.Write("Введіть дату екскурсії (рррр-мм-дд): ");
            DateTime tourDate;
            while (!DateTime.TryParse(Console.ReadLine(), out tourDate) || tourDate < DateTime.Today)
            {
                Console.Write("Невірний формат дати або дата в минулому. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Ім'я гіда: ");
            var guideName = Console.ReadLine();

            return new TourCreationModel
            {
                ExhibitionId = exhibitionId,
                TourDate = tourDate,
                GuideName = guideName
            };
            #endregion
        }
    }
}
#endregion

