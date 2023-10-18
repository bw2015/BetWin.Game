namespace BetWin.Game.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
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