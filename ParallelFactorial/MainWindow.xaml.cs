using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ParallelFactorial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            int n = 0;
            Dispatcher.Invoke(() =>
            {
                n = int.Parse(Input.Text);
            });            
            Thread f = new Thread(new ParameterizedThreadStart(Factorial));
            f.Start(n);
        }

        private void Factorial(object n)
        {
            Dispatcher.Invoke(() =>
            {
                Output.Text = "";
            });
            BigInteger result = 1;
            Mutex mut = new Mutex();
            Task[] factorial = new Task[4];
            for (int i = 0; i < 4; i++)
            {
                int startingIndex = i + 1;
                factorial[i] = Task.Run(() =>
                {
                    BigInteger intermResult = 1;
                    for (int j = startingIndex; j <= (int)n; j += 4)
                    {
                        intermResult *= j;
                        //Dispatcher.Invoke(() =>
                        //{
                        //    Output.Text += $"{intermResult}\n";
                        //});
                    }
                    mut.WaitOne();
                    result *= intermResult;
                    mut.ReleaseMutex();
                });
            }
            Task.WaitAll(factorial);
            Dispatcher.Invoke(() =>
            {
                Output.Text += $"{result}";
            });
        }

    }
}
