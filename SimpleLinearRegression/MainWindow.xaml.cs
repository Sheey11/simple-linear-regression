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
using System.Windows.Navigation;
using System.Reflection;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading;
using System.IO;

namespace SimpleLinearRegression
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        Thread workThread, uiThread;
        Model m = new Model(0.00059);

        public MainWindow() {
            InitializeComponent();
        }

        private void GoBtn_Click(object sender, RoutedEventArgs e) {
            var observableValues = new ChartValues<LiveCharts.Defaults.ObservableValue>();
            var series = new SeriesCollection {
                new LineSeries {
                    Values = observableValues
                }
            };
            LossChart.Series = series;

            // https://www.kaggle.com/andonians/random-linear-regression
            var data_x = File.ReadAllLines(@"C:\Users\sheey\Documents\ml\train_x.txt");
            var data_y = File.ReadAllLines(@"C:\Users\sheey\Documents\ml\train_y.txt");
            var trainDataset = new Point[data_x.Length];
            for(int i = 0; i < data_x.Length; i++) {
                trainDataset[i] = new Point(double.Parse(data_x[i]), double.Parse(data_y[i]));
            }
            uiThread = new Thread(() => {
                int x = 0;
                m.OnTrain += (s, args) => {
                    Epoch.Dispatcher.Invoke(() => {
                        // update chart, epoch, step, a, b
                        observableValues.Add(new LiveCharts.Defaults.ObservableValue(m.Loss));
                        x++;
                        Epoch.Text = m.CurrentEpoch.ToString();
                        Step.Text = m.CurrentStep.ToString();
                        A.Text = Math.Round(m.a, 2).ToString();
                        B.Text = Math.Round(m.b, 2).ToString();
                    });
                };
            });

            workThread = new Thread(() => {
                m.Train(trainDataset, 1);
            });
            workThread.Start();
            uiThread.Start();
        }
    }
}
