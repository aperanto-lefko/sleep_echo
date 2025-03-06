using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using sleepApp.Dto;
using System.Windows;

namespace sleepApp
{
    /// <summary>
    /// Логика взаимодействия для GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public SeriesCollection DataValues { get; set; }
        public List<SleepDataDto> _sleepDataList;

        public GraphWindow(List<SleepDataDto> sleepDataList)
        {
            InitializeComponent();
            _sleepDataList = sleepDataList;
            //инициализация графика

            DataValues = new SeriesCollection();
            DataContext = this;

            XAxisCombobox.ItemsSource = new List<string>
            {
                "Время отхода ко сну HH.MM",
                "Время пробуждения HH.MM",
                "Общее время сна HH.MM",
                "Качество сна (1-10)",
                "Время для спорта (минуты)",
                "Кофеин, мг",
                "Время у экрана (минуты)",
                "Рабочее время HH.MM"
            };
            YAxisCombobox.ItemsSource = new List<string>
            {
                "Настроение (1-10)",
                "Стресс (1-10)",
                "Продуктивность (1-10)"
            };
        }

        private void UpdateGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (XAxisCombobox.SelectedItem == null || YAxisCombobox.SelectedItem == null)
                {
                    return;
                }

                string xAxisParam = XAxisCombobox.SelectedItem.ToString();
                string yAxisParam = YAxisCombobox.SelectedItem.ToString();

                var dataWindow = Application.Current.Windows.OfType<DashboardWindow>().FirstOrDefault(); //ищем открытое окно с данными

                if (dataWindow != null)
                {
                    // Получаем данные из DataWindow
                    _sleepDataList = dataWindow._allSleepData; //получаем данные из окна

                    if (_sleepDataList == null || _sleepDataList.Count == 0)
                    {
                        MessageBox.Show("Нет данных для отображения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Окно с данными не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                List<double> xValues = null;
                List<double> yValues = null;

                xValues = GetXValues(xAxisParam);
                yValues = GetYValues(yAxisParam);

                if (xValues == null || yValues == null || xValues.Count == 0 || yValues.Count == 0)
                {
                    MessageBox.Show("Нет данных для отображения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Упорядочиваем данные по X
                var combined = xValues.Zip(yValues, (x, y) => new { X = x, Y = y }).OrderBy(point => point.X).ToList();
                xValues = combined.Select(point => point.X).ToList();
                yValues = combined.Select(point => point.Y).ToList();
                
                double minYValue = yValues.Min();
                double maxYValue = yValues.Max();

                if (minYValue == maxYValue) // Если все значения одинаковые
                {

                    MessageBox.Show("Недостаточно данных для отображения", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                double minXValue = xValues.Min();
                double maxXValue = xValues.Max();

                if (minXValue == maxXValue) // Если все значения одинаковые
                {
                    MessageBox.Show("Недостаточно данных для отображения", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Создаем новую серию
                var lineSeries = new LineSeries
                {
                    Title = $"{yAxisParam} от {xAxisParam}",
                    Values = new ChartValues<ObservablePoint>(), // Используем ObservablePoint для точек (X, Y)
                    PointGeometrySize = 10,
                    StrokeThickness = 2
                };

                // Добавляем точки (X, Y)
                for (int i = 0; i < xValues.Count; i++)
                {
                    lineSeries.Values.Add(new ObservablePoint(xValues[i], yValues[i]));
                }

                // Очищаем старые серии (если нужно)
                DataChart.Series.Clear();

                // Добавляем новую серию в график
                DataChart.Series.Add(lineSeries);

                // Настраиваем оси
                DataChart.AxisX.Clear();
                DataChart.AxisX.Add(new Axis
                {
                    Title = xAxisParam,
                    LabelFormatter = value => value.ToString("N2"), // Форматирование значений Добавляет разделитель тысяч (например, 1,000 вместо 1000). Округляет число до 2 знаков после запятой.
                    MinValue = minXValue, // Минимальное значение на оси X
                    MaxValue = maxXValue, // Максимальное значение на оси X
                    Separator = new Separator { Step = GetStepForAxis(xAxisParam) }
                });

                DataChart.AxisY.Clear();
                DataChart.AxisY.Add(new Axis
                {
                    Title = yAxisParam,
                    LabelFormatter = value => value.ToString("N2"), // Форматирование значений
                    MinValue = minYValue,
                    MaxValue = maxYValue,
                    Separator = new Separator { Step = 1 } // Шаг между значениями
                });
            }
            catch (LiveChartsException ex)
            {
                MessageBox.Show($"Ошибка при обновлении графика: {ex.Message}, {ex.InnerException}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении графика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }


        private double GetStepForAxis(string xAxisParam)
        {
            switch (xAxisParam)
            {
                case "Время отхода ко сну HH.MM": return 1;
                case "Время пробуждения HH.MM": return 1;
                case "Общее время сна HH.MM": return 1;
                case "Рабочее время HH.MM": return 1;
                case "Качество сна (1-10)": return 1;
                case "Время для спорта (минуты)": return 10;
                case "Кофеин, мг": return 20;
                case "Время у экрана (минуты)": return 5;
                default: return 5;
            }
        }

        // Метод для получения значений по оси X
        private List<double> GetXValues(string xAxisParam)
        {
            switch (xAxisParam)
            {
                case "Время отхода ко сну HH.MM": return _sleepDataList.Select(x => x.SleepStartTime).ToList();
                case "Время пробуждения HH.MM": return _sleepDataList.Select(x => x.SleepEndTime).ToList();
                case "Общее время сна HH.MM": return _sleepDataList.Select(x => x.TotalSleepHours).ToList();
                case "Качество сна (1-10)": return _sleepDataList.Select(x => (double)x.SleepQuality).ToList();
                case "Время для спорта (минуты)": return _sleepDataList.Select(x => (double)x.ExerciseMinutes).ToList();
                case "Кофеин, мг": return _sleepDataList.Select(x => (double)x.CaffeineIntakeMg).ToList();
                case "Время у экрана (минуты)": return _sleepDataList.Select(x => (double)x.ScreenTime).ToList();
                case "Рабочее время HH.MM": return _sleepDataList.Select(x => x.WorkHours).ToList();

                default: return new List<double>();
            }
        }

        private List<double> GetYValues(string yAxisParam)
        {
            switch (yAxisParam)
            {
                case "Настроение (1-10)": return _sleepDataList.Select(x => (double)x.MoodScore).ToList();
                case "Стресс (1-10)": return _sleepDataList.Select(x => (double)x.StressLevel).ToList();
                case "Продуктивность (1-10)": return _sleepDataList.Select(x => (double)x.ProductivityScore).ToList();
                default: return new List<double>();
            }
        }
    }
}