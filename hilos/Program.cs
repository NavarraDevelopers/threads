using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hilos
{
    class Program
    {
        static void Main(string[] args)
        {
            //Start3();
            MyOuterMethod();
            Console.ReadLine();
        }

        public static async Task MyOuterMethod()
        {
            var urls = new List<string>();
            var sw = Stopwatch.StartNew();

            for (int i = 1; i < 500; i++)
            {
                urls.Add("url"+i);
            }
            var allTasks = new List<Task>();
            var throttler = new SemaphoreSlim(initialCount: 20);
            foreach (var url in urls)
            {
                await throttler.WaitAsync();

              
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            var client = new HttpClient();
                            Console.WriteLine(url);
                            var html = await client.GetStringAsync(url);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }

            // won't get here until all urls have been put into tasks
            await Task.WhenAll(allTasks);
            Console.WriteLine(" seconds", sw.Elapsed.TotalSeconds);

            // won't get here until all tasks have completed in some way
            // (either success or exception)
        }

        public static void Start()
        {
            var listas = new List<String>();
            for (int i = 0; i < 1000; i++)
            {
                listas.Add(i.ToString());
            }
           
            Console.WriteLine("comienza el programa");
            Thread thread = new Thread(() =>
            {

                BinaryFormatter formatter = new BinaryFormatter();


                Parallel.ForEach(listas, lista =>
                {
                    Console.WriteLine("prueba hilo: " + lista);
                });
            });
            Console.WriteLine("no ha comenzado el hilo aún");
            thread.Start();
            Console.WriteLine("comienza el juego");
        }

        public static void Start2()
        {
            // foreach normal
            string[] colors = {
                                  "1. Red",
                                  "2. Green",
                                  "3. Blue",
                                  "4. Yellow",
                                  "5. White",
                                  "6. Black",
                                  "7. Violet",
                                  "8. Brown",
                                  "9. Orange",
                                  "10. Pink"
                              };
            Console.WriteLine("Traditional foreach loop\n");

            var sw = Stopwatch.StartNew();
            foreach (string color in colors)
            {
                Console.WriteLine("{0}, Thread Id= {1}", color, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(10);
            }
            Console.WriteLine("foreach loop execution time = {0} seconds\n", sw.Elapsed.TotalSeconds);
            Console.WriteLine("Using Parallel.ForEach");
            
            //foreach parallel
            sw = Stopwatch.StartNew();
            Parallel.ForEach(colors, color =>
            {
                Console.WriteLine("{0}, Thread Id= {1}", color, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(10);
            }
            );
            Console.WriteLine("Parallel.ForEach() execution time = {0} seconds", sw.Elapsed.TotalSeconds);
            Console.Read();
        }

        static void DoSomething(string s)
        {
            Console.WriteLine("item: "+s);
        }

        public async static void Start3()
        {
            // Sólo parallel
            var sw = Stopwatch.StartNew();
            List<string> strings = new List<string> { "s1", "s2", "s3" };
            Parallel.ForEach(strings, s =>
            {
                DoSomething(s);
            });
            Console.WriteLine(sw.Elapsed.TotalSeconds);

            // Task run
            sw = Stopwatch.StartNew();
            List<Task> Tasks = new List<Task>();
            foreach (var s in strings)
            {
                Tasks.Add(Task.Run(() => DoSomething(s)));
            }
            await Task.WhenAll(Tasks);
            Console.WriteLine(sw.Elapsed.TotalSeconds);

            // Combinación de ambas
            sw = Stopwatch.StartNew();
            await Task.Run(() => Parallel.ForEach(strings, DoSomething));
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            
            // Creación de un hilo y utilización de parallel 
            sw = Stopwatch.StartNew();
            Thread thread = new Thread(() =>
            {
                Parallel.ForEach(strings, s =>
                {
                    DoSomething(s);
                });
            });
            thread.Start();
            Console.WriteLine(sw.Elapsed.TotalSeconds);


        }
    }
}
