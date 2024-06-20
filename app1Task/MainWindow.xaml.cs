using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app1Task
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Counter { get; set; }
        CancellationToken CancellationToken;
        CancellationTokenSource CancellationTokenSource;
        Task TareaLarga;

        public MainWindow()
        {
            InitializeComponent();
            CrearTarea();

        }
        public void CrearTarea()
        {
            Task TareaAction;
            Task<int> TareaFunc;
            var ejecucionAction = new Action(ShowMessage);
            var ejecucionFunc = new Func<int>(suma);
            TareaAction = new Task(ejecucionAction);
            TareaFunc = new Task<int>(ejecucionFunc);
            TareaAction.Start();

            Task Tarea2 = new Task(Function1);
            Tarea2.Start();
            TareaFunc.Start();





            var response = TareaFunc.Result;
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            Task Tarea3 = new Task(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    System.Diagnostics.Debug.WriteLine(i + " en hilo 1");
                    Thread.Sleep(1000);
                }
                taskCompletionSource.TrySetResult(true);
            });
            Task Tarea4 = new Task(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    System.Diagnostics.Debug.WriteLine(i + " en hilo 2");
                    Thread.Sleep(1000);
                }

            });
            Tarea3.Start();
            var TaskFinished = taskCompletionSource.Task.Result;
            if (TaskFinished)
                Tarea4.Start();


        }
        public void ShowMessage()
        {
            System.Diagnostics.Debug.WriteLine("ejecucion show message");
            MessageBox.Show("hola mundo");

        }
        public int suma()
        {
            var response = 1 + 1;
            System.Diagnostics.Debug.WriteLine(response);
            System.Diagnostics.Debug.WriteLine(Counter + " en suma");
            return response;
        }
        public void Function1()
        {
            System.Diagnostics.Debug.WriteLine("Funcion 1");
            System.Diagnostics.Debug.WriteLine($"Counter antes {Counter}");
            lock (this)
            {
                Counter++;
            }
            System.Diagnostics.Debug.WriteLine($"Counter despues {Counter}");
        }

        public Task Start()
        {
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            TareaLarga = Task.Run(() =>
            {
                EjecucionLarga(CancellationToken);
            }, CancellationToken);
            return Task.CompletedTask;
        }
        public Task Stop()
        {
            CancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
        void EjecucionLarga(CancellationToken CT)
        {
            for (int i = 0; i < 100000; i++)
            {
                if (!CT.IsCancellationRequested)
                {
                    System.Diagnostics.Debug.WriteLine("Trabajando");
                    Thread.Sleep(1000);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Cancelado");
                    break;
                }
            }
        }

        private async void start_Click(object sender, RoutedEventArgs e)
        {
            Start();

        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        public void Getstatus()
        {
            System.Diagnostics.Debug.WriteLine($"status de tarea {TareaLarga.Status}");
        }

        private void status_Click(object sender, RoutedEventArgs e)
        {
            Getstatus();
        }

        public async Task IniciaProceso()
        {
            var progreso = new Progress<int>(percent =>
            {
                System.Diagnostics.Debug.WriteLine($"{percent} %");
            });

            await Task.Run(()=> Proceso(progreso));
            System.Diagnostics.Debug.WriteLine($"proceso terminado");
        }
        public void Proceso(IProgress<int> progress)
        {
            for (int i = 0; i <= 100; i++)
            {
                Thread.Sleep(100);
                if(progress != null)
                {
                    progress.Report(i);
                }
            }
        }

        private async void progressTest_Click(object sender, RoutedEventArgs e)
        {
            await IniciaProceso();
        }
    }
}