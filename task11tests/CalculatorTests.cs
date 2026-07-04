using System;
using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        private const string InitialClassCode = @"
                            public class Calculator{
                                public int Add(int a, int b) => a + b;
                                public int Minus(int a, int b) => a - b;
                                public int Mul(int a, int b) => a * b;
                                public int Div(int a, int b) => a / b;
                            }";

        [Fact]
        public void Calculator_ShouldExecuteAllMethodsCorrectlyWithoutReflectionInvocations()
        {
            ICalculator calculator = CompilerService.CompileCalculator(InitialClassCode);

            Assert.Equal(15, calculator.Add(10, 5));
            Assert.Equal(5, calculator.Minus(10, 5));
            Assert.Equal(50, calculator.Mul(10, 5));
            Assert.Equal(2, calculator.Div(10, 5));
        }

        [Fact]
        public void Calculator_DivisionByZero_ShouldThrowStandardException()
        {
            ICalculator calculator = CompilerService.CompileCalculator(InitialClassCode);

            Assert.Throws<DivideByZeroException>(() => calculator.Div(10, 0));
        }
    }
}