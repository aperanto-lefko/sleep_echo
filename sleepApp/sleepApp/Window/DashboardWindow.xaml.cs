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
using static System.Diagnostics.Activity;

namespace sleepApp
{

    public partial class DashboardWindow : Window
    {

        private int _currentRespondentPage = 1; //текущая страница для respondent
        private int _currenSleepDataPage = 1;
        private List<Respondent> _allRespondents;
        private List<SleepDataDto> _allSleepData;
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
                _currentRespondentPage = 1;
                _allRespondents = _rService.GetAllRespondents();
                MessageBox.Show($"Загружено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information); // Отладочное сообщение
                LoadPage(_allRespondents, _currentRespondentPage, UserDataGrid, PageNumberText); //загружаем первую страницу
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
                    _currentRespondentPage = 1;
                    LoadPage(_allRespondents, _currentRespondentPage, UserDataGrid, PageNumberText);
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
        private void FindRespondentForUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RespondentUpdate_Click(object sender, RoutedEventArgs e)
        {

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


                _currenSleepDataPage = 1;
                _allSleepData = _slService.GetSleepDataWithParameters(respondentId,
                                                                      slStartTimeStart,
                                                                      slStartTimeEnd,
                                                                      slEndTimeStart,
                                                                      slEndTimeEnd,
                                                                      slTotalTimeStart,
                                                                      slTotalTimeEnd,
                                                                      slQualityStart,
                                                                      slQualityEnd,
                                                                      exerciseStart,
                                                                      exerciseEnd,
                                                                      coffeeStart,
                                                                      coffeeEnd,
                                                                      screenTimeStart,
                                                                      screenTimeEnd,
                                                                      workTimeStart,
                                                                      workTimeEnd,
                                                                      productivityStart,
                                                                      productivityEnd,
                                                                      moodStart,
                                                                      moodEnd,
                                                                      stressStart,
                                                                      stressEnd);
                MessageBox.Show($"Загружено {_allSleepData.Count} записей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadPage(_allSleepData, _currenSleepDataPage, DataGrid, PageNumberText_data);
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
            return string.IsNullOrWhiteSpace(text) ? 0 : double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        private void CreateDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var parsedData = ParseInputDataFields();
                if (parsedData == null)
                {
                    return; // Если парсинг не удался, метод уже выделил поле красным
                }

                // Извлекаем данные из кортежа
                var (respondentId,
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
                    stress) = parsedData.Value;

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
                    } else
                    {
                        MessageBox.Show("Данные не удалены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            try
            {
                if (!string.IsNullOrWhiteSpace(UpdatedDataIdTextBox.Text))
                {
                    int id = int.Parse(UpdatedDataIdTextBox.Text);
                    SleepData data = _slService.GetSleepDataById(id);
                    NewRespondentIDTextBox.Text = data.PersonId.ToString().Trim();
                    NewSlStartTimeTextBox.Text = data.SleepStartTime.ToString().Trim().Replace(",", ".");
                    NewSlEndTimeTextBox.Text = data.SleepEndTime.ToString().Trim().Replace(",", ".");
                    NewSlTotalTimeTextBox.Text = data.TotalSleepHours.ToString().Trim().Replace(",", ".");
                    NewSlQualityTextBox.Text = data.SleepQuality.ToString().Trim();
                    NewExercciseTextBox.Text = data.ExerciseMinutes.ToString().Trim();
                    NewCoffeeTextBox.Text = data.CaffeineIntakeMg.ToString().Trim();
                    NewScreenTimeTextBox.Text = data.ScreenTime.ToString().Trim();
                    NewWorkTimeTextBox.Text = data.WorkHours.ToString().Trim().Replace(",", ".");
                    NewProductivityTextBox.Text = data.ProductivityScore.ToString();
                    NewMoodTextBox.Text = data.MoodScore.ToString().Trim();
                    NewStressTextBox.Text = data.StressLevel.ToString().Trim();

                }
                else
                {
                    HighlightTextBox(UpdatedDataIdTextBox);
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
        private void DateUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int dataId;
                if (!string.IsNullOrWhiteSpace(UpdatedDataIdTextBox.Text))
                {
                    dataId = int.Parse(UpdatedDataIdTextBox.Text);
                }
                else
                {
                    HighlightTextBox(UpdatedDataIdTextBox);
                    return;
                }
                var parsedData = ParseInputDataFields();
                if (parsedData == null)
                {
                    return; // Если парсинг не удался, метод уже выделил поле красным
                }

                // Извлекаем данные из кортежа
                var (respondentId,
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
                    stress) = parsedData.Value;

                if (_slService.UpdateSleepData(dataId,
                                                                     respondentId,
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
                                                                     stress))
                {
                  
                        MessageBox.Show($"Данные для записи id={dataId} успешно обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    } else { 
                       MessageBox.Show("Данные не обновлены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        

        private (int respondentId, double slStartTime, double slEndTime, double slTotalTime, int slQuality,
            int exercise, int coffee, int screenTime, double workTime, int productivity, int mood, int stress)? ParseInputDataFields()
        {
            if (!int.TryParse(NewRespondentIDTextBox.Text, out int respondentId))
            {
                HighlightTextBox(NewRespondentIDTextBox);
                return null;
            }

            if (!double.TryParse(NewSlStartTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double slStartTime))
            {
                HighlightTextBox(NewSlStartTimeTextBox);
                return null;
            }

            if (!double.TryParse(NewSlEndTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double slEndTime))
            {
                HighlightTextBox(NewSlEndTimeTextBox);
                return null;
            }

            if (!double.TryParse(NewSlTotalTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double slTotalTime))
            {
                HighlightTextBox(NewSlTotalTimeTextBox);
                return null;
            }

            if (!int.TryParse(NewSlQualityTextBox.Text, out int slQuality))
            {
                HighlightTextBox(NewSlQualityTextBox);
                return null;
            }

            if (!int.TryParse(NewExercciseTextBox.Text, out int exercise))
            {
                HighlightTextBox(NewExercciseTextBox);
                return null;
            }

            if (!int.TryParse(NewCoffeeTextBox.Text, out int coffee))
            {
                HighlightTextBox(NewCoffeeTextBox);
                return null;
            }

            if (!int.TryParse(NewScreenTimeTextBox.Text, out int screenTime))
            {
                HighlightTextBox(NewScreenTimeTextBox);
                return null;
            }

            if (!double.TryParse(NewWorkTimeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double workTime))
            {
                HighlightTextBox(NewWorkTimeTextBox);
                return null;
            }

            if (!int.TryParse(NewProductivityTextBox.Text, out int productivity))
            {
                HighlightTextBox(NewProductivityTextBox);
                return null;
            }

            if (!int.TryParse(NewMoodTextBox.Text, out int mood))
            {
                HighlightTextBox(NewMoodTextBox);
                return null;
            }

            if (!int.TryParse(NewStressTextBox.Text, out int stress))
            {
                HighlightTextBox(NewStressTextBox);
                return null;
            }

            // Если все поля прошли валидацию, возвращаем кортеж с данными
            return (respondentId, slStartTime, slEndTime, slTotalTime, slQuality, exercise, coffee, screenTime, workTime, productivity, mood, stress);
        }


        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            PreviousPage(ref _currentRespondentPage, _allRespondents, UserDataGrid, PageNumberText);
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            NextPage(ref _currentRespondentPage, _allRespondents, UserDataGrid, PageNumberText);
        }

        private void NextPageData_Click(object sender, RoutedEventArgs e)
        {
            NextPage(ref _currenSleepDataPage, _allSleepData, DataGrid, PageNumberText_data);
        }
        private void PreviousPageData_Click(object sender, RoutedEventArgs e)
        {
            PreviousPage(ref _currenSleepDataPage, _allSleepData, DataGrid, PageNumberText_data);
        }

        private void LoadPage(IEnumerable<object> dataList, int page, DataGrid dataGrid, TextBlock pageNumberText)
        {
            var itemsForDisplay = dataList
                .Skip((page - 1) * _pageSize) //Пропускает указанное количество элементов
                .Take(_pageSize) //Берет следующее количество элементов
                .ToList();
            dataGrid.ItemsSource = itemsForDisplay;
            pageNumberText.Text = $"Страница {page}";
        }
        private void NextPage(ref int currentPage, IEnumerable<object> dataList, DataGrid dataGrid, TextBlock pageNumberText)
        {
            if (currentPage < (dataList.Count() / _pageSize) + 1)
            {
                currentPage++;
                LoadPage(dataList, currentPage, dataGrid, pageNumberText);
            }
        }
        private void PreviousPage(ref int currentPage, IEnumerable<object> dataList, DataGrid dataGrid, TextBlock pageNumberText)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPage(dataList, currentPage, dataGrid, pageNumberText);
            }
        }
        private void DashBoardTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) //изменение размера окна при переходе на закладку
        {
            this.Width = DashBoardTabControl.SelectedIndex == 1 ? 1000 : 800;
            this.Height = DashBoardTabControl.SelectedIndex == 1 ? 900 : 580;
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
