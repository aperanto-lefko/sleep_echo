using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using sleepApp.Service;
using sleepApp.ServiceProvider;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace sleepApp
{
    public partial class AuthApp : Window
    {
        public AuthApp()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) //обработчик события кнопки войти
        {
            string login = LoginTextBox.Text; //логин из textBox
            string password = PasswordBox.Visibility == Visibility.Visible ? PasswordBox.Password : VisiblePasswordBox.Text; //пароль из PasswordBox
            string port = PortTextBox.Text;
            string dataBase = DatabaseTextBox.Text;
            string host = HostTextBox.Text;

            try
            {
                //Настраиваем DI контейнер с полученными данными
                var serviceProvider = ServiceProviderFactory.ConfigureServices(login, password, port, dataBase, host);
                var context = serviceProvider.GetRequiredService<AppDbContext>();
                // Проверяем подключение к базе данных
                bool isConnect = context.Database.CanConnect();
                if (isConnect)
                {
                    // Если подключение успешно, открываем DashboardWindow
                    DashboardWindow dashBoardWindow = serviceProvider.GetRequiredService<DashboardWindow>();
                    dashBoardWindow.Show();
                    this.Close();
                }
                else
                {
                    HighlightIfError(LoginTextBox, true);
                    HighlightIfError(PasswordBox, true);
                    HighlightIfError(VisiblePasswordBox, true);
                    HighlightIfError(PortTextBox, true);
                    HighlightIfError(DatabaseTextBox, true);
                    HighlightIfError(HostTextBox, true);
                    MessageBox.Show($"Неверные входные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HighlightIfError(Control control, bool isError) //подсветка в случае ошибки
        {
            control.BorderBrush = isError ? Brushes.Red : SystemColors.ControlDarkBrush; //цвет рамки
            control.BorderThickness = isError ? new Thickness(2) : new Thickness(1); //толщина
            ResetTextBoxBorderAfterDelay(control, 700);
        }

        private void ResetTextBoxBorderAfterDelay(Control control, int milliSeconds) //сбрасывание цвета через опред.время
        {
            Task.Delay(milliSeconds).ContinueWith(_ => //создает задачу, которая завершится через указанное количество миллисекунд
            //Это асинхронная операция, которая не блокирует основной поток приложения.
            {
                control.Dispatcher.Invoke(() => // используется для выполнения кода в потоке, который владеет объектом textBox
                {
                    control.BorderBrush = SystemColors.ControlDarkBrush;
                    control.BorderThickness = new Thickness(1);
                });
            });
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            HighlightIfError(PasswordBox, false);
            HighlightIfError(VisiblePasswordBox, false);
            // Переключаем видимость пароля
            // Проверяем, видимо ли поле PasswordBox
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Если PasswordBox видимо, значит, пароль сейчас скрыт.
                // Копируем текст из PasswordBox в VisiblePasswordBox.
                VisiblePasswordBox.Text = PasswordBox.Password;
                // Делаем VisiblePasswordBox видимым, чтобы показать пароль.
                VisiblePasswordBox.Visibility = Visibility.Visible;
                // Скрываем PasswordBox, так как пароль теперь виден в VisiblePasswordBox
                PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Если PasswordBox не видимо, значит, пароль сейчас показан в VisiblePasswordBox.
                // Мы хотим скрыть пароль.
                // Копируем текст из VisiblePasswordBox обратно в PasswordBox.
                PasswordBox.Password = VisiblePasswordBox.Text;
                // Делаем PasswordBox видимым, чтобы снова скрыть пароль.
                PasswordBox.Visibility = Visibility.Visible;
                // Скрываем VisiblePasswordBox, так как пароль теперь снова скрыт.
                VisiblePasswordBox.Visibility = Visibility.Collapsed;
            }
        }

        private void AdvancedSettingsExpander_Expanded(object sender, RoutedEventArgs e)
        {
            // Увеличиваем высоту окна при раскрытии Expander
            this.Height = 520; // Новая высота окна
        }

        private void AdvancedSettingsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            // Возвращаем исходную высоту окна при сворачивании Expander
            this.Height = 340; // Исходная высота окна
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
        private void ChangeColorTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFFFF");
        }
    }
}