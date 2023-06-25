using BetWin.Game.Lottery.Base;
using BetWin.Game.Lottery.Models;
using BetWin.Game.Lottery.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Mark6
{
    /// <summary>
    /// 六合彩玩法的基类
    /// </summary>
    public abstract class Mark6Base : LotteryBase
    {
        static readonly ChineseLunisolarCalendar chineseDate = new ChineseLunisolarCalendar();

        protected virtual string[] NUMBERS => new[] {
            "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
            "41", "42", "43", "44", "45", "46", "47", "48", "49"
        };

        /// <summary>
        /// 生肖
        /// </summary>
        protected readonly string[] ZODIAC = new[] { "Rat", "Ox", "Tiger", "Rabbit", "Dragon", "Snake", "Horse", "Goat", "Monkey", "Rooster", "Dog", "Pig" };

        /// <summary>
        /// 五行
        /// </summary>
        protected readonly string[] GOLD = new[] { "01", "02", "09", "10", "23", "24", "31", "32", "39", "40" };
        protected readonly string[] WOOD = new[] { "05", "06", "13", "14", "21", "22", "35", "36", "43", "44" };
        protected readonly string[] WATER = new[] { "11", "12", "19", "20", "27", "28", "41", "42", "49" };
        protected readonly string[] FIRE = new[] { "07", "08", "15", "16", "29", "30", "37", "38", "45", "46" };
        protected readonly string[] EARTH = new[] { "03", "04", "17", "18", "25", "26", "33", "34", "47", "48" };


        /// <summary>
        /// 波色
        /// </summary>
        protected readonly string[] RED = new[] { "01", "02", "07", "08", "12", "13", "18", "19", "23", "24", "29", "30", "34", "35", "40", "45", "46" };
        protected readonly string[] BLUE = new[] { "03", "04", "09", "10", "14", "15", "20", "25", "26", "31", "36", "37", "41", "42", "47", "48" };
        protected readonly string[] GREEN = new[] { "05", "06", "11", "16", "17", "21", "22", "27", "28", "32", "33", "38", "39", "43", "44", "49" };


        /// <summary>
        // 尾数
        /// </summary>
        protected readonly string[] TAIL = new[] { "0Tail", "1Tail", "2Tail", "3Tail", "4Tail", "5Tail", "6Tail", "7Tail", "8Tail", "9Tail" };

        /// <summary>
        /// 头数
        /// </summary>
        protected readonly string[] HEAD = new[] { "0Head", "1Head", "2Head", "3Head", "4Head" };

        protected override IEnumerable<BetContentTranslate> betContentTranslate => this.NUMBERS.Select(t => new BetContentTranslate(t));

        protected override bool CheckBetContent(string content)
        {
            return NUMBERS.Contains(content);
        }

        /// <summary>
        /// 检查数量
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        protected bool CheckBetContent(string content, int length)
        {
            string[] nums = content.Split(',');
            // 选择的号码小于2个或者有重复
            if (nums.Length < length || nums.Distinct().Count() != nums.Length) return false;
            if (nums.Any(t => !NUMBERS.Contains(t))) return false;
            return true;
        }

        /// <summary>
        /// 获取开奖号码的特码
        /// </summary>
        protected string GetExtraNumber(string openNumber)
        {
            return openNumber.Split(',').Last();
        }

        /// <summary>
        /// 获取正码
        /// </summary>
        /// <param name="openNumber"></param>
        /// <returns></returns>
        protected string[] GetBallNumber(string openNumber)
        {
            return openNumber.Split(',').Take(6).ToArray();
        }

        /// <summary>
        /// 获取号码的波色
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected string GetColor(string num)
        {
            if (RED.Contains(num)) return "Red";
            if (BLUE.Contains(num)) return "Blue";
            if (GREEN.Contains(num)) return "Green";
            return string.Empty;
        }

        /// <summary>
        /// 获取号码的五行
        /// </summary>
        protected string GetElement5(string num)
        {
            if (GOLD.Contains(num)) return "Gold";
            if (WOOD.Contains(num)) return "Wood";
            if (WATER.Contains(num)) return "Water";
            if (FIRE.Contains(num)) return "Fire";
            if (EARTH.Contains(num)) return "Earth";
            return string.Empty;
        }

        /// <summary>
        /// 获取号码的生肖
        /// </summary>
        /// <param name="num"></param>
        /// <param name="openTime">开奖时间（东八区）</param>
        /// <returns></returns>
        protected string GetZodiac(string num, DateTime? openTime)
        {
            openTime ??= DateTime.Now;

            // 得到当前时间的农历年份
            int lunaYear = chineseDate.GetYear(openTime.Value);

            // 得到农历年的生肖
            string zodiac = ZODIAC[(lunaYear - 4) % 12];

            int zodiacIndex = Array.IndexOf(ZODIAC, zodiac);
            int code = int.Parse(num);

            zodiacIndex = ((12 - (code - 1) % 12) % 12 + zodiacIndex) % 12;
            return ZODIAC[zodiacIndex];
        }

        /// <summary>
        /// 检查开奖号码是否正确
        /// </summary>
        protected override bool CheckOpenNumber(OpenNumber openNumber)
        {
            if (string.IsNullOrEmpty(openNumber)) return false;
            string[] numbers = ((string)openNumber).Split(',');
            if (numbers.Length != 7 || numbers.Distinct().Count() != 7) return false;
            if (numbers.Any(num => !NUMBERS.Contains(num))) return false;
            return true;
        }

        protected override int GetBetCountNumber(string input)
        {
            return 1;
        }

        /// <summary>
        /// 获取排列组合的数量
        /// </summary>
        protected int GetBetCountNumber(string input, int length)
        {
            string[] nums = input.Split(',');
            return MathHelper.Combination(nums.Length, length);
        }
    }
}
