namespace BetWin.Game.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            app.Run();
        }
    }
}