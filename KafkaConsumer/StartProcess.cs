using KafkaConsumer.MDMSVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumer
{
    public class StartProcess
    {
        public async Task<bool> StartPolling(CancellationTokenSource cancellationTokenSource)
        {



            //Creating local function to recall


            int TimerInterval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"]);
            try
            {

                Log("Start PollingData : " + DateTime.Now.ToString());
                //PollingData();
                Log("End PollingData");
                Log("Process_StgKafkaData : Start Process Data : " + DateTime.Now.ToString());


                int count = ProcessKafkaMessage.GetPollDataCount();

                while (count > 0)
                {
                    ProcessKafkaMessage.Process_StgKafkaData();
                    count = ProcessKafkaMessage.GetPollDataCount();

                }
                Log("Process_StgKafkaData : END Process Data : " + DateTime.Now.ToString());

                if (count == 0)
                {
                    Log("Start RePollingData : Start : " + DateTime.Now.ToString());

                    await Task.Delay(TimerInterval, cancellationTokenSource.Token);
                     StartPolling(cancellationTokenSource);


                }



            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Canceled!");
            }







            void PollingData()
            {

                var requestObject = new DC_M_masterattribute
                {
                    MasterFor = "Kafka",
                    PageNo = 0,
                    PageSize = int.MaxValue
                };
                try
                {
                    IList<DC_M_masterattribute> Kafka = Proxy.Post<IList<DC_M_masterattribute>, DC_M_masterattribute>(System.Configuration.ConfigurationManager.AppSettings["GetMasterAttributes"], requestObject).GetAwaiter().GetResult();

                    Log("Get Kafka variable Start");
                    IList<DC_M_masterattributevalue> KafkaVariables = new List<DC_M_masterattributevalue>();
                    if (Kafka != null)
                    {
                        Guid MasterAttribute_Id = Kafka.Where(w => w.Name == "KafkaVariables").Select(s => s.MasterAttribute_Id).FirstOrDefault();
                        KafkaVariables = Proxy.Get<IList<DC_M_masterattributevalue>>(string.Format(System.Configuration.ConfigurationManager.AppSettings["GetAllAttributeValuesByMasterId"].ToString(), MasterAttribute_Id.ToString(), int.MaxValue, 0)).GetAwaiter().GetResult();
                    }
                    Log("Get Kafka variable End");
                    var mode = "poll";

                    Dictionary<string, object> constructConfig = new Dictionary<string, object>();
                    var topics = new List<string>();

                    if (KafkaVariables != null)
                    {
                        Log("Construct config start");
                        KafkaVariables = KafkaVariables.Where(w => w.IsActive == "Y").ToList();

                        constructConfig.Add("group.id", KafkaVariables.Where(w => w.AttributeValue == "group.id").Select(s => s.OTA_CodeTableValue).FirstOrDefault());
                        //constructConfig.Add("group.id", "test101");
                        //constructConfig.Add("enable.auto.commit", true);
                        constructConfig.Add("enable.auto.commit", KafkaVariables.Where(w => w.AttributeValue == "enable.auto.commit").Select(s => s.OTA_CodeTableValue).FirstOrDefault());
                        constructConfig.Add("auto.commit.interval.ms", KafkaVariables.Where(w => w.AttributeValue == "auto.commit.interval.ms").Select(s => s.OTA_CodeTableValue).FirstOrDefault());
                        constructConfig.Add("statistics.interval.ms", KafkaVariables.Where(w => w.AttributeValue == "statistics.interval.ms").Select(s => s.OTA_CodeTableValue).FirstOrDefault());
                        //constructConfig.Add("bootstrap.servers", KafkaVariables.Where(w => w.AttributeValue == "bootstrap.servers").Select(s => s.OTA_CodeTableValue).FirstOrDefault());



                        var bootstrap = KafkaVariables.Where(w => w.AttributeValue.StartsWith("bootstrap.servers")).Select(s => s.OTA_CodeTableValue).ToList();
                        constructConfig.Add("bootstrap.servers", string.Join(",", bootstrap));
                        //constructConfig.Add("bootstrap.servers", "172.23.236.68:9092");
                        constructConfig.Add("default.topic.config", new Dictionary<string, object>()
                    {
                        { "auto.offset.reset", KafkaVariables.Where(w => w.AttributeValue == "auto.offset.reset").Select(s => s.OTA_CodeTableValue).FirstOrDefault() }
                    //{ "auto.offset.reset", "smallest" }
                    });
                        topics = KafkaVariables.Where(w => w.AttributeValue.StartsWith("topic_acco")).Select(s => s.OTA_CodeTableValue).ToList();
                        //topics = new List<string>() { "MDM.PROD.PRODUCTACCO.PUB" };

                        Log("Construct config End");
                    }

                    switch (mode)
                    {
                        case "poll":
                            AdvancedConsumer.Run_Poll(constructConfig, topics, cancellationTokenSource);
                            break;
                        case "consume":
                            AdvancedConsumer.Run_Consume(constructConfig, topics, cancellationTokenSource);
                            break;
                        default:
                            AdvancedConsumer.PrintUsage();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log("Exception Occurs ");
                    Log(ex.ToString());
                    throw;
                }
            }

            return true;
        }

        public async Task<bool> StartConsuming(CancellationTokenSource cancellationTokenSource)
        {
            #region Thread to Get File details from DB and Process them
            int TimerInterval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimerInterval"]);
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    ProcessKafkaMessage.Process_StgKafkaData();
                    await Task.Delay(TimerInterval, cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Canceled!");
            }

            return true;

            #endregion Thread to Get File details from DB and Process them
        }

        public void Log(string logMessage)
        {

            using (StreamWriter w = File.AppendText(System.Configuration.ConfigurationManager.AppSettings["FilePath"]))
            {

                w.WriteLine($"{logMessage}");
                w.WriteLine("-------------------------------");
                w.Flush();
                w.Close();
            }
        }

        //public static Dictionary<string, object> constructConfig(string brokerList, bool enableAutoCommit) =>
        //    new Dictionary<string, object>
        //    {
        //        { "group.id", "rubesh" },
        //        { "enable.auto.commit", enableAutoCommit },
        //        { "auto.commit.interval.ms", 5000 },
        //        { "statistics.interval.ms", 60000 },
        //        { "bootstrap.servers", brokerList },
        //        { "default.topic.config", new Dictionary<string, object>()
        //            {
        //                { "auto.offset.reset", "smallest" }
        //            }
        //        }
        //    };

        //public static void CallMethods(string mode)
        //{
        //    var topics = new List<string> { "MDM.UAT.PRODUCTACCO.PUB" };
        //    switch (mode)
        //    {
        //        case "poll":
        //            var brokerList = "172.23.217.31:9092";
        //            AdvancedConsumer.Run_Poll(constructConfig(brokerList, false), topics, CancellationToken.None);
        //            break;
        //        case "consume":
        //            AdvancedConsumer.Run_Consume(constructConfig("", false), topics, CancellationToken.None);
        //            break;
        //        default:
        //            AdvancedConsumer.PrintUsage();
        //            break;
        //    }
        //}

    }
}
