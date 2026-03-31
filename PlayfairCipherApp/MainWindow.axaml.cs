using System;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PlayfairCipherApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCipher(cipher => cipher.Encrypt(InputText.Text),
                "Шифрование выполнено успешно");
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            ExecuteCipher(cipher => cipher.Decrypt(InputText.Text),
                "Дешифрование выполнено успешно");
        }

        private void ShowMatrix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cipher = new PlayfairCipher(KeyInput.Text);
                var matrix = cipher.GetMatrix();

                MatrixDisplay.Text = FormatMatrix(matrix);
                StatusText.Text = "Матрица 5×5 построена успешно.";
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Универсальный метод для шифрования/дешифрования
        /// </summary>
        private void ExecuteCipher(Func<PlayfairCipher, string> action, string successMessage)
        {
            try
            {
                var cipher = new PlayfairCipher(KeyInput.Text);
                var result = action(cipher);

                OutputText.Text = result;
                StatusText.Text = $"{successMessage}. Длина результата: {result.Length} символов.";
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Форматирование матрицы 5×5
        /// </summary>
        private string FormatMatrix(char[,] matrix)
        {
            var sb = new StringBuilder();

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    sb.Append(matrix[row, col]);
                    if (col < 4) sb.Append("   ");
                }

                if (row < 4)
                    sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Универсальный обработчик ошибок
        /// </summary>
        private async void ShowError(Exception ex)
        {
            string message = ex switch
            {
                ArgumentNullException => $"Ошибка: {ex.Message}",
                ArgumentException => $"Ошибка валидации: {ex.Message}",
                _ => $"Непредвиденная ошибка: {ex.Message}"
            };

            StatusText.Text = message;

            // Простой диалог (замена MessageBox)
            var dialog = new Window
            {
                Title = "Ошибка",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = message,
                    Margin = new Avalonia.Thickness(10),
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                }
            };

            await dialog.ShowDialog(this);
        }
    }
}