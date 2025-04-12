using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Museum.DAL.Context;
using Museum.DAL.Interfaces;
using Museum.DAL.Repositories;
using Museum.DAL.Entities;

namespace Museum.ConsoleUI
{
    class Program
    {
        private static IUnitOfWork _unitOfWork;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InitializeDatabase();

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

        static void InitializeDatabase()
        {
            var options = new DbContextOptionsBuilder<MuseumContext>()
                .UseInMemoryDatabase(databaseName: "MuseumDB")
                .Options;

            var context = new MuseumContext(options);
            _unitOfWork = new UnitOfWork(context);

            // Додамо тестові дані при першому запуску
            if (!_unitOfWork.Exhibitions.GetAll().Any())
            {
                AddSampleData();
            }
        }

        static void AddSampleData()
        {
            // Додаємо тестові експозиції
            var exhibition1 = new Exhibition
            {
                Title = "Сучасне мистецтво",
                Theme = "Сучасне",
                TargetAudience = "Дорослі",
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(25)
            };

            var exhibition2 = new Exhibition
            {
                Title = "Історія стародавнього світу",
                Theme = "Історичне",
                TargetAudience = "Всі",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(20)
            };

            _unitOfWork.Exhibitions.Add(exhibition1);
            _unitOfWork.Exhibitions.Add(exhibition2);
            _unitOfWork.SaveChanges();

            // Додаємо розклади
            var schedule1 = new Schedule
            {
                Date = DateTime.Today,
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                ExhibitionId = exhibition1.ExhibitionId
            };

            var schedule2 = new Schedule
            {
                Date = DateTime.Today.AddDays(1),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                ExhibitionId = exhibition2.ExhibitionId
            };

            _unitOfWork.Schedules.Add(schedule1);
            _unitOfWork.Schedules.Add(schedule2);
            _unitOfWork.SaveChanges();

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
            var exhibitions = _unitOfWork.Exhibitions.GetAll();

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

            var exhibition = new Exhibition
            {
                Title = title,
                Theme = theme,
                TargetAudience = audience,
                StartDate = startDate,
                EndDate = endDate
            };

            _unitOfWork.Exhibitions.Add(exhibition);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Експозицію успішно додано!");
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

            var exhibition = _unitOfWork.Exhibitions.GetById(id);
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

            _unitOfWork.Exhibitions.Update(exhibition);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Дані експозиції успішно оновлено!");
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

            var exhibition = _unitOfWork.Exhibitions.GetById(id);
            if (exhibition == null)
            {
                Console.WriteLine("Експозиція з таким ID не знайдена.");
                return;
            }

            // Перевірка наявності пов'язаних даних
            var hasSchedules = _unitOfWork.Schedules.GetByExhibitionId(id).Any();
            var hasVisits = _unitOfWork.Visits.GetByExhibitionId(id).Any();
            var hasTours = _unitOfWork.Tours.GetByExhibitionId(id).Any();

            if (hasSchedules || hasVisits || hasTours)
            {
                Console.WriteLine("Неможливо видалити експозицію, оскільки є пов'язані розклади, візити або екскурсії.");
                return;
            }

            _unitOfWork.Exhibitions.Delete(id);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Експозицію успішно видалено!");
        }

        static void SearchExhibitionsByTheme()
        {
            Console.Write("\nВведіть тему для пошуку: ");
            var theme = Console.ReadLine();

            var exhibitions = _unitOfWork.Exhibitions.GetByTheme(theme);

            Console.WriteLine("\nРезультати пошуку:");
            foreach (var ex in exhibitions)
            {
                Console.WriteLine($"{ex.ExhibitionId}: {ex.Title} ({ex.Theme})");
            }
        }

