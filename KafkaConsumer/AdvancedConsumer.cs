using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KafkaConsumer.MDMSVC;
using System.Timers;
using Confluent.Kafka.Serialization;
using System.Configuration;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace KafkaConsumer
{
    public class AdvancedConsumer : IDisposable
    {
        public static void PrintUsage() => Console.WriteLine("Usage: .. <poll|consume> <broker,broker,..> <topic> [topic..]");

        /// <summary>   
        ///     In this example
        ///         - offsets are manually committed.
        ///         - consumer.Consume is used to consume messages.
        ///             (all other events are still handled by event handlers)
        ///         - no extra thread is created for the Poll (Consume) loop.
        /// </summary>
        public static void Run_Consume(Dictionary<string, object> constructConfig, List<string> topics, CancellationTokenSource cancellationTokenSource)
        {
            using (var consumer = new Confluent.Kafka.Consumer<Confluent.Kafka.Ignore, string>(constructConfig, null, new StringDeserializer(Encoding.UTF8)))
            {
                // Note: All event handlers are called on the main thread.

                consumer.OnPartitionEOF += (_, end)
                    => Console.WriteLine($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

                consumer.OnError += (_, error)
                    => Console.WriteLine($"Error: {error}");

                consumer.OnConsumeError += (_, error)
                    => Console.WriteLine($"Consume error: {error}");

                consumer.OnPartitionsAssigned += (_, partitions) =>
                {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                    consumer.Assign(partitions);
                };

                consumer.OnPartitionsRevoked += (_, partitions) =>
                {
                    Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
                    consumer.Unassign();
                };

                consumer.OnStatistics += (_, json)
                    => Console.WriteLine($"Statistics: {json}");

                consumer.Subscribe(topics);

                Console.WriteLine($"Started consumer, Ctrl-C to stop consuming");

                //var cancelled = false;
                //Console.CancelKeyPress += (_, e) =>
                //{
                //    e.Cancel = true; // prevent the process from terminating.
                //    cancelled = true;
                //};

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    Confluent.Kafka.Message<Confluent.Kafka.Ignore, string> msg;
                    if (!consumer.Consume(out msg, TimeSpan.FromMilliseconds(100)))
                    {
                        continue;
                    }

                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

                    if (msg.Offset % 5 == 0)
                    {
                        Console.WriteLine($"Committing offset");
                        var committedOffsets = consumer.CommitAsync(msg).Result;
                        Console.WriteLine($"Committed offset: {committedOffsets}");
                    }
                }
            }
        }

        /// <summary>
        //  In this example:
        ///         - offsets are auto commited.
        ///         - consumer.Poll / OnMessage is used to consume messages.
        ///         - no extra thread is created for the Poll loop.
        /// </summary>
        /// 
        private static bool checkReachedEnd = false;
        public static void Run_Poll(Dictionary<string, object> constructConfig, List<string> topics, CancellationTokenSource cancellationTokenSource)
        {
            StartProcess sp = new StartProcess();

            try
            {
                using (var consumer = new Confluent.Kafka.Consumer<Confluent.Kafka.Null, string>(constructConfig, null, new StringDeserializer(Encoding.UTF8)))
                {
                    sp.Log("Run_Poll Start");
                    // Note: All event handlers are called on the main thread.
                    consumer.OnMessage += (_, msg) => { /*sp.Log(msg.Value)*/; ProcessKafkaMessage.InsertInto_StgKafka(msg); };

                    consumer.OnPartitionEOF += (_, end) =>
                    {
                        sp.Log($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");
                        Console.WriteLine($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");
                         checkReachedEnd = true;
                    };
                    // Raised on critical errors, e.g. connection failures or all brokers down.
                    consumer.OnError += (_, error) =>
                    {
                        sp.Log($"Error: {error}");
                        Console.WriteLine($"Error: {error}");
                    };

                    // Raised on deserialization errors or when a consumed message has an error != NoError.
                    consumer.OnConsumeError += (_, msg) =>
                    {
                        sp.Log($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");
                        Console.WriteLine($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");
                    };

                    consumer.OnOffsetsCommitted += (_, commit) =>
                    {
                        Console.WriteLine($"[{string.Join(", ", commit.Offsets)}]");
                        sp.Log(string.Join(", ", commit.Offsets));
                        if (commit.Error)
                        {
                            Console.WriteLine($"Failed to commit offsets: {commit.Error}");
                        }
                        Console.WriteLine($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
                    };

                    consumer.OnPartitionsAssigned += (_, partitions) =>
                    {
                        Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                        consumer.Assign(partitions);
                    };

                    consumer.OnPartitionsRevoked += (_, partitions) =>
                    {
                        Console.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]");
                        consumer.Unassign();
                    };

                    consumer.OnStatistics += (_, json) => Console.WriteLine($"Statistics: {json}");

                    consumer.Subscribe(topics);

                    Console.WriteLine($"Subscribed to: [{string.Join(", ", consumer.Subscription)}]");

                    //var cancelled = false;
                    //Console.CancelKeyPress += (_, e) =>
                    //{
                    //    e.Cancel = true; // prevent the process from terminating.
                    //    cancelled = true;
                    //};
                    sp.Log("Berfore Wile loop");
                    Console.WriteLine("Ctrl-C to exit.");
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        consumer.Poll(TimeSpan.FromMilliseconds(1000));
                        if (checkReachedEnd)
                        {
                            break;
                        }
                    }
                    //consumer.CommitAsync();
                    sp.Log("Run_Poll End");
                }
            }
            catch (Exception ex)
            {
                sp.Log("Exeception in Run_Poll ");
                sp.Log(ex.ToString());
                throw;
            }
        }

        public void Dispose()
        {

        }
    }
}
