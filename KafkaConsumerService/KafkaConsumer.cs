using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KafkaConsumerService
{
    partial class KafkaConsumer : ServiceBase
    {
        public KafkaConsumer()
        {
            InitializeComponent();
        }

        Timer timer = new Timer();
        protected override void OnStart(string[] args)
        {
            //TraceService("start service");
            ////handle Elapsed event
            //timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);

            ////This statement is used to set interval to 1 minute (= 60,000 milliseconds)

            //timer.Interval = 60000;

            ////enabling the timer
            //timer.Enabled = true;
        }

        protected override void OnStop()
        {
            // timer.Enabled = false;
            //TraceService("stopping service");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            TraceService("Another entry at " + DateTime.Now);
        }

        void TraceService(string content)
        {
            //set up a filestream
            FileStream fs = new FileStream(@"d:\ScheduledService.txt", FileMode.OpenOrCreate, FileAccess.Write);

            //set up a streamwriter for adding text
            StreamWriter sw = new StreamWriter(fs);

            //find the end of the underlying filestream
            sw.BaseStream.Seek(0, SeekOrigin.End);

            //add the text
            sw.WriteLine(content);
            //add the text to the underlying filestream

            sw.Flush();
            //close the writer
            sw.Close();
        }

    }
}
