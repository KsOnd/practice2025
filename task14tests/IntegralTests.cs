using System;
using Xunit;
using task14;

namespace task14tests
{
    public class IntegralTests
    {
        Func<double, double> sinFunction = Math.Sin;
        Func<double, double> xFunction = x => x;

        [Fact]
        public void Solve_LinearFunction_ReturnsZeroForSymmetricInterval()
        {
            double result = DefiniteIntegral.Solve(-1, 1, xFunction, 1e-4, 2);

            Assert.Equal(0, result, precision: 4);
        }

        [Fact]
        public void Solve_SinFunction_ReturnsZeroForSymmetricInterval()
        {
            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, sinFunction, 1e-4, 2), 1e-4);

            Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, sinFunction, 1e-5, 8), 1e-4);
        }

        [Fact]
        public void Solve_LinearFunction_ReturnsCorrectArea()
        {
            double expected = 12.5; 
            double result = DefiniteIntegral.Solve(0, 5, xFunction, 1e-6, 8);

            Assert.Equal(expected, result, precision: 5);
        }

        [Fact]
        public void Solve_ThrowsArgumentException_WhenThreadsNumberIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => DefiniteIntegral.Solve(0, 1, xFunction, 1e-3, 0));
        }
    }
}