using System;
using System.Diagnostics;
using System.Threading;
using ScottPlot;

namespace Task17;

class Program
{
    static void Main()
    {
        Console.WriteLine("Запуск эксперимента для графика...");
        double[] withoutScheduler = RunBlockingScenario();
        double[] withScheduler = RunSchedulerScenario();

        var plt = new Plot();
        var bar1 = plt.Add.Bars(withoutScheduler);
        bar1.LegendText = "Без планировщика (Блокировка)";
        
        var bar2 = plt.Add.Bars(withScheduler);
        bar2.LegendText = "С планировщиком (Round Robin)";
        
        plt.Title("Время ожидания коротких команд (Latency)");
        plt.YLabel("Время отклика (мс)");
        plt.ShowLegend();
        
        plt.SavePng("latency_graph.png", 600, 400);
        Console.WriteLine("График сохранен в файл: latency_graph.png");
    }

    static double[] RunBlockingScenario()
    {
        double[] latencies = new double[5];
        var sw = Stopwatch.StartNew();
        
        Thread.Sleep(100);
        for (int i = 0; i < 5; i++)
        {
            Thread.Sleep(5);
            latencies[i] = sw.ElapsedMilliseconds;
        }
        return latencies;
    }

    static double[] RunSchedulerScenario()
    {
        double[] latencies = new double[5];
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();
        
        var sw = Stopwatch.StartNew();
        
        server.AddCommand(new LongRunningCommand(10));
        
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            server.AddCommand(new ShortCommand(() => latencies[index] = sw.ElapsedMilliseconds));
        }

        Thread.Sleep(200);
        server.Stop();
        return latencies;
    }
}