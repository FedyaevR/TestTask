using System;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public struct LetterStats : IComparable<LetterStats>
    {
        /// <summary>
        /// Буква/Пара букв для учёта статистики.
        /// </summary>
        public string Letter;

        /// <summary>
        /// Кол-во вхождений буквы/пары.
        /// </summary>
        public int Count;

        /// <summary>
        /// Флаг обозночающий, было ли вхождение
        /// </summary>
        public bool IsExist;

        public int CompareTo(LetterStats other)
        {
            return string.Compare(Letter, other.Letter);
        }
    }
}
