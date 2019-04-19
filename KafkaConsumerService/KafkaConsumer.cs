using KafkaConsumer;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumerService
{
    partial class KafkaConsumer : ServiceBase
    {
        private CancellationTokenSource cancelSource;
        public static int TimerInterval;
        public KafkaConsumer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            cancelSource = new CancellationTokenSource();
            StartProcess objProcess = new StartProcess();
            if (!int.TryParse(ConfigurationManager.AppSettings["TimerInterval"], out TimerInterval))
            {
                TimerInterval = 60000;
            }

            Task.Run(async () =>
            {
                try
                {
                    while (!cancelSource.IsCancellationRequested)
                    {
                        await objProcess.StartPolling(cancelSource);
                        await Task.Delay(TimerInterval, cancelSource.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Canceled!");
                }
            });

            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        await objProcess.StartConsuming(cancelSource);
            //    }
            //    catch (OperationCanceledException)
            //    {
            //        Console.WriteLine("Canceled!");
            //    }
            //});
        }

        protected override void OnStop()
        {
            cancelSource.Cancel();
        }
    }
}
