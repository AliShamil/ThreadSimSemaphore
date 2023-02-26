using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace ThreadSimSemaphore;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    private int availableThreads;
    private double? upDownValue = 0;
    private readonly Semaphore _semaphore;
    public ObservableCollection<Thread> CreatedThreads { get; set; }
    public ObservableCollection<Thread> WaitingThreads { get; set; }
    public ObservableCollection<Thread> WorkingThreads { get; set; }


    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        CreatedThreads = new();
        WaitingThreads = new();
        WorkingThreads = new();

        upDownValue = UpDown.Value;
        
        _semaphore = new(1, 20, "ThreadManagementSemaphore");
        availableThreads = 1;
    }

    private void BtnCreate_Click(object sender, RoutedEventArgs e)
    {
        var t = new Thread(ThreadManagementSimulation);

        t.Name = "Thread number " + t.ManagedThreadId;

        CreatedThreads.Add(t);
    }
   
    private void ThreadManagementSimulation(object? semaphore)
    {

        if (semaphore is Semaphore s)
        {
            Thread.Sleep(3000);

            if (s.WaitOne())
            {
                var current = Thread.CurrentThread;
                Dispatcher.Invoke(() => WaitingThreads.Remove(current));
                Dispatcher.Invoke(() => WorkingThreads.Add(current));

                availableThreads--;
                
                var workingThreads = Random.Shared.Next(3, 10);
                    
                current.Name = current.Name + "->" + workingThreads;


                while (workingThreads > 0)
                {
                    Thread.Sleep(1500);
                    workingThreads--;
                }

                Dispatcher.Invoke(() => WorkingThreads.Remove(current));
                availableThreads++;
                s.Release();
            }
        }
    }
    
    private void UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
    {
        if (sender is NumericUpDown upDown)
        {
            if (upDownValue < upDown.Value)
            {
                _semaphore?.Release();
                availableThreads++;
            }
            else
            {
                if (availableThreads == 0)
                {
                    upDown.Value = upDownValue;
                    return;
                }

                availableThreads--;
                _semaphore?.WaitOne();
            }


            upDownValue = upDown.Value;
        }
    }

    private void CreatedList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (CreatedList.SelectedItem is Thread t)
        {
            CreatedThreads.Remove(t);

            WaitingThreads.Add(t);
            t.Start(_semaphore);
        }
    }
}