using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalR.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection =
                new HubConnectionBuilder()
                    .WithUrl(new Uri(new Uri("http://localhost:5000"), "/myhub"))
                    .Build();

            var obs =
                Observable.Create<int>(async observer =>
                {
                    Console.WriteLine("Starting SignalR...");
                    await connection.StartAsync();

                    Console.WriteLine("Attaching...");
                    await connection.SendAsync("Attach", "42");

                    var handler =
                        connection
                            .On<int>("MyMessages", s =>
                                {
                                    Console.WriteLine($"SignalR received '{s}'");
                                    observer.OnNext(s);
                                    if (s == 20)
                                    {
                                        Console.WriteLine($"SignalR calling OnCompleted");
                                        observer.OnCompleted();
                                    }
                                });

                    return Disposable.Create(async () =>
                        {
                            Console.WriteLine($"Disposing subscription");
                            handler.Dispose();
                            await connection.SendAsync("Detach", "42");
                            await connection.StopAsync();
                        } );
                });

            var sub =
                obs
                    .Subscribe(
                        s => Console.WriteLine($"Observable received '{s}'"),
                        ex => Console.WriteLine($"Observable received ex: {ex}"),
                        () => Console.WriteLine($"Observable received 'OnCompleted'"));

            Console.WriteLine("Press the any key to quit...");
            Console.ReadKey();
            
            sub.Dispose();
        }
    }
}
