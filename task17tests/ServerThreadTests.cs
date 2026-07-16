using System;
using System.Threading;
using Xunit;
using Task17;

namespace Task17.Tests
{
    public class ServerThreadTests
    {
        private class DummyExceptionHandler : IExceptionHandler
        {
            public void Handle(ICommand command, Exception exception) { }
        }

        // Команда-счетчик для проверки количества реально выполненных задач
        private class CountdownCommand : ICommand
        {
            private readonly CountdownEvent _countdown;
            public CountdownCommand(CountdownEvent countdown) => _countdown = countdown;
            public void Execute() => _countdown.Signal();
        }

        [Fact]
        public void HardStop_ShouldStopThreadImmediately_AndDiscardRemainingQueue()
        {
            var handler = new DummyExceptionHandler();
            var server = new ServerThread(handler);
            server.Start();

            var countdown = new CountdownEvent(2);
            var firstCmd = new CountdownCommand(countdown);
            var hardStop = new HardStopCommand(server);
            var remainingCmd = new CountdownCommand(countdown);

            server.AddCommand(firstCmd);
            server.AddCommand(hardStop);
            server.AddCommand(remainingCmd);

            bool threadExited = server.Thread.Join(2000);

            Assert.True(threadExited, "Поток не завершил работу вовремя при HardStop.");
            Assert.False(server.IsRunning);
            Assert.Equal(1, countdown.CurrentCount);
        }

        [Fact]
        public void SoftStop_ShouldExecuteAllQueuedCommands_BeforeStopping()
        {
            var handler = new DummyExceptionHandler();
            var server = new ServerThread(handler);
            server.Start();

            var countdown = new CountdownEvent(2);
            var firstCmd = new CountdownCommand(countdown);
            var softStop = new SoftStopCommand(server);
            var remainingCmd = new CountdownCommand(countdown);

            server.AddCommand(firstCmd);
            server.AddCommand(softStop);
            server.AddCommand(remainingCmd);

            bool threadExited = server.Thread.Join(2000);

            Assert.True(threadExited, "Поток не завершил работу вовремя при SoftStop.");
            Assert.False(server.IsRunning);
            Assert.Equal(0, countdown.CurrentCount);
        }

        [Fact]
        public void HardStopCommand_ShouldThrowException_WhenExecutedFromOutsideThread()
        {
            var handler = new DummyExceptionHandler();
            var server = new ServerThread(handler);
            var hardStop = new HardStopCommand(server);

            Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
        }

        [Fact]
        public void SoftStopCommand_ShouldThrowException_WhenExecutedFromOutsideThread()
        {
            var handler = new DummyExceptionHandler();
            var server = new ServerThread(handler);
            var softStop = new SoftStopCommand(server);

            Assert.Throws<InvalidOperationException>(() => softStop.Execute());
        }
    }
}