using lab4.Models;
using Microsoft.EntityFrameworkCore;

namespace lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var options = new DbContextOptionsBuilder<MuseumContext>()
                .UseInMemoryDatabase(databaseName: "MuseumDB")
                .Options;

            using var context = new MuseumContext(options);
            var service = new ExhibitionService(context);

            while (true)
            {
                Console.WriteLine("1. Додати експозицію\n2. Показати всі\n3. Видалити\n4. Вийти");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Назва: "); var title = Console.ReadLine();
                    Console.Write("Тема: "); var theme = Console.ReadLine();
                    Console.Write("Цільова аудиторія: "); var target = Console.ReadLine();

                    service.Add(new Exhibition
                    {
                        Title = title,
                        Theme = theme,
                        TargetAudience = target,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(10)
                    });
                }
                else if (choice == "2")
                {
                    var exhibitions = service.GetAll();
                    foreach (var ex in exhibitions)
                        Console.WriteLine($"{ex.ExhibitionId}: {ex.Title} - {ex.Theme}");
                }
                else if (choice == "3")
                {
                    Console.Write("ID експозиції: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                        service.Delete(id);
                }
                else break;
            }
        }
    }

}
