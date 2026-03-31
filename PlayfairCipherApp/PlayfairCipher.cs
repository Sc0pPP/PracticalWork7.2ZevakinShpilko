using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayfairCipherApp
{
    /// <summary>
    /// Реализация шифра Плейфера с матрицей 5×5.
    /// Буквы I и J объединяются в одну позицию матрицы.
    /// </summary>
    public class PlayfairCipher
    {
        /// <summary>
        /// Матрица 5×5 для шифрования.
        /// </summary>
        private readonly char[,] _matrix = new char[5, 5];

        /// <summary>
        /// Словарь позиций каждой буквы в матрице (строка, столбец).
        /// </summary>
        private readonly Dictionary<char, (int row, int col)> _positions = new();

        /// <summary>
        /// Символ-заполнитель при совпадении букв в биграмме или нечётной длине.
        /// </summary>
        private const char Filler = 'X';

        /// <summary>
        /// Создаёт экземпляр шифра Плейфера с указанным ключом.
        /// </summary>
        /// <param name="key">Ключ шифрования (латинские буквы).</param>
        /// <exception cref="ArgumentNullException">Если ключ равен null.</exception>
        /// <exception cref="ArgumentException">Если ключ пустой или не содержит букв.</exception>
        public PlayfairCipher(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Ключ не может быть null.");
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Ключ не может быть пустым.", nameof(key));

            BuildMatrix(key.ToUpper());
        }

        /// <summary>
        /// Строит матрицу 5×5 из ключа, удаляя повторяющиеся буквы
        /// и дополняя оставшимися буквами алфавита.
        /// </summary>
        /// <param name="key">Ключ в верхнем регистре.</param>
        private void BuildMatrix(string key)
        {
            var used = new HashSet<char>();
            var letters = new List<char>();

            // Добавляем буквы из ключа (J заменяем на I)
            foreach (char c in key.Where(char.IsLetter))
            {
                char ch = (c == 'J') ? 'I' : c;
                if (used.Add(ch))
                    letters.Add(ch);
            }

            // Дополняем оставшимися буквами алфавита (без J)
            for (char c = 'A'; c <= 'Z'; c++)
            {
                if (c == 'J') continue;
                if (used.Add(c))
                    letters.Add(c);
            }

            // Заполняем матрицу и сохраняем позиции
            for (int i = 0; i < 25; i++)
            {
                int row = i / 5;
                int col = i % 5;
                _matrix[row, col] = letters[i];
                _positions[letters[i]] = (row, col);
            }
        }

        /// <summary>
        /// Шифрует текст шифром Плейфера.
        /// </summary>
        /// <param name="text">Открытый текст (латинские буквы).</param>
        /// <returns>Зашифрованный текст.</returns>
        /// <exception cref="ArgumentNullException">Если текст равен null.</exception>
        /// <exception cref="ArgumentException">Если текст пустой.</exception>
        public string Encrypt(string text)
        {
            ValidateText(text);
            var bigrams = PrepareBigrams(text.ToUpper());
            return ProcessBigrams(bigrams, encrypt: true);
        }

        /// <summary>
        /// Дешифрует текст шифром Плейфера.
        /// </summary>
        /// <param name="cipher">Зашифрованный текст (латинские буквы).</param>
        /// <returns>Расшифрованный текст.</returns>
        /// <exception cref="ArgumentNullException">Если текст равен null.</exception>
        /// <exception cref="ArgumentException">Если текст пустой.</exception>
        public string Decrypt(string cipher)
        {
            ValidateText(cipher);

            // Для дешифрования берём текст как есть (попарно)
            var prepared = cipher.ToUpper()
                .Where(char.IsLetter)
                .Select(c => (c == 'J') ? 'I' : c)
                .ToList();

            var bigrams = new List<(char, char)>();
            for (int i = 0; i < prepared.Count - 1; i += 2)
                bigrams.Add((prepared[i], prepared[i + 1]));

            return ProcessBigrams(bigrams, encrypt: false);
        }

        /// <summary>
        /// Возвращает копию матрицы 5×5 для отображения в интерфейсе.
        /// </summary>
        /// <returns>Двумерный массив символов матрицы.</returns>
        public char[,] GetMatrix() => (char[,])_matrix.Clone();

        /// <summary>
        /// Проверяет входной текст на корректность.
        /// </summary>
        /// <param name="text">Текст для проверки.</param>
        private static void ValidateText(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text), "Текст не может быть null.");
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Текст не может быть пустым.", nameof(text));
        }

        /// <summary>
        /// Разбивает текст на биграммы. Если две одинаковые буквы
        /// оказываются в одной паре, вставляется символ-заполнитель 'X'.
        /// При нечётной длине добавляется 'X' в конец.
        /// </summary>
        /// <param name="text">Текст в верхнем регистре.</param>
        /// <returns>Список биграмм.</returns>
        private List<(char, char)> PrepareBigrams(string text)
        {
            var clean = text.Where(char.IsLetter)
                           .Select(c => (c == 'J') ? 'I' : c)
                           .ToList();

            var bigrams = new List<(char, char)>();
            int i = 0;

            while (i < clean.Count)
            {
                char first = clean[i];

                if (i + 1 < clean.Count)
                {
                    char second = clean[i + 1];
                    if (first == second)
                    {
                        // Одинаковые буквы — вставляем заполнитель
                        bigrams.Add((first, Filler));
                        i += 1;
                    }
                    else
                    {
                        bigrams.Add((first, second));
                        i += 2;
                    }
                }
                else
                {
                    // Нечётное количество — дополняем заполнителем
                    bigrams.Add((first, Filler));
                    i += 1;
                }
            }

            return bigrams;
        }

        /// <summary>
        /// Обрабатывает список биграмм по правилам шифра Плейфера.
        /// </summary>
        /// <param name="bigrams">Список биграмм.</param>
        /// <param name="encrypt">true — шифрование, false — дешифрование.</param>
        /// <returns>Результирующая строка.</returns>
        private string ProcessBigrams(List<(char, char)> bigrams, bool encrypt)
        {
            int shift = encrypt ? 1 : -1;
            var sb = new StringBuilder();

            foreach (var (a, b) in bigrams)
            {
                var (ra, ca) = _positions[a];
                var (rb, cb) = _positions[b];

                if (ra == rb)
                {
                    // Одна строка — сдвигаем по столбцу
                    sb.Append(_matrix[ra, Mod(ca + shift, 5)]);
                    sb.Append(_matrix[rb, Mod(cb + shift, 5)]);
                }
                else if (ca == cb)
                {
                    // Один столбец — сдвигаем по строке
                    sb.Append(_matrix[Mod(ra + shift, 5), ca]);
                    sb.Append(_matrix[Mod(rb + shift, 5), cb]);
                }
                else
                {
                    // Прямоугольник — берём противоположные углы
                    sb.Append(_matrix[ra, cb]);
                    sb.Append(_matrix[rb, ca]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Вычисляет остаток от деления с корректной обработкой отрицательных чисел.
        /// </summary>
        private static int Mod(int a, int m) => ((a % m) + m) % m;
    }
}