        static void SearchExhibitionsByAudience()
        {
            Console.Write("\nВведіть цільову аудиторію для пошуку: ");
            var audience = Console.ReadLine();

            var exhibitions = _unitOfWork.Exhibitions.GetByTargetAudience(audience);

            Console.WriteLine("\nРезультати пошуку:");
            foreach (var ex in exhibitions)
            {
                Console.WriteLine($"{ex.ExhibitionId}: {ex.Title} (для {ex.TargetAudience})");
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
            var schedules = _unitOfWork.Schedules.GetAll();

            Console.WriteLine("\nСписок всіх розкладів:");
            Console.WriteLine("ID\tДата\t\tЧас\t\tЕкспозиція");
            foreach (var s in schedules)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(s.ExhibitionId);
                Console.WriteLine($"{s.ScheduleId}\t{s.Date.ToShortDateString()}\t" +
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

            var exhibition = _unitOfWork.Exhibitions.GetById(exhibitionId);
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

            var schedule = new Schedule
            {
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                ExhibitionId = exhibitionId
            };

            _unitOfWork.Schedules.Add(schedule);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Розклад успішно додано!");
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

            var schedule = _unitOfWork.Schedules.GetById(id);
            if (schedule == null)
            {
                Console.WriteLine("Розклад з таким ID не знайдений.");
                return;
            }

            _unitOfWork.Schedules.Delete(id);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Розклад успішно видалено!");
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

            var schedules = _unitOfWork.Schedules.GetByExhibitionId(id);
            var exhibition = _unitOfWork.Exhibitions.GetById(id);

            Console.WriteLine($"\nРозклад для експозиції '{exhibition.Title}':");
            foreach (var s in schedules)
            {
                Console.WriteLine($"{s.Date.ToShortDateString()}: {s.StartTime}-{s.EndTime}");
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

            var slots = _unitOfWork.Schedules.GetAvailableSchedules(date);

            Console.WriteLine("\nДоступні часові слоти:");
            foreach (var s in slots)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(s.ExhibitionId);
                Console.WriteLine($"{s.StartTime}-{s.EndTime} - {exhibition.Title}");
            }
        }
        #endregion

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
            var visits = _unitOfWork.Visits.GetAll();

            Console.WriteLine("\nСписок всіх візитів:");
            Console.WriteLine("ID\tІм'я\t\tДата\t\tЦіна\tЕкспозиція");
            foreach (var v in visits)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(v.ExhibitionId);
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

            var exhibition = _unitOfWork.Exhibitions.GetById(exhibitionId);
            if (exhibition == null)
            {
                Console.WriteLine("Експозиція з таким ID не знайдена.");
                return;
            }

            Console.Write("Ім'я відвідувача: ");
            var name = Console.ReadLine();

            Console.Write("Дата візиту (рррр-мм-дд): ");
            DateTime visitDate;
            while (!DateTime.TryParse(Console.ReadLine(), out visitDate) || visitDate < DateTime.Today)
            {
                Console.Write("Невірний формат дати або дата в минулому. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Ціна квитка: ");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
            {
                Console.Write("Невірний формат ціни. Спробуйте ще раз: ");
            }

            var visit = new Visit
            {
                VisitorName = name,
                VisitDate = visitDate,
                TicketPrice = price,
                ExhibitionId = exhibitionId
            };

            _unitOfWork.Visits.Add(visit);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Візит успішно заплановано!");
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

            var visit = _unitOfWork.Visits.GetById(id);
            if (visit == null)
            {
                Console.WriteLine("Візит з таким ID не знайдений.");
                return;
            }

            _unitOfWork.Visits.Delete(id);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Візит успішно скасовано!");
        }

        static void SearchVisitsByName()
        {
            Console.Write("\nВведіть ім'я або частину імені відвідувача: ");
            var name = Console.ReadLine();

            var visits = _unitOfWork.Visits.GetByVisitorName(name);

            Console.WriteLine("\nРезультати пошуку:");
            foreach (var v in visits)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(v.ExhibitionId);
                Console.WriteLine($"{v.VisitDate.ToShortDateString()}: {v.VisitorName} - {exhibition.Title} ({v.TicketPrice} грн)");
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

            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate);

            Console.WriteLine("\nРезультати пошуку:");
            foreach (var v in visits)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(v.ExhibitionId);
                Console.WriteLine($"{v.VisitDate.ToShortDateString()}: {v.VisitorName} - {exhibition.Title}");
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
                Console.WriteLine("1. Переглянути всі екскурсії");
                Console.WriteLine("2. Додати нову екскурсію");
                Console.WriteLine("3. Скасувати екскурсію");
                Console.WriteLine("4. Переглянути екскурсії експозиції");
                Console.WriteLine("5. Переглянути приватні екскурсії");
                Console.WriteLine("6. Переглянути заплановані екскурсії");
                Console.WriteLine("7. Пошук екскурсій за ім'ям гіда");
                Console.WriteLine("8. Повернутися до головного меню");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllTours();
                        break;
                    case "2":
                        AddTour();
                        break;
                    case "3":
                        DeleteTour();
                        break;
                    case "4":
                        ShowExhibitionTours();
                        break;
                    case "5":
                        ShowPrivateTours();
                        break;
                    case "6":
                        ShowScheduledTours();
                        break;
                    case "7":
                        SearchToursByGuide();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        break;
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }

        static void ShowAllTours()
        {
            var tours = _unitOfWork.Tours.GetAll();

            Console.WriteLine("\nСписок всіх екскурсій:");
            Console.WriteLine("ID\tДата\t\tГід\t\tТип\tЦіна\tЕкспозиція");
            foreach (var t in tours)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(t.ExhibitionId);
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

            var exhibition = _unitOfWork.Exhibitions.GetById(exhibitionId);
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

            var tour = new Tour
            {
                TourDate = tourDate,
                GuideName = guideName,
                IsPrivate = isPrivate,
                Price = price,
                ExhibitionId = exhibitionId
            };

            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Екскурсію успішно додано!");
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

            var tour = _unitOfWork.Tours.GetById(id);
            if (tour == null)
            {
                Console.WriteLine("Екскурсія з таким ID не знайдена.");
                return;
            }

            _unitOfWork.Tours.Delete(id);
            _unitOfWork.SaveChanges();

            Console.WriteLine("Екскурсію успішно скасовано!");
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

            var tours = _unitOfWork.Tours.GetByExhibitionId(id);
            var exhibition = _unitOfWork.Exhibitions.GetById(id);

            Console.WriteLine($"\nЕкскурсії для експозиції '{exhibition.Title}':");
            foreach (var t in tours)
            {
                var tourType = t.IsPrivate ? "Приватна" : "Групова";
                Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} ({tourType}) - {t.Price} грн");
            }
        }

