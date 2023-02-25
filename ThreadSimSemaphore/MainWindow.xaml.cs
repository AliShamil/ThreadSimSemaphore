using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace ThreadSimSemaphore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Thread> CreatedThreads { get; set; }
        public ObservableCollection<Thread> WaitingThreads { get; set; }
        public ObservableCollection<Thread> CurrentWorkingThreads { get; set; }
        private readonly Semaphore _semaphore;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;


            CreatedThreads = new();
            WaitingThreads = new();
            CurrentWorkingThreads = new();

            _semaphore = new((int)NumericUp.Value, (int)NumericUp.Value, "Sema");
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var t = new Thread(MyThreadSimulation);
            t.Name = "Thread number " + t.ManagedThreadId;

            CreatedThreads.Add(t);
        }

        private void MyThreadSimulation(object? semaphore)
        {
            if (semaphore is Semaphore s)
            {
                Thread.Sleep(3000);

                if (s.WaitOne())
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() => WaitingThreads.Remove(t));
                    Dispatcher.Invoke(() => CurrentWorkingThreads.Add(t));
                    Thread.Sleep(7000);
                    Dispatcher.Invoke(() => CurrentWorkingThreads.Remove(t));
                    s.Release();
                }
            }
        }

        private void CreatedThreadsThreadsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CreatedThreadsThreadsList.SelectedItem is Thread t)
            {
                CreatedThreads.Remove(t);

                WaitingThreads.Add(t);
                t.Start(_semaphore);
            }
        }

        private void NumericUp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
          
        }


    }
}
