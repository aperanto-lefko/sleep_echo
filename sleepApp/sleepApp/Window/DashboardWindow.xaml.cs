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

namespace sleepApp
{

    public partial class DashboardWindow : Window
    {
        private List<Respondent> _allRespondents; //все пользователи из базы данных
        private int _currentPage = 1; //текущая страница
        private int _pageSize = 10; //количество записей на странице
        private readonly RespondentRepository _respondentRepository;
        private readonly IMapper _mapper;



        public DashboardWindow(string login, string password)
        {
            InitializeComponent();
            _respondentRepository = new RespondentRepository(login, password);
            var config = new MapperConfiguration(config => config.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }


        private void GetAllUsersButton_Click(object sender, RoutedEventArgs e) //поиск всех пользователей
        {
            _currentPage = 1;
            _allRespondents = _respondentRepository.GetAllRespondents();
            MessageBox.Show($"Загружено {_allRespondents.Count} пользователей."); // Отладочное сообщение
            LoadPage(_currentPage); //загружаем первую страницу
        }

        private void GetUserByName_Click(object sender, RoutedEventArgs e)
        {
            string name = RespondentLastNameTextBox.Text;
            if (name != null)
            {
                _allRespondents = _respondentRepository.GetRespondentByLastName(name);
                MessageBox.Show($"Найдено {_allRespondents.Count} пользователей.");
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
            RespondentDto resp = GetNewRespondent();
            if (resp != null)
            {
                Respondent newResp = _mapper.Map<Respondent>(resp);
                newResp = _respondentRepository.AddRespondent(newResp);
                MessageBox.Show($"Добавлен новый респондент {newResp}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void CheckNewUser()
        {

        }



        private void DeleteRespondent_Click(object sender, RoutedEventArgs e)
        {

        }

        private RespondentDto GetNewRespondent()
        {
            try
            {
                return new RespondentDto(NewRespondentFirstNameTextBox.Text,
                    NewRespondentLastNameTextBox.Text,
                    NewRespondentEmailTextBox.Text,
                    NewRespondentGenderComboBox.Text,
                    NewRespondentCountryTextBox.Text,
                    int.Parse(NewRespondentAgeTextBox.Text));
            }
            catch (ValidationException e)
            {
                MessageBox.Show(e.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат возраста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Неверный формат возраста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ValidationTextFields(params string[] fields)
        {
            foreach (var field in fields)
            {
                if (ContainsDigit(field))
                {
                    throw new ArgumentException("Текстовые поля не должны содержать цифр");
                }
            }
        }

        private bool ContainsDigit(string field)
        {
            return field.Any(char.IsDigit); //проверка содержит ли цифры
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