        static void ShowPrivateTours()
        {
            var tours = _unitOfWork.Tours.GetPrivateTours();

            Console.WriteLine("\nПриватні екскурсії:");
            foreach (var t in tours)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(t.ExhibitionId);
                Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} - {exhibition.Title} ({t.Price} грн)");
            }
        }

        static void ShowScheduledTours()
        {
            var tours = _unitOfWork.Tours.GetScheduledTours();

            Console.WriteLine("\nЗаплановані екскурсії:");
            foreach (var t in tours)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(t.ExhibitionId);
                var tourType = t.IsPrivate ? "Приватна" : "Групова";
                Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} - {exhibition.Title} ({tourType})");
            }
        }

        static void SearchToursByGuide()
        {
            Console.Write("\nВведіть ім'я або частину імені гіда: ");
            var guideName = Console.ReadLine();

            var tours = _unitOfWork.Tours.GetByGuideName(guideName);

            Console.WriteLine("\nРезультати пошуку:");
            foreach (var t in tours)
            {
                var exhibition = _unitOfWork.Exhibitions.GetById(t.ExhibitionId);
                var tourType = t.IsPrivate ? "Приватна" : "Групова";
                Console.WriteLine($"{t.TourDate.ToShortDateString()}: {t.GuideName} - {exhibition.Title} ({tourType})");
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
            var currentExhibitions = _unitOfWork.Exhibitions.GetCurrentExhibitions(DateTime.Now);

            Console.WriteLine("\nПоточні експозиції (на " + DateTime.Now.ToShortDateString() + "):");
            Console.WriteLine("Назва\t\tТема\tАудиторія\tЗакінчується");
            foreach (var ex in currentExhibitions)
            {
                Console.WriteLine($"{ex.Title}\t{ex.Theme}\t{ex.TargetAudience}\t{ex.EndDate.ToShortDateString()}");
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

            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate);
            var totalIncome = visits.Sum(v => v.TicketPrice);

            Console.WriteLine($"\nЗвіт по візитам за період {startDate.ToShortDateString()} - {endDate.ToShortDateString()}:");
            Console.WriteLine($"Кількість візитів: {visits.Count()}");
            Console.WriteLine($"Загальний дохід: {totalIncome} грн");

            var visitsByExhibition = visits
                .GroupBy(v => v.ExhibitionId)
                .Select(g => new {
                    Exhibition = _unitOfWork.Exhibitions.GetById(g.Key),
                    Count = g.Count(),
                    Income = g.Sum(v => v.TicketPrice)
                })
                .OrderByDescending(x => x.Count);

            Console.WriteLine("\nРозподіл по експозиціях:");
            foreach (var item in visitsByExhibition)
            {
                Console.WriteLine($"{item.Exhibition.Title}: {item.Count} візитів, {item.Income} грн");
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

            // Жадібне завантаження даних про екскурсії з інформацією про експозиції
            var tours = _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .ToList();

            var totalIncome = tours.Sum(t => t.Price);
            var privateToursCount = tours.Count(t => t.IsPrivate);
            var groupToursCount = tours.Count(t => !t.IsPrivate);

            Console.WriteLine($"\nЗвіт по екскурсіях за період {startDate.ToShortDateString()} - {endDate.ToShortDateString()}:");
            Console.WriteLine($"Загальний дохід: {totalIncome} грн");
            Console.WriteLine($"Кількість приватних екскурсій: {privateToursCount}");
            Console.WriteLine($"Кількість групових екскурсій: {groupToursCount}");

            var toursByGuide = tours
                .GroupBy(t => t.GuideName)
                .Select(g => new {
                    Guide = g.Key,
                    Count = g.Count(),
                    Income = g.Sum(t => t.Price)
                })
                .OrderByDescending(x => x.Income);

            Console.WriteLine("\nРозподіл по гідах:");
            foreach (var item in toursByGuide)
            {
                Console.WriteLine($"{item.Guide}: {item.Count} екскурсій, {item.Income} грн доходу");
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

            // Ліниве завантаження даних
            var visits = _unitOfWork.Visits.GetByDateRange(startDate, endDate);
            var tours = _unitOfWork.Tours.GetAll()
                .Where(t => t.TourDate >= startDate && t.TourDate <= endDate)
                .ToList();

            var exhibitionsPopularity = _unitOfWork.Exhibitions.GetAll()
                .Select(e => new
                {
                    Exhibition = e,
                    VisitsCount = visits.Count(v => v.ExhibitionId == e.ExhibitionId),
                    ToursCount = tours.Count(t => t.ExhibitionId == e.ExhibitionId)
                })
                .OrderByDescending(e => e.VisitsCount + e.ToursCount * 2); // Вага для екскурсій більша

            Console.WriteLine($"\nРейтинг популярності експозицій за період {startDate.ToShortDateString()} - {endDate.ToShortDateString()}:");
            Console.WriteLine("Місце\tНазва\t\tВізити\tЕкскурсії\tЗагальний бал");

            int place = 1;
            foreach (var item in exhibitionsPopularity)
            {
                var totalScore = item.VisitsCount + item.ToursCount * 2;
                Console.WriteLine($"{place++}\t{item.Exhibition.Title}\t{item.VisitsCount}\t{item.ToursCount}\t\t{totalScore}");
            }
        }

        #region Додаткові методи для роботи з візитами
        static void PlanIndividualVisit()
        {
            Console.WriteLine("\nПланування індивідуального візиту");

            ShowCurrentExhibitionsReport();

            Console.Write("\nВведіть ID експозиції: ");
            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            var exhibition = _unitOfWork.Exhibitions.GetById(exhibitionId);
            if (exhibition == null)
            {
                Console.WriteLine("Експозиція з таким ID не знайдена.");
                return;
            }

            // Перевірка чи експозиція дійсно поточна
            if (exhibition.StartDate > DateTime.Now || exhibition.EndDate < DateTime.Now)
            {
                Console.WriteLine("Ця експозиція не є активною на поточну дату.");
                return;
            }

            Console.Write("Введіть дату візиту (рррр-мм-дд): ");
            DateTime visitDate;
            while (!DateTime.TryParse(Console.ReadLine(), out visitDate) || visitDate < DateTime.Today)
            {
                Console.Write("Невірний формат дати або дата в минулому. Спробуйте ще раз (рррр-мм-дд): ");
            }

            // Отримання доступних часових слотів
            var availableSlots = _unitOfWork.Schedules.GetAvailableSchedules(visitDate)
                .Where(s => s.ExhibitionId == exhibitionId)
                .ToList();

            if (!availableSlots.Any())
            {
                Console.WriteLine("На жаль, на обрану дату немає доступних часових слотів для цієї експозиції.");
                return;
            }

            Console.WriteLine("\nДоступні часові слоти:");
            for (int i = 0; i < availableSlots.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableSlots[i].StartTime} - {availableSlots[i].EndTime}");
            }

            Console.Write("Оберіть номер слота: ");
            if (!int.TryParse(Console.ReadLine(), out int slotNumber) || slotNumber < 1 || slotNumber > availableSlots.Count)
            {
                Console.WriteLine("Невірний вибір слота.");
                return;
            }

            var selectedSlot = availableSlots[slotNumber - 1];

            Console.Write("Ім'я відвідувача: ");
            var visitorName = Console.ReadLine();

            Console.Write("Кількість відвідувачів: ");
            if (!int.TryParse(Console.ReadLine(), out int visitorsCount) || visitorsCount <= 0)
            {
                Console.WriteLine("Невірна кількість відвідувачів.");
                return;
            }

            // Розрахунок вартості (можна додати більш складну логіку)
            decimal basePrice = 50; // Базова ціна
            decimal totalPrice = visitorsCount * basePrice;

            // Запис візиту
            var visit = new Visit
            {
                VisitorName = visitorName,
                VisitDate = visitDate,
                TicketPrice = totalPrice,
                ExhibitionId = exhibitionId
            };

            _unitOfWork.Visits.Add(visit);
            _unitOfWork.SaveChanges();

            Console.WriteLine($"\nВізит успішно заплановано на {visitDate.ToShortDateString()} {selectedSlot.StartTime}-{selectedSlot.EndTime}");
            Console.WriteLine($"Загальна вартість: {totalPrice} грн");
        }
        #endregion

        #region Додаткові методи для роботи з екскурсіями
        static void PlanGroupTour()
        {
            Console.WriteLine("\nПланування групової екскурсії");

            ShowCurrentExhibitionsReport();

            Console.Write("\nВведіть ID експозиції: ");
            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            var exhibition = _unitOfWork.Exhibitions.GetById(exhibitionId);
            if (exhibition == null)
            {
                Console.WriteLine("Експозиція з таким ID не знайдена.");
                return;
            }

            Console.Write("Введіть дату екскурсії (рррр-мм-дд): ");
            DateTime tourDate;
            while (!DateTime.TryParse(Console.ReadLine(), out tourDate) || tourDate < DateTime.Today)
            {
                Console.Write("Невірний формат дати або дата в минулому. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Ім'я гіда: ");
            var guideName = Console.ReadLine();

            Console.Write("Кількість учасників: ");
            if (!int.TryParse(Console.ReadLine(), out int participantsCount) || participantsCount <= 0)
            {
                Console.WriteLine("Невірна кількість учасників.");
                return;
            }

            // Розрахунок вартості
            decimal basePricePerPerson = 30;
            decimal totalPrice = participantsCount * basePricePerPerson;

            // Запис екскурсії
            var tour = new Tour
            {
                TourDate = tourDate,
                GuideName = guideName,
                IsPrivate = false,
                Price = totalPrice,
                ExhibitionId = exhibitionId
            };

            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();

            Console.WriteLine($"\nГрупова екскурсія успішно запланована на {tourDate.ToShortDateString()}");
            Console.WriteLine($"Загальна вартість: {totalPrice} грн");
        }

        static void PlanPrivateTour()
        {
            Console.WriteLine("\nПланування приватної екскурсії");

            ShowCurrentExhibitionsReport();

            Console.Write("\nВведіть ID експозиції: ");
            if (!int.TryParse(Console.ReadLine(), out int exhibitionId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            var exhibition = _unitOfWork.Exhibitions.GetById(exhibitionId);
            if (exhibition == null)
            {
                Console.WriteLine("Експозиція з таким ID не знайдена.");
                return;
            }

            Console.Write("Введіть дату екскурсії (рррр-мм-дд): ");
            DateTime tourDate;
            while (!DateTime.TryParse(Console.ReadLine(), out tourDate) || tourDate < DateTime.Today)
            {
                Console.Write("Невірний формат дати або дата в минулому. Спробуйте ще раз (рррр-мм-дд): ");
            }

            Console.Write("Ім'я гіда: ");
            var guideName = Console.ReadLine();

            Console.Write("Тривалість екскурсії (години): ");
            if (!int.TryParse(Console.ReadLine(), out int duration) || duration <= 0)
            {
                Console.WriteLine("Невірна тривалість.");
                return;
            }

            // Розрахунок вартості
            decimal basePricePerHour = 200;
            decimal totalPrice = duration * basePricePerHour;

            // Запис екскурсії
            var tour = new Tour
            {
                TourDate = tourDate,
                GuideName = guideName,
                IsPrivate = true,
                Price = totalPrice,
                ExhibitionId = exhibitionId
            };

            _unitOfWork.Tours.Add(tour);
            _unitOfWork.SaveChanges();

            Console.WriteLine($"\nПриватна екскурсія успішно запланована на {tourDate.ToShortDateString()}");
            Console.WriteLine($"Загальна вартість: {totalPrice} грн");
        }
        #endregion
    }
}
#endregion

