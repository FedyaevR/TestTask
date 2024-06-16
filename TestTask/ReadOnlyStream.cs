using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;

            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new StreamReader(fileFullPath);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            var pattern = @"[a-zA-Z]";
            
            if (!IsEof)
            {
                var result = (char)_localStream.Read();
                IsEof = _localStream.EndOfStream;
                
                if (!Regex.IsMatch(result.ToString(), pattern))
                {
                    throw new FormatException();
                }
                
                return result;
            }

            throw new EndOfStreamException();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                
                return;
            }
            
            _localStream.DiscardBufferedData();
            _localStream.BaseStream.Seek(0, SeekOrigin.Begin);
            IsEof = false;
        }

        public void Dispose()
        {
            _localStream?.Dispose();
        }
    }
}
