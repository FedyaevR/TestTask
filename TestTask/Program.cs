using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            try
            {
                using var inputStream1 = GetInputStream(args[0]);
                using var inputStream2 = GetInputStream(args[1]);

                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            catch (FormatException exception)
            {
                Console.WriteLine(exception);
            }
            catch (EndOfStreamException exception)
            {
                Console.WriteLine(exception);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
            Console.Read();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var result = new List<LetterStats>();
            
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char nextChar = stream.ReadNextChar();
                var currentLetter = result.FirstOrDefault(e => e.Letter == nextChar.ToString());
                
                IncStatistic(ref currentLetter, nextChar);
                
                result.Add(currentLetter);
            }

            return result;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var result = new List<LetterStats>();
            var symbolPairs = string.Empty;
            
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char nextChar = stream.ReadNextChar();
                symbolPairs += nextChar.ToString().ToLower();
                
                if (symbolPairs.Length == 2)
                {
                    if (symbolPairs[0] == symbolPairs[1])
                    {
                        var currentLetter = result.FirstOrDefault(e => e.Letter.ToLower() == symbolPairs);
                        IncStatistic(ref currentLetter, symbolPairs);
                        
                        result.Add(currentLetter);
                    }
                    
                    symbolPairs = string.Empty;
                }
            }

            return result;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
          

            var result = default(IList<LetterStats>);
            
            switch (charType)
            {
                case CharType.Consonants:
                    result = letters.ToList()
                                    .FindAll(IsConsonant);
                    break;
                case CharType.Vowel:
                    result = letters.ToList()
                                    .FindAll(IsVowel);
                    break;
            }

            foreach (var letter in result)
            {
                letters.Remove(letter);
            }
        }

        private static bool IsConsonant(LetterStats letter)
        {
            var pattern = @"[bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ]";
            
            return Regex.IsMatch(letter.Letter, pattern);
        }

        private static bool IsVowel(LetterStats letter)
        {
            var pattern = @"[aeiouAEIOU]";
            
            return Regex.IsMatch(letter.Letter, pattern);
        }
        
        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var summaryCount = 0;
            
            letters.ToList().Sort();
            
            foreach (var letter in letters)
            {
                summaryCount += letter.Count;
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
            }
            
            Console.WriteLine($"ИТОГО: {summaryCount}");
        }
        

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре с одним символом.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats, char nextChar)
        {
            if (!letterStats.IsExist)
            {
                letterStats.Letter = nextChar.ToString();
                letterStats.IsExist = true;
            }
            
            letterStats.Count++;
        }
        
        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре с парой символов.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats, string nextSymbolPairs)
        {
            if (!letterStats.IsExist)
            {
                letterStats.Letter = nextSymbolPairs;
                letterStats.IsExist = true;
            }
            
            letterStats.Count++;
        }
    }
}
