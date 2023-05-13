using SP.StudioCore.Tools;

namespace BetWin.Game.Test
{
    public class Program : ToolsProgram
    {
        public static void Main(string[] args)
        {
            if (WebStartup<ToolsStartup>(args)) return;
        }


    }
}