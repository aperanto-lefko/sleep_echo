using sleepApp.Service;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using sleepApp.ServiceProvider;
using Microsoft.Extensions.DependencyInjection;


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


            using (var context = new AppDbContext(login, password))
            {
                try
                {
                    // Проверяем подключение к базе данных
                    bool isConnect = context.Database.CanConnect();
                    if (isConnect)
                    {
                        //Настраиваем DI контейнер с полученными данными
                        var serviceProvider = ServiceProviderFactory.ConfigureServices(login, password);
                        // Если подключение успешно, открываем DashboardWindow
                        DashboardWindow dashBoardWindow = serviceProvider.GetRequiredService<DashboardWindow>(); //получение 
                        dashBoardWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        HighlightIfError(LoginTextBox, true);
                        HighlightIfError(PasswordBox, true);
                        HighlightIfError(VisiblePasswordBox, true);
                        MessageBox.Show($"Неверная пара логин/пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        }

        private void HighlightIfError(Control control, bool isError) //подсветка в случае ошибки
        {
            control.BorderBrush = isError ? Brushes.Red : SystemColors.ControlDarkBrush; //цвет рамки
            control.BorderThickness = isError ? new Thickness(2) : new Thickness(1); //толщина


        }

        private void ResetHighlight(object sender, RoutedEventArgs e) //обработчик чтобы сбросить красный цвет после ошибочного ввода
        {
            if (sender is Control control)
            {
                HighlightIfError(control, false);
            }
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


    }
}
