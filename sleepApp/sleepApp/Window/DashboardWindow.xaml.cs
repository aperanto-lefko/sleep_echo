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
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Npgsql;
using System.Xml.Linq;

namespace sleepApp
{

    public partial class DashboardWindow : Window
    {
        private List<Respondent> _allRespondents; //все пользователи из базы данных
        private int _currentPage = 1; //текущая страница
        private int _pageSize = 10; //количество записей на странице
        private readonly RespodentService _rService;
        private readonly SleepDataService _slService;



        public DashboardWindow(string login, string password)
        {
            InitializeComponent();
            _rService = new RespodentService(login, password);
            _slService = new SleepDataService(login, password);
        }


        private void GetAllUsersButton_Click(object sender, RoutedEventArgs e) //поиск всех пользователей
        {
            try
            {
                _currentPage = 1;
                _allRespondents = _rService.GetAllRespondents();
                MessageBox.Show($"Загружено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information); // Отладочное сообщение
                LoadPage(_currentPage); //загружаем первую страницу
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }


        private void GetUserByName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = RespondentLastNameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    _allRespondents = _rService.GetRespondentByLastName(name);
                    MessageBox.Show($"Найдено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    _currentPage = 1;
                    LoadPage(_currentPage);
                }
                else
                {
                    HighlightTextBox(RespondentLastNameTextBox);
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddRespondent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RespondentDto newResp = _rService.AddRespondent(
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
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (FormatException)
            {
                MessageBox.Show("Неправильный формат числа, поле не должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }


        private void DeleteRespondent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(DeletedRespondentIdTextBox.Text))
                {
                    int id = int.Parse(DeletedRespondentIdTextBox.Text);
                    if (_rService.RemoveRespondentById(id))
                    {
                        MessageBox.Show($"Пользователь с id={id} удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    HighlightTextBox(DeletedRespondentIdTextBox);
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void GetAllDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int respondentId = ParseIntOrNull(RespondentIDTextBox.Text);
                double slStartTimeStart = ParseDoubleOrNull(SlStartTimeTextBox_start.Text);
                double slStartTimeEnd = ParseDoubleOrNull(SlStartTimeTextBox_end.Text);
                double slEndTimeStart = ParseDoubleOrNull(SlEndTimeTextBox_start.Text);
                double slEndTimeEnd = ParseDoubleOrNull(SlEndTimeTextBox_start_end.Text);
                double slTotalTimeStart = ParseDoubleOrNull(SlTotalTimeTextBox_start.Text);
                double slTotalTimeEnd = ParseDoubleOrNull(SlTotalTimeTextBox_end.Text);
                int slQualityStart = ParseIntOrNull(SlQualityTextBox_start.Text);
                int slQualityEnd = ParseIntOrNull(SlQualityTextBox_end.Text);
                int exerciseStart = ParseIntOrNull(ExercciseTextBox_start.Text);
                int exerciseEnd = ParseIntOrNull(ExercciseTextBox_start_end.Text);
                int coffeeStart = ParseIntOrNull(CoffeeTextBox_start.Text);
                int coffeeEnd = ParseIntOrNull(CoffeeTextBox_end.Text);
                int screenTimeStart = ParseIntOrNull(ScreenTimeTextBox_start.Text);
                int screenTimeEnd = ParseIntOrNull(ScreenTimeTextBox_end.Text);
                double workTimeStart = ParseDoubleOrNull(WorkTimeTextBox_start.Text);
                double workTimeEnd = ParseDoubleOrNull(WorkTimeTextBox_end.Text);
                int productivityStart = ParseIntOrNull(ProductivityTextBox_start.Text);
                int productivityEnd = ParseIntOrNull(ProductivityTextBox_end.Text);
                int moodStart = ParseIntOrNull(MoodTextBox_start.Text);
                int moodEnd = ParseIntOrNull(MoodTextBox_end.Text);
                int stressStart = ParseIntOrNull(StressTextBox_start.Text);
                int stressEnd = ParseIntOrNull(StressTextBox_end.Text);
            


                _currentPage = 1;
            _allData = _slService.GetSleepDataWithParameters();
            MessageBox.Show($"Загружено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information); // Отладочное сообщение
            LoadPage(_currentPage); //загружаем первую страницу

        }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Неправильный формат числа, поле не должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private int ParseIntOrNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? 0 : int.Parse(text);
        }
        private double ParseDoubleOrNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? 0 : double.Parse(text);
        }

        private void CreateDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (!int.TryParse(NewRespondentIDTextBox.Text, out int respondentId))
                {
                    HighlightTextBox(NewRespondentIDTextBox);
                    return;
                }

                if (!double.TryParse(NewSlStartTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double slStartTime))
                {
                    HighlightTextBox(NewSlStartTimeTextBox);
                    return;
                }

                if (!double.TryParse(NewSlEndTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double slEndTime))
                {
                    HighlightTextBox(NewSlEndTimeTextBox);
                    return;
                }

                if (!double.TryParse(NewSlTotalTimeTextBox.Text, out double slTotalTime))
                {
                    HighlightTextBox(NewSlTotalTimeTextBox);
                    return;
                }

                if (!int.TryParse(NewSlQualityTextBox.Text, out int slQuality))
                {
                    HighlightTextBox(NewSlQualityTextBox);
                    return;
                }

                if (!int.TryParse(NewExercciseTextBox.Text, out int exercise))
                {
                    HighlightTextBox(NewExercciseTextBox);
                    return;
                }

                if (!int.TryParse(NewCoffeeTextBox.Text, out int coffee))
                {
                    HighlightTextBox(NewCoffeeTextBox);
                    return;
                }

                if (!int.TryParse(NewScreenTimeTextBox.Text, out int screenTime))
                {
                    HighlightTextBox(NewScreenTimeTextBox);
                    return;
                }

                if (!double.TryParse(NewWorkTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double workTime))
                {
                    HighlightTextBox(NewWorkTimeTextBox);
                    return;
                }

                if (!int.TryParse(NewProductivityTextBox.Text, out int productivity))
                {
                    HighlightTextBox(NewProductivityTextBox);
                    return;
                }

                if (!int.TryParse(NewMoodTextBox.Text, out int mood))
                {
                    HighlightTextBox(NewMoodTextBox);
                    return;
                }

                if (!int.TryParse(NewStressTextBox.Text, out int stress))
                {
                    HighlightTextBox(NewStressTextBox);
                    return;
                }

                // Если все поля прошли валидацию, создаем объект SleepDataDto
                SleepDataDto newSleepData = _slService.AddSleepData(respondentId,
                                                                     slStartTime,
                                                                     slEndTime,
                                                                     slTotalTime,
                                                                     slQuality,
                                                                     exercise,
                                                                     coffee,
                                                                     screenTime,
                                                                     workTime,
                                                                     productivity,
                                                                     mood,
                                                                     stress);

                if (newSleepData != null)
                {
                    MessageBox.Show($"Добавлена запись {newSleepData}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void DeleteData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(DeletedDataIdTextBox.Text))
                {
                    int id = int.Parse(DeletedDataIdTextBox.Text);
                    if (_slService.RemoveSleepDataById(id))
                    {
                        MessageBox.Show($"Данные с id={id} удалены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    HighlightTextBox(DeletedDataIdTextBox);
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void FindDataForUpdate_Click(object sender, RoutedEventArgs e)
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

        private void DashBoardTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) //изменение размера окна при переходе на закладку
        {
            this.Width = DashBoardTabControl.SelectedIndex == 1 ? 1000 : 800;
            this.Height = DashBoardTabControl.SelectedIndex == 1 ? 900 : 580;
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

        private void CommaValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string newText = textBox.Text + e.Text; //суммирует то, что уже есть в строке, с тем, что вводится
            Regex regex = new Regex(@"^([01]?[0-9]|2[0-3])(\.[0-5]?[0-9]?)?$");
            e.Handled = !regex.IsMatch(newText) || (newText.Count(c => c == '.') > 1);   //дополнительно ограничение количества точек      
        }

        private void NumberOneTenValidation(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            string newText = textBox.Text + e.Text;
            e.Handled = !int.TryParse(newText, out int number) || number < 1 || number > 10;
        }

        private void HighlightTextBox(TextBox textBox)
        {
            textBox.BorderBrush = Brushes.Red;
            textBox.BorderThickness = new Thickness(2);
            ResetTextBoxBorderAfterDelay(textBox, 700);
        }
        private void ResetTextBoxBorderAfterDelay(TextBox textBox, int milliSeconds) //сбрасывание цвета через опред.время
        {
            Task.Delay(milliSeconds).ContinueWith(_ => //создает задачу, которая завершится через указанное количество миллисекунд
            //Это асинхронная операция, которая не блокирует основной поток приложения.
            {
                textBox.Dispatcher.Invoke(() => // используется для выполнения кода в потоке, который владеет объектом textBox
                {
                    textBox.BorderBrush = SystemColors.ControlDarkBrush;
                    textBox.BorderThickness = new Thickness(1);
                });
            });
        }

    }
}
