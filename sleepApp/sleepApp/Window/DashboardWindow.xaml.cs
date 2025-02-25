using sleepApp.Model;
using sleepApp.Repository;
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
        private List<Respondent> _allRespondents; //все пользователи из базы данных
        private int _currentPage = 1; //текущая страница
        private int _pageSize = 10; //количество записей на странице
        private readonly RespondentRepository _respondentRepository;


        public DashboardWindow(string login, string password)
        {
             InitializeComponent();
            _respondentRepository = new RespondentRepository(login, password);
        }

                
        private void GetAllUsersButton_Click(object sender, RoutedEventArgs e) //поиск всех пользователей
        {
            _currentPage = 1;
            _allRespondents = _respondentRepository.GetAllRespondents();
            LoadPage(_currentPage); //загружаем первую страницу
        }

        private void GetUserByName_Click(object sender, RoutedEventArgs e)
        {
            string name = RespondentLastNameTextBox.Text;
            if (name != null)
            {
                _allRespondents = _respondentRepository.GetRespondentByLastName(name);
                _currentPage = 1;
                LoadPage(_currentPage);
            } else
            {
                RespondentLastNameTextBox.BorderBrush=Brushes.Red;
                RespondentLastNameTextBox.BorderThickness = new Thickness(2);
            }
        }

        private void LoadPage(int page)
        {
            var usersToDisplay = _allRespondents
            .Skip((page - 1) * _pageSize) //Пропускает указанное количество элементов
            .Take(_pageSize) //Берет следующее количество элементов
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
            if (_currentPage < (_allRespondents.Count / _pageSize) + 1)
            {
                _currentPage++;
                LoadPage(_currentPage);
            }
        }
    }
}
