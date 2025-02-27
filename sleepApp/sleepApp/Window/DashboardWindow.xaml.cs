using sleepApp.Model;
using sleepApp.Dto;
using sleepApp.Repository;
using sleepApp.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using AutoMapper;
using sleepApp.ExceptionType;

namespace sleepApp
{

    public partial class DashboardWindow : Window
    {
        private List<Respondent> _allRespondents; //все пользователи из базы данных
        private int _currentPage = 1; //текущая страница
        private int _pageSize = 10; //количество записей на странице
        private readonly RespodentService _rService;



        public DashboardWindow(string login, string password)
        {
            InitializeComponent();
            _rService = new RespodentService(login, password);
        }


        private void GetAllUsersButton_Click(object sender, RoutedEventArgs e) //поиск всех пользователей
        {
            _currentPage = 1;
            _allRespondents = _rService.GetAllRespondents();
            MessageBox.Show($"Загружено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information); // Отладочное сообщение
            LoadPage(_currentPage); //загружаем первую страницу
        }

        private void GetUserByName_Click(object sender, RoutedEventArgs e)
        {
            string name = RespondentLastNameTextBox.Text;
            if (name != null)
            {
                _allRespondents = _rService.GetRespondentByLastName(name);
                MessageBox.Show($"Найдено {_allRespondents.Count} пользователей.","Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _currentPage = 1;
                LoadPage(_currentPage);
            }
            else
            {
                RespondentLastNameTextBox.BorderBrush = Brushes.Red;
                RespondentLastNameTextBox.BorderThickness = new Thickness(2);
            }
        }

        private void AddRespondent_Click(object sender, RoutedEventArgs e)
        {
            Respondent newResp = _rService.AddRespondent(
                    NewRespondentFirstNameTextBox.Text,
                    NewRespondentLastNameTextBox.Text,
                    NewRespondentEmailTextBox.Text,
                    NewRespondentGenderComboBox.Text,
                    NewRespondentCountryTextBox.Text,
                    int.Parse(NewRespondentAgeTextBox.Text));
            if (newResp != null)
            {
                MessageBox.Show($"Добавлен новый респондент {newResp}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void DeleteRespondent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = int.Parse(DeletedRespondentIdTextBox.Text);
                int result = _rService.RemoveRespondentById(id);
                if (result > 0)
                {
                    MessageBox.Show($"Пользователь с id={id} удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void GetAllDataButton_Click(object sender, RoutedEventArgs e)
        {

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

        private void DashBoardTabControl_SelectionChanged(object sender,SelectionChangedEventArgs e) //изменение размера окна при переходе на закладку
        {
            this.Width = DashBoardTabControl.SelectedIndex == 1 ? 1000 : 800;
            this.Height = DashBoardTabControl.SelectedIndex == 1 ? 800 : 580;
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
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
             e.Handled = !int.TryParse(e.Text, out _);
         }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
