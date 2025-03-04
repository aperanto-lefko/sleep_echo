using LiveCharts;
using LiveCharts.Wpf;
using sleepApp.Dto;
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
    /// <summary>
    /// Логика взаимодействия для GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public SeriesCollection DataValues { get; set; }
        private List<SleepDataDto> _sleepDataList;
        public GraphWindow(List<SleepDataDto> sleepDataList)
        {
            InitializeComponent();
            _sleepDataList = sleepDataList;
            //инициализация графика
            DataValues = new SeriesCollection();
            DataContext = this;

            XAxisCombobox.ItemsSource = new List<string>
            {
                "Кофеин, мг",
                "Качество сна (1-10)"
            };
            YAxisCombobox.ItemsSource = new List<string>
            {
                "Настроение (1-10)",
                "Стресс (1-10)",
                "Продуктивность (1-10)"
            };
            //значения по умолчанию
            XAxisCombobox.SelectedIndex = 0;
            YAxisCombobox.SelectedIndex = 0;
        }

        private void UpdateGraph(object sender, RoutedEventArgs e)
        {
            if (XAxisCombobox.SelectedItem == null || YAxisCombobox.SelectedItem == null)
            {
                return;
            }
            DataValues.Clear();
            string xAxisParam = XAxisCombobox.SelectedItem.ToString();
            string yAxisParam = YAxisCombobox.SelectedItem.ToString();

            List<double> xValues = GetXValues(xAxisParam);
            List<double> yValues = GetYValues(yAxisParam);

            DataValues.Add(new LineSeries
            {
                Title = $"{yAxisParam} от {xAxisParam}",
                Values = new ChartValues<double>(yValues),
                PointGeometrySize = 10,
                StrokeThickness = 2
            });
            DataChart.AxisX[0].Title = xAxisParam;
            DataChart.AxisY[0].Title = yAxisParam;
            DataChart.AxisX[0].Labels = xValues.ConvertAll(x => x.ToString());
            DataChart.AxisY[0].Labels = yValues.ConvertAll(y => y.ToString());
        }
        // Метод для получения значений по оси X
        private List<double> GetXValues(string xAxisParam)
        {
            switch (xAxisParam)
            {
                case "Кофеин (мг)": return _sleepDataList.Select(x => (double)x.CaffeineIntakeMg).ToList();
                //case "Рабочее время (часы)": return _sleepDataList.Select(x => x.WorkHours).ToList();
                case "Качество сна (1-10)": return _sleepDataList.Select(x => (double)x.SleepQuality).ToList();
                //case "Время у экрана (минуты)": return _sleepDataList.Select(x => (double)x.ScreenTime).ToList();
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
