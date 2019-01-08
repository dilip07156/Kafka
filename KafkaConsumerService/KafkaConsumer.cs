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
            objProcess.Start(cancelSource);
        }

        protected override void OnStop()
        {
            cancelSource.Cancel();
        }
    }
}
