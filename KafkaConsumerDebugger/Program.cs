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
            sp.Start(cancelSource.Token);

            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        while (!cancelSource.IsCancellationRequested)
            //        {

            //            await Task.Delay(6000, cancelSource.Token);
            //        }
            //    }
            //    catch (OperationCanceledException)
            //    {
            //        Console.WriteLine("Canceled!");
            //    }
            //});

            Console.WriteLine("Press C to cancel the operation.");

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