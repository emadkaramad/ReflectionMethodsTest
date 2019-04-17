using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReflectionTest
{
    public class Program
    {
        public static void Main(params string[] parameters)
        {
            // 1: Normal
            // 2: Cached delegates
            // 3: Direct access
            var totalTestItems = 1000;
            var totalTasks = 100;
            var totalIterations = 5;
            Task<long>[] tasks;
            Stopwatch sw;

            long averageTotalDirectAccess = 0;
            long averageTotalCachedDelegate = 0;
            long averageTotalNormal = 0;

            new TestPerformance().CacheReflection();


            for (var i = 0; i < totalIterations; i++)
            {
                Console.WriteLine($"============== Attempt: {i + 1} ==============");
                Console.WriteLine("\r\nDirect access:");
                sw = Stopwatch.StartNew();
                tasks = Enumerable.Range(0, totalTasks)
                    .Select(_ => new TestPerformance().TestReflection(3, numberOfObjects: totalTestItems)) // Direct Access
                    .ToArray();
                Task.WaitAll(tasks);
                sw.Stop();
                averageTotalDirectAccess += tasks.Sum(x => x.Result);
                Console.WriteLine($"Average milliseconds for each task: {tasks.Average(x => new TimeSpan(x.Result).TotalMilliseconds)}");
                Console.WriteLine($"Total ticks for all tasks: {sw.Elapsed.Ticks}");

                Console.WriteLine("\r\nCached delegates:");
                sw = Stopwatch.StartNew();
                tasks = Enumerable.Range(0, totalTasks)
                    .Select(_ => new TestPerformance().TestReflection(2, numberOfObjects: totalTestItems)) // Cached Delegates
                    .ToArray();
                Task.WaitAll(tasks);
                sw.Stop();
                averageTotalCachedDelegate += tasks.Sum(x => x.Result);
                Console.WriteLine($"Average milliseconds for each task: {tasks.Average(x => new TimeSpan(x.Result).TotalMilliseconds)}");
                Console.WriteLine($"Total ticks for all tasks: {sw.Elapsed.Ticks}");

                Console.WriteLine("\r\nNormal reflection:");
                sw = Stopwatch.StartNew();
                tasks = Enumerable.Range(0, totalTasks)
                    .Select(_ => new TestPerformance().TestReflection(1, numberOfObjects: totalTestItems)) // Normal reflection
                    .ToArray();
                Task.WaitAll(tasks);
                sw.Stop();
                averageTotalNormal += tasks.Sum(x => x.Result);
                Console.WriteLine($"Average milliseconds for each task: {tasks.Average(x => new TimeSpan(x.Result).TotalMilliseconds)}");
                Console.WriteLine($"Total ticks for all tasks: {sw.Elapsed.Ticks}");
            }

            Console.WriteLine($"\r\n\r\n============== Result: ==============");
            Console.WriteLine($"Total test objects: {totalIterations * totalTasks * totalTestItems:n0}");
            Console.WriteLine($"Total tasks: {totalIterations * totalTasks:n0}");
            Console.WriteLine($"Total iterations: {totalIterations:n0}");
            Console.WriteLine($"Total tasks in each iteration: {totalTasks:n0}");
            Console.WriteLine($"Total test objects in each task: {totalTestItems:n0}");
            Console.WriteLine();
            Console.WriteLine($"Average ms for each iteration of Direct Access: \t{new TimeSpan(averageTotalDirectAccess).TotalMilliseconds / totalIterations}");
            Console.WriteLine($"Average ms for each iteration of Cached Delegates: \t{new TimeSpan(averageTotalCachedDelegate).TotalMilliseconds / totalIterations}");
            Console.WriteLine($"Average ms for each iteration of Normal: \t\t{new TimeSpan(averageTotalNormal).TotalMilliseconds / totalIterations}");
            Console.WriteLine();
            Console.WriteLine($"Average ms for each task of Direct Access: \t\t{new TimeSpan(averageTotalDirectAccess).TotalMilliseconds / totalIterations / totalTasks}");
            Console.WriteLine($"Average ms for each task of Cached Delegates: \t\t{new TimeSpan(averageTotalCachedDelegate).TotalMilliseconds / totalIterations / totalTasks}");
            Console.WriteLine($"Average ms for each task of Normal: \t\t\t{new TimeSpan(averageTotalNormal).TotalMilliseconds / totalIterations / totalTasks}");
            Console.WriteLine();
            Console.WriteLine($"Average ms for each test object of Direct Access: \t{new TimeSpan(averageTotalDirectAccess).TotalMilliseconds / totalIterations / totalTasks / totalTestItems}");
            Console.WriteLine($"Average ms for each test object of Cached Delegates: \t{new TimeSpan(averageTotalCachedDelegate).TotalMilliseconds / totalIterations / totalTasks / totalTestItems}");
            Console.WriteLine($"Average ms for each test object of Normal: \t\t{new TimeSpan(averageTotalNormal).TotalMilliseconds / totalIterations / totalTasks / totalTestItems}");

            Console.ReadKey();
        }

    }
}