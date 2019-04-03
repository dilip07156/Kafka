using KafkaConsumer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace KafkaConsumerService
{
    partial class KafkaConsumer : ServiceBase
    {
        CancellationTokenSource cancelSource;
        public KafkaConsumer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            cancelSource = new CancellationTokenSource();
            StartProcess objProcess = new StartProcess();

            Task.Run(async () =>
            {
                try
                {
                    await objProcess.StartPolling(cancelSource);
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
