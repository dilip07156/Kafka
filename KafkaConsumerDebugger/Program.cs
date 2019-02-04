﻿using KafkaConsumer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumerDebugger
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = string.Empty;
            bool LogToConsole = false;

            int ThreadSize = 0;

            Console.WriteLine("Do you want to log info to console ? (y/n)");

            input = Console.ReadLine();

            while (input != "y" || input != "n")
            {
                Console.Clear();
                if (input == "y")
                {
                    LogToConsole = true;
                    break;
                }
                else if (input == "n")
                {
                    LogToConsole = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input : " + input); Console.WriteLine("Do you want to log info to console ? (y/n)"); input = Console.ReadLine();
                }
            }

            Console.WriteLine("Press any key to start the process.."); input = Console.ReadLine();

            Console.WriteLine("Processing..");

            var cancelSource = new CancellationTokenSource();

            StartProcess sp = new StartProcess();
            Console.WriteLine("Press C to cancel the operation.");
            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        await sp.StartPolling(cancelSource);
            //    }
            //    catch (OperationCanceledException Ex)
            //    {
            //        Console.WriteLine("Canceled!");
            //    }
            //});

            Task.Run(async () =>
            {
                try
                {
                    await sp.StartConsuming(cancelSource);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Canceled!");
                }
            });


            input = Console.ReadLine();

            if (input.ToLower() == "c")
            {
                cancelSource.Cancel(); // Safely cancel worker.
                cancelSource.Dispose();
                Console.WriteLine(Environment.NewLine + "Processing Completed. Press any key to exit..");
            }

            Console.ReadLine();

        }
    }
}
