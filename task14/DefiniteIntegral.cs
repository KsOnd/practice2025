using System;
using System.Threading;

namespace task14
{
    public static class DefiniteIntegral
    {
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            ArgumentNullException.ThrowIfNull(function);

            if (threadsNumber <= 0)
                throw new ArgumentException("Количество потоков должно быть больше нуля.", nameof(threadsNumber));
            
            if (step <= 0)
                throw new ArgumentException("Размер шага должен быть строго положительным.", nameof(step));

            int totalSteps = (int)Math.Round((b - a) / step);

            if (totalSteps <= 0) return 0.0;

            double totalResult = 0.0;
            using var barrier = new Barrier(threadsNumber + 1);
            int stepsPerThread = totalSteps / threadsNumber;
            int remainder = totalSteps % threadsNumber;

            for (int k = 0; k < threadsNumber; k++)
            {
                int threadIndex = k;
                int startStep = threadIndex * stepsPerThread + Math.Min(threadIndex, remainder);
                int endStep = startStep + stepsPerThread + (threadIndex < remainder ? 1 : 0);

                var thread = new Thread(() =>
                {
                    double localSum = 0.0;

                    for (int i = startStep; i < endStep; i++)
                    {
                        double x1 = a + i * step;
                        double x2 = a + (i + 1) * step;

                        if (i == totalSteps - 1)
                        {
                            x2 = b;
                        }

                        localSum += (function(x1) + function(x2)) * (x2 - x1) / 2.0;
                    }

                    double initialValue, newValue;
                    do
                    {
                        initialValue = totalResult;
                        newValue = initialValue + localSum;
                    } while (initialValue != Interlocked.CompareExchange(ref totalResult, newValue, initialValue));

                    barrier.SignalAndWait();
                });

                thread.Start();
            }
            barrier.SignalAndWait();

            return totalResult;
        }
    }
}