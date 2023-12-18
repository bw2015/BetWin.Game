using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BetWin.Game.Lottery.Utils
{
    /// <summary>
    /// 数学扩展
    /// </summary>
    internal static class MathHelper
    {
        /// <summary>
        /// 乘法积
        /// </summary>
        public static decimal Multiplication(this IEnumerable<decimal> list)
        {
            decimal result = decimal.One;
            foreach (decimal item in list)
            {
                result *= item;
            }
            return result;
        }

        /// <summary>
        /// 阶乘
        /// 一个正整数的阶乘（factorial）是所有小于及等于该数的正整数的积，并且0的阶乘为1。自然数n的阶乘写作n!
        /// </summary>
        /// <param name="n">正整数</param>
        /// <returns></returns>
        public static int Factorial(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        /// <summary>
        /// 乘积
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static decimal Product(this IEnumerable<decimal> list)
        {
            decimal value = decimal.One;
            foreach (decimal num in list)
            {
                value *= num;
            }
            return value;
        }

        /// <summary>
        /// 组合数量
        /// 从n个不同的元素中，任取m（m≤n）个元素为一组
        /// </summary>
        /// <returns></returns>
        public static int Combination(int n, int m)
        {
            if (m > n) throw new Exception("发生错误,m>n");
            if (m == 0 || n == m || n == 0) return 1;
            return Factorial(n) / (Factorial(m) * Factorial(n - m));
        }

        /// <summary>
        /// 重复组合
        /// 从n个不同元素中可重复地选取m个元素。不管其顺序合成一组，称为从n个元素中取m个元素的可重复组合
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int CombinationWithRepetiton(int n, int m)
        {
            return Factorial(n + m - 1) / (Factorial(m) * Factorial(n - 1));
        }

        /// <summary>
        /// 获取排列组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> Combination<T>(T[] items, int length)
        {
            T[] result = new T[length];
            return CombinationUtil(items, result);
        }

        private static IEnumerable<T[]> CombinationUtil<T>(T[] items, T[] result, int start = 0, int depth = 0)
        {
            for (int i = start; i < items.Length; i++)
            {
                result[depth] = items[i];
                if (depth == result.Length - 1)
                {
                    yield return result;
                }
                else
                {
                    foreach (T[] item in CombinationUtil(items, result, i + 1, depth + 1))
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// 取一个二维数组的交集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> Intersect<T>(this T[][] list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Length == 1) return list[0];
            IEnumerable<T> result = list[0];
            for (int index = 1; index < list.Length; index++)
            {
                result = result.Intersect(list[index]);
            }
            return result;
        }

        /// <summary>
        /// 概率算法
        /// </summary>
        /// <param name="numbers">号码的赔率</param>
        /// <returns></returns>
        public static string GetRandom(this Dictionary<string, double> numbers)
        {
            Random random = new Random();
            while (true)
            {
                // 随机排序
                foreach (KeyValuePair<string, double> item in numbers.OrderBy(t => Guid.NewGuid()))
                {
                    // 产生随机数
                    double rnd = random.NextDouble();

                    // 当前号码的概率
                    double probability = 1D / item.Value;

                    if (rnd < probability) return item.Key;
                }
            }


            throw new Exception("没有找到概率数字");
        }
    }
}
