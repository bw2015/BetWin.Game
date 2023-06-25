using BetWin.Game.Lottery.Attributes;
using BetWin.Game.Lottery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Mark6
{
    /// <summary>
    /// 1.特码-双面
    /// 特码大小：开奖特码≥25为“大”，≤24为“小”，特码为49时为和局，退还本金。
    /// 特码单双：开奖特码个位数为1、3、5、7、9为“单”，0、2、4、6、8为“双”，特码为49时为和局，退还本金。
    /// 特合大小：开奖特码十位与个位之和≥7为“合大”，≤6为“合小”，特码为49时为和局，退还本金。举例：特码为07，投注「合大」，即中奖。
    /// 特合单双：开奖特码十位与个位之和的个位数为1、3、5、7、9为“合单”，0、2、4、6、8为“合双”，特码为49时为和局，退还本金。
    /// 特码：天、地肖：开奖特码属于十二生肖中的牛、兔、龙、马、猴、猪号码为“天肖”，鼠、虎、蛇、羊、鸡、狗号码为“地肖”，特码为49时为和局，退还本金。
    /// 特码：前、后肖：开奖特码属于十二生肖中的鼠、牛、虎、兔、龙、蛇号码为“前肖”，马、羊、猴、鸡、狗、猪号码为“后肖”，特码为49时为和局，退还本金。
    /// 特码：家、野肖：开奖特码属于十二生肖中的牛、马、羊、鸡、狗、猪号码为“家肖”，鼠、虎、龙、蛇、兔、猴号码为“野肖”，特码为49时为和局，退还本金。
    /// 特码尾数大小：开奖特码个位数≥5为“尾大”，≤4为“尾小”，特码为49时为和局，退还本金。
    /// 特码大小单双：开奖特码≥25为“大”，≤24为“小”，开奖特码个位数为1、3、5、7、9为“单”，0、2、4、6、8为“双”，特码为49时为和局，退还本金。
    /// 通过大小和单双组合产生「大单」，「小单」，「大双」和「小双」四种组合。
    /// 举例：开奖特码48，投注「大双」，即中奖。
    /// </summary>
    public class Sides2 : Mark6Base
    {
        protected new readonly string[] NUMBERS = new string[]
           {
                "Big","Small","Odd","Even","BigSum","SmallSum",
                "OddSum","EvenSum","ZodiacSky","ZodiacGround",
                "ZodiacFirst", "ZodiacLast","ZodiacPoultry","ZodiacBeast",
                "TailBig","TailSmall",
                "BigOdd","SmallOdd","BigEven","SmallEven"
           };

        protected override Odds DefaultOdds
        {
            get
            {
                Odds odds = new Odds();

                odds.Add("Big", 1.97M);
                odds.Add("Small", 1.97M);
                odds.Add("Odd", 1.97M);
                odds.Add("Even", 1.97M);

                odds.Add("BigSum", 1.97M);
                odds.Add("SmallSum", 1.97M);
                odds.Add("OddSum", 1.97M);
                odds.Add("EvenSum", 1.97M);

                odds.Add("ZodiacSky", 1.97M);
                odds.Add("ZodiacGround", 1.97M);
                odds.Add("ZodiacFirst", 1.97M);
                odds.Add("ZodiacLast", 1.97M);
                odds.Add("ZodiacPoultry", 1.97M);
                odds.Add("ZodiacBeast", 1.97M);

                odds.Add("TailBig", 1.97M);
                odds.Add("TailSmall", 1.97M);

                odds.Add("BigOdd", 3.94M);
                odds.Add("SmallOdd", 3.94M);
                odds.Add("BigEven", 3.94M);
                odds.Add("SmallEven", 3.94M);

                return odds;
            }
        }

        protected override RewardOdds CheckReward(string input, OpenNumber openNumber, Odds odds)
        {
            decimal rewardOdds = odds.GetOdds(input);
            // 特码
            string extraNumber = GetExtraNumber(openNumber),
                // 特码的生肖
                zodiac = GetZodiac(extraNumber, (DateTime)openNumber);

            int num = int.Parse(extraNumber),
                tenPlace = num / 10,
                onePlace = num % 10;

            switch (input)
            {
                // 特码大小：开奖特码≥25为“大”，≤24为“小”，特码为49时为和局，退还本金。
                case "Big":
                    rewardOdds = num == 49 ? 1M : num >= 25 ? rewardOdds : 0M;
                    break;
                case "Small":
                    rewardOdds = num == 49 ? 1M : num <= 24 ? rewardOdds : 0M;
                    break;
                //特码单双：开奖特码个位数为1、3、5、7、9为“单”，0、2、4、6、8为“双”，特码为49时为和局，退还本金。
                case "Odd":
                    rewardOdds = num == 49 ? 1M : num % 2 == 1 ? rewardOdds : 0M;
                    break;
                case "Even":
                    rewardOdds = num == 49 ? 1M : num % 2 == 0 ? rewardOdds : 0M;
                    break;
                // 特合大小：开奖特码十位与个位之和≥7为“合大”，≤6为“合小”，特码为49时为和局，退还本金。举例：特码为 07，投注「合大」，即中奖。
                case "BigSum":
                    rewardOdds = num == 49 ? 1M : tenPlace + onePlace >= 7 ? rewardOdds : 0M;
                    break;
                case "SmallSum":
                    rewardOdds = num == 49 ? 1M : tenPlace + onePlace <= 6 ? rewardOdds : 0M;
                    break;
                // 特合单双：开奖特码十位与个位之和的个位数为1、3、5、7、9为“合单”，0、2、4、6、8为“合双”，特码为49时为和局，退还本金。
                case "OddSum":
                    rewardOdds = num == 49 ? 1M : (tenPlace + onePlace) % 2 == 1 ? rewardOdds : 0M;
                    break;
                case "EvenSum":
                    rewardOdds = num == 49 ? 1M : (tenPlace + onePlace) % 2 == 0 ? rewardOdds : 0M;
                    break;
                // 特码：天、地肖：开奖特码属于十二生肖中的牛、兔、龙、马、猴、猪号码为“天肖”，鼠、虎、蛇、羊、鸡、狗号码为“地肖”，特码为49时为和局，退还本金。
                case "ZodiacSky":
                    rewardOdds = num == 49 ? 1M : new[] { "Ox", "Rabbit", "Dragon", "Horse", "Monkey", "Pig" }.Contains(zodiac) ? rewardOdds : 0M;
                    break;
                case "ZodiacGround":
                    rewardOdds = num == 49 ? 1M : new[] { "Rat", "Tiger", "Snake", "Goat", "Horse", "Dog" }.Contains(zodiac) ? rewardOdds : 0M;
                    break;
                // 特码：前、后肖：开奖特码属于十二生肖中的鼠、牛、虎、兔、龙、蛇号码为“前肖”，马、羊、猴、鸡、狗、猪号码为“后肖”，特码为49时为和局，退还本金。
                case "ZodiacFirst":
                    rewardOdds = num == 49 ? 1M : new[] { "Rat", "Ox", "Tiger", "Rabbit", "Dragon", "Snake" }.Contains(zodiac) ? rewardOdds : 0M;
                    break;
                case "ZodiacLast":
                    rewardOdds = num == 49 ? 1M : new[] { "Horse", "Goat", "Monkey", "Rooster", "Dog", "Pig" }.Contains(zodiac) ? rewardOdds : 0M;
                    break;
                // 特码：家、野肖：开奖特码属于十二生肖中的牛、马、羊、鸡、狗、猪号码为“家肖”，鼠、虎、龙、蛇、兔、猴号码为“野肖”，特码为49时为和局，退还本金。
                case "ZodiacPoultry":
                    rewardOdds = num == 49 ? 1M : new[] { "Ox", "Rooster", "Goat", "Rooster", "Dog", "Horse" }.Contains(zodiac) ? rewardOdds : 0M;
                    break;
                case "ZodiacBeast":
                    rewardOdds = num == 49 ? 1M : new[] { "Rat", "Tiger", "Dragon", "Snake", "Rabbit", "Monkey" }.Contains(zodiac) ? rewardOdds : 0M;
                    break;
                // 特码尾数大小：开奖特码个位数≥5为“尾大”，≤4为“尾小”，特码为49时为和局，退还本金。
                case "TailBig":
                    rewardOdds = num == 49 ? 1M : num % 10 >= 5 ? rewardOdds : 0M;
                    break;
                case "TailSmall":
                    rewardOdds = num == 49 ? 1M : num % 10 <= 4 ? rewardOdds : 0M;
                    break;
                // 特码大小单双：开奖特码≥25为“大”，≤24为“小”，开奖特码个位数为1、3、5、7、9为“单”，0、2、4、6、8为“双”，特码为49时为和局，退还本金。
                case "BigOdd":
                    rewardOdds = num == 49 ? 1M : num % 2 == 1 && num >= 25 ? rewardOdds : 0M;
                    break;
                case "SmallOdd":
                    rewardOdds = num == 49 ? 1M : num % 2 == 1 && num <= 24 ? rewardOdds : 0M;
                    break;
                case "BigEven":
                    rewardOdds = num == 49 ? 1M : num % 2 == 0 && num >= 25 ? rewardOdds : 0M;
                    break;
                case "SmallEven":
                    rewardOdds = num == 49 ? 1M : num % 2 == 0 && num <= 24 ? rewardOdds : 0M;
                    break;
            }

            return rewardOdds;
        }
    }
}
