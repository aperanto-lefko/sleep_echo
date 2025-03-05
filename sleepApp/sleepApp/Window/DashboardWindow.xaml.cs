using sleepApp.Model;
using sleepApp.Dto;
using sleepApp.Service;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using sleepApp.ExceptionType;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ClosedXML.Excel;
using NLog;


namespace sleepApp
{

    public partial class DashboardWindow : Window
    {

        private int _currentRespondentPage = 1; //текущая страница для respondent
        private int _currenSleepDataPage = 1;
        private List<Respondent> _allRespondents;
        public List<SleepDataDto> _allSleepData {  get; private set; }
        private int _pageSize = 10; //количество записей на странице
        private readonly RespodentService _rService;
        private readonly SleepDataService _slService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();



        public DashboardWindow(RespodentService respondentService, SleepDataService sleepDataService)
        {
            InitializeComponent();
            _rService = respondentService;
            _slService = sleepDataService;
         }


        private void GetAllUsersButton_Click(object sender, RoutedEventArgs e) //поиск всех пользователей
        {
            try
            {
                _currentRespondentPage = 1;
                _allRespondents = _rService.GetAllRespondents();
                MessageBox.Show($"Загружено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information); // Отладочное сообщение
                Logger.Info($"Успешная загрузка данных.Загружено {_allRespondents.Count} пользователей");
                LoadPage(_allRespondents, _currentRespondentPage, UserDataGrid, PageNumberText); //загружаем первую страницу
                
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
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
                    if (_allRespondents.Count == 0)
                    {
                        MessageBox.Show($"Респондентов с такими параметрами не найдено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        UserDataGrid.ItemsSource = null;
                    }
                    else
                    {
                        MessageBox.Show($"Найдено {_allRespondents.Count} пользователей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        Logger.Info($"Успешная загрузка данных.Загружено {_allRespondents.Count} пользователей");
                        _currentRespondentPage = 1;
                        LoadPage(_allRespondents, _currentRespondentPage, UserDataGrid, PageNumberText);
                    }
                }
                else
                {
                    HighlightTextBox(RespondentLastNameTextBox);
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
            }
        }

        private void AddRespondent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parsedData = ParseInputRespondentFields();
                if (parsedData == null)
                {
                    return; // Если парсинг не удался, метод уже выделил поле красным(null будет даже если одно из значений null)
                }

                // Извлекаем данные из кортежа
                var (firstName,
                     lastName,
                     email,
                     gender,
                     country,
                     age) = parsedData.Value;
                RespondentDto newResp = _rService.AddRespondent(firstName,
                                                                 lastName,
                                                                 email,
                                                                 gender,
                                                                 country,
                                                                 age);
                if (newResp != null)
                {
                    MessageBox.Show($"Добавлен новый респондент {newResp}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Logger.Info($"Добавление нового респондента {newResp}");
                }
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка валидации: {ex.InnerException}");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка валидации: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
            }


        }

        private void FindRespondentForUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(UpdateRespondentIdTextBox.Text))
                {
                    int id = int.Parse(UpdateRespondentIdTextBox.Text);
                    Respondent resp = _rService.GetRespondentById(id);
                    NewRespondentFirstNameTextBox.Text = resp.FirstName.ToString();
                    NewRespondentLastNameTextBox.Text = resp.LastName.ToString();
                    NewRespondentEmailTextBox.Text = resp.Email.ToString().Trim();
                    NewRespondentGenderComboBox.SelectedItem = NewRespondentGenderComboBox.Items
                        .Cast<ComboBoxItem>() //Преобразует элементы ComboBox в тип ComboBoxItem, чтобы можно было работать с их свойствами
                        .FirstOrDefault(item => item.Content.ToString().Equals(resp.Gender, StringComparison.OrdinalIgnoreCase)); //сравнение без учета регистра
                    NewRespondentAgeTextBox.Text = resp.Age.ToString().Trim();
                    NewRespondentCountryTextBox.Text = resp.Country.ToString();

                }
                else
                {
                    HighlightTextBox(UpdateRespondentIdTextBox);
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
            }


        }
        private void RespondentUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(UpdateRespondentIdTextBox.Text))
                {
                    int id = int.Parse(UpdateRespondentIdTextBox.Text);
                    var parsedData = ParseInputRespondentFields();
                    if (parsedData == null)
                    {
                        return;
                    }

                    var (firstName,
                         lastName,
                         email,
                         gender,
                         country,
                         age) = parsedData.Value;
                    MessageBoxResult result = MessageBox.Show($"Обновить данные для респондента с id:{id}:\n" +
                        $"имя: {firstName},\n" +
                        $"фамилия: {lastName},\n" +
                        $"email: {email},\n" +
                        $"gender: {gender},\n" +
                        $"age: {age}?", "Обновление данных", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        if (_rService.UpdateRespondent(id,
                                                    firstName,
                                                    lastName,
                                                    email,
                                                    gender,
                                                    country,
                                                    age))
                        {
                            MessageBox.Show($"Данные для респондента id={id} успешно обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            Logger.Info($"Обновление данных для респондента  с id {id} успешно");
                        }
                        else
                        {
                            MessageBox.Show("Данные не обновлены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                else
                {
                    HighlightTextBox(UpdateRespondentIdTextBox);
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка валидации: {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
            }

        }


        private void DeleteRespondent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(DeletedRespondentIdTextBox.Text))
                {
                    int id = int.Parse(DeletedRespondentIdTextBox.Text);
                    MessageBoxResult result = MessageBox.Show($"Удаление респондента  с id {id} повлечет удаление статистических данных на него",
                        "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        if (_rService.RemoveRespondentById(id))
                        {
                            MessageBox.Show($"Респондент с id={id} и данные для него удалены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            Logger.Info($"Удаление респондента с id {id} успешно");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Удаление отменено", "Отмена", MessageBoxButton.OK, MessageBoxImage.Information);
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
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
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
                if (_allSleepData.Count == 0)
                {
                    MessageBox.Show($"Данные с такими параметрами не найдены", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    DataGrid.ItemsSource = null;
                }
                else
                {
                    MessageBox.Show($"Загружено {_allSleepData.Count} записей.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    Logger.Info($"Успешная загрузка данных.Загружено {_allSleepData.Count} записей");
                    LoadPage(_allSleepData, _currenSleepDataPage, DataGrid, PageNumberText_data);
                }
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
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
                    return; // Если парсинг не удался, метод уже выделил поле красным(null будет даже если одно из значений null)
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
                    Logger.Info($"Успешное добавление новой записи {newSleepData.Id}");
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка валидации: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
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
                        Logger.Info($"Успешное удаление записи {id}");
                    }
                    else
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
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
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
                    Logger.Info($"Поиск данных по id {id}");
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
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
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
                MessageBoxResult result = MessageBox.Show($"Обновить данные с id {dataId} для респондента с id:{respondentId}:\n" +
                       $"Время отхода ко сну: {slStartTime},\n" +
                       $"Время пробуждения: {slEndTime},\n" +
                       $"Общее время сна: {slTotalTime},\n" +
                       $"Качество сна: {slQuality},\n" +
                       $"Время для спорта: {exercise},\n" +
                       $"Кофеин: {coffee},\n" +
                       $"Время у экрана: {screenTime},\n" +
                       $"Рабочее время: {workTime},\n" +
                       $"Производительность: {productivity},\n" +
                       $"Настроение: {mood},\n" +
                       $"Уровень стресса: {stress}?", "Обновление данных", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
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
                        Logger.Info($"Успешное обновление записи {dataId}");
                    }
                    else
                    {
                        MessageBox.Show("Данные не обновлены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Logger.Info($"Данные не обновлены {dataId}");
                    }
                }
                else
                {
                    return;
                }
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Данные не найдены: {ex.InnerException}");
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка валидации: {ex.InnerException}");
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Ошибка обновления базы данных {ex.InnerException}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Info($"Непредвиденная ошибка: {ex.InnerException}");
            }
        }

        private void GraphButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Построение графиков для большого количества данных может занять продолжительное время", "Инфо", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if(result == MessageBoxResult.Cancel)
            {
                return;
            }
            var graphWindow = new GraphWindow(_allSleepData);
            graphWindow.Show();
            Logger.Info($"Запрос на построение графиков");
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

        private (string firsName, string lastName, string email, string gender, string country, int age)? ParseInputRespondentFields()
        {
            if (string.IsNullOrWhiteSpace(NewRespondentFirstNameTextBox.Text))
            {
                HighlightTextBox(NewRespondentFirstNameTextBox);
                return null;
            }
            string firstname = NewRespondentFirstNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(NewRespondentLastNameTextBox.Text))
            {
                HighlightTextBox(NewRespondentLastNameTextBox);
                return null;
            }
            string lastName = NewRespondentLastNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(NewRespondentEmailTextBox.Text))
            {
                HighlightTextBox(NewRespondentEmailTextBox);
                return null;
            }
            string email = NewRespondentEmailTextBox.Text;
            if (string.IsNullOrEmpty(NewRespondentGenderComboBox.Text))
            {
                HighlightTextBox(NewRespondentGenderComboBox);
                return null;
            }
            string gender = NewRespondentGenderComboBox.Text;
            if (string.IsNullOrWhiteSpace(NewRespondentCountryTextBox.Text))
            {
                HighlightTextBox(NewRespondentCountryTextBox);
                return null;
            }
            string country = NewRespondentCountryTextBox.Text;
            if (!int.TryParse(NewRespondentAgeTextBox.Text, out int age))
            {
                HighlightTextBox(NewRespondentAgeTextBox);
                return null;
            }
            return (firstname, lastName, email, gender, country, age);
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

        private void HighlightTextBox(Control control)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(2);
            MessageBox.Show("Необходимо заполнить все поля", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            ExportData(DataGrid, _allSleepData);
        }

        private void ExportData(DataGrid dataGrid, IEnumerable<object> dataList)
        {
            if (dataGrid.Items.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = "Data.xlsx"
            };
            if (saveDialog.ShowDialog() == true)
            {
                ExportDataToExcel(dataList, saveDialog.FileName, DataGrid);
            }
        }
        private void ExportDataToExcel(IEnumerable<object> dataList, string filePath, DataGrid dataGrid)
        {
            // Создаем новую книгу Excel с помощью библиотеки ClosedXML
            using (var workBook = new XLWorkbook())
            {
                // Добавляем новый лист в книгу Excel 
                var worksheet = workBook.Worksheets.Add("Data");

                //Заголовки столбцов
                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = DataGrid.Columns[i].Header.ToString();
                }

                //Запись строк
                int row = 2;
                foreach(var item in dataList)
                {
                    // Приводим текущий элемент DataGrid к типу SleepData
                    var data = item as SleepDataDto;
                    if (data != null)
                    {
                        // в столбец 1, строку i+2 (первая строка занята заголовками)
                        worksheet.Cell(row, 1).Value = data.Id;
                        worksheet.Cell(row, 2).Value = data.Date.ToString();
                        worksheet.Cell(row, 3).Value = data.PersonId;
                        worksheet.Cell(row, 4).Value = data.SleepStartTime;
                        worksheet.Cell(row, 5).Value = data.SleepEndTime;
                        worksheet.Cell(row, 6).Value = data.TotalSleepHours;
                        worksheet.Cell(row, 7).Value = data.SleepQuality;
                        worksheet.Cell(row, 8).Value = data.ExerciseMinutes;
                        worksheet.Cell(row, 9).Value = data.CaffeineIntakeMg;
                        worksheet.Cell(row, 10).Value = data.ScreenTime;
                        worksheet.Cell(row, 11).Value = data.WorkHours;
                        worksheet.Cell(row, 12).Value = data.ProductivityScore;
                        worksheet.Cell(row, 13).Value = data.MoodScore;
                        worksheet.Cell(row, 14).Value = data.StressLevel;
                        row++;
                    }
                   
                  
                }
                worksheet.Columns().AdjustToContents(); //выравнивание столбцов по содержимому
                workBook.SaveAs(filePath);
            }
            MessageBox.Show("Данные успешно экспортированы в Excel", "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            Logger.Info($"Выгрузка данных в excel");

        }
    }
}
