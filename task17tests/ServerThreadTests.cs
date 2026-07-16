using Xunit;
using Task17;
using System.Threading;

namespace Task17.Tests;

public class ServerTests
{
    [Fact]
    public void Scheduler_AllowsShortCommands_ToExecuteDuringLongCommand()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        server.Start();

        bool shortCommandDone = false;
        var longCommand = new LongRunningCommand(10);
        var shortCommand = new ShortCommand(() => shortCommandDone = true);

        server.AddCommand(longCommand);
        server.AddCommand(shortCommand);

        Thread.Sleep(30);

        Assert.True(shortCommandDone);
        server.Stop();
    }
}