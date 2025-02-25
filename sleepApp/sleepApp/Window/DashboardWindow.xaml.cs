using sleepApp.Model;
using sleepApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sleepApp
{

    public partial class DashboardWindow : Window
    {
        private readonly string login;
        private readonly string password;
        public DashboardWindow(string login, string password)
        {
            this.login = login;
            this.password = password;
            InitializeComponent();
        }

        private List<Respondent> _allUsers; //все пользователи из базы данных
        private int _currentPage = 1; //текущая страница
        private int _pageSize = 10; //количество записей на странице


        private List<Respondent> GetUsersFromDataBase()
        {
            using (var context = new AppDbContext(login, password))
            {

                //return context.respondents.ToList();
                var users = context.Respondents.OrderBy(r=> r.Id).ToList();
                MessageBox.Show($"Загружено {users.Count} пользователей."); // Отладочное сообщение
                return users;
            }
        }
        private void GetAllUsersButton_Click(object sender, RoutedEventArgs e)
        {
            _allUsers = GetUsersFromDataBase();
            LoadPage(_currentPage); //загружаем первую страницу
        }

        private void LoadPage(int page)
        {
            var usersToDisplay = _allUsers
            .Skip((page - 1) * _pageSize)
            .Take(_pageSize)
            .ToList();

            UserDataGrid.ItemsSource = usersToDisplay;
            PageNumberText.Text = $"Страница {page}";

        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadPage(_currentPage);
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < (_allUsers.Count / _pageSize) + 1)
            {
                _currentPage++;
                LoadPage(_currentPage);
            }
        }
    }
}
