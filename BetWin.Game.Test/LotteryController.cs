using Microsoft.AspNetCore.Mvc;
using SP.StudioCore.Array;

namespace BetWin.Game.Test
{
    [Route("[controller]/[action]"), ApiController]
    public class LotteryController : ControllerBase
    {
        private string GetRandom(Dictionary<string, double> numbers)
        {
            double sum = numbers.Sum(t => t.Value);
            Random random = new Random();
            double randomNumber = random.NextDouble() * sum; // 生成0到1之间的随机数

            double probability = 0D;
            foreach (var item in numbers)
            {
                string number = item.Key;
                double value = item.Value;

                probability += value;
                if (randomNumber <= probability) return number;
            }

            throw new Exception("没有找到概率数字");
        }

        /// <summary>
        /// 概率算法
        /// </summary>
        /// <returns></returns>
        public IActionResult Probability()
        {
            Dictionary<string, double> numbers = new Dictionary<string, double>()
            {
                { "Mercury",15 },
                { "Venus",25 },
                { "Earth",5 },
                { "Mars",5 },
                { "Jupiter",15 },
                { "Saturn",5 },
                { "Uranus",25 },
                { "Neptune",5 }
            }.ToDictionary(t => t.Key, t => 1D / t.Value);

            Dictionary<string, int> count = new Dictionary<string, int>();
            int total = 1000;
            for (int i = 0; i < total; i++)
            {
                string number = this.GetRandom(numbers);
                if (!count.ContainsKey(number)) count.Add(number, 0);
                count[number]++;
            }

            return new JsonResult(new
            {
                numbers,
                probability = count.ToDictionary(t => t.Key, t => (double)t.Value / (double)total)
            });
        }
    }
}
