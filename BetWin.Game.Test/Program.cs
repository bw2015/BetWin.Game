using SP.StudioCore.Json;

namespace BetWin.Game.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("-test"))
            {
                var odds = new Dictionary<string, decimal>()
                {
                    { "1",15 },
                    { "2",25 },
                    { "3",5 },
                    { "4",5 },
                    { "5",15 },
                    { "6",5 },
                    { "7",25 },
                    { "8",5 }
                };


                decimal total = odds.Sum(t => t.Value);
                var data = odds.ToDictionary(t => t.Key, t => total / t.Value);

                string[] results = odds.Select(t => t.Key).ToArray();
                List<Tuple<string, decimal>> numbers = new List<Tuple<string, decimal>>();
                decimal p = 0M;
                int index = 0;
                foreach (var item in data)
                {
                    p += item.Value;
                    numbers.Add(new Tuple<string, decimal>(results[index], p));
                    index++;
                }

                Dictionary<string, int> result = new Dictionary<string, int>();
                for (int i = 0; i < 1000; i++)
                {
                    int random = new Random().Next((int)total);
                    string number = numbers.First(t => random < t.Item2).Item1;
                    if (result.ContainsKey(number))
                    {
                        result[number]++;
                    }
                    else
                    {
                        result.Add(number, 1);
                    }
                }
                Console.WriteLine(result.OrderBy(t => t.Key).ToJson());

                return;
            }

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddCors(opt => opt.AddPolicy("Api", policy =>
            {
                policy.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                policy.AllowAnyHeader();
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
            }));

            var app = builder.Build();

            app.UseStaticFiles()
                .UseRouting()
                .UseCors("Api")
                .UseEndpoints(endpoints => endpoints.MapControllers());

            app.Run();
        }
    }
}