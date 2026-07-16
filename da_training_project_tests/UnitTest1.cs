using System;
using System.Text.RegularExpressions;
using Xunit;

namespace CalcMachineTests
{
    public class BasicSimpleTests
    {
        private string RunSimple(string expression)
        {
            return TestRunner.RunWithTimeout($"1\n1\n{expression}\n3\n9\n");
        }

        [Fact]
        public void Simple_SingleReadline_Addition()
        {
            var output = RunSimple("1+1");
            Assert.Contains("2", output);
        }

        [Fact]
        public void Simple_SingleReadline_Subtraction()
        {
            var output = RunSimple("5-3");
            Assert.Contains("2", output);
        }

        [Fact]
        public void Simple_SingleReadline_Multiplication()
        {
            var output = RunSimple("3*4");
            Assert.Contains("12", output);
        }

        [Fact]
        public void Simple_SingleReadline_Division()
        {
            var output = RunSimple("10/2");
            Assert.Contains("5", output);
        }

        [Fact]
        public void Simple_SingleReadline_Power()
        {
            var output = RunSimple("2^3");
            Assert.Contains("8", output);
        }

        [Fact]
        public void Simple_SingleReadline_Log()
        {
            var output = RunSimple("log(100:10)");
            Assert.Contains("2", output);
        }

        [Fact]
        public void Simple_UsesOnlyOneReadlineForInput()
        {
            var output = RunSimple("3+7");
            Assert.Contains("10", output);
        }
    }

    public class BasicFullTests
    {
        private string RunFull(string expression)
        {
            return TestRunner.RunWithTimeout($"1\n2\n{expression}\n3\n9\n");
        }

        [Fact]
        public void Full_NoHowManyOperationsPrompt()
        {
            var output = RunFull("(1+1)*2");
            Assert.DoesNotContain("how many operations", output.ToLower());
        }

        [Fact]
        public void Full_ParenthesesPrecedence()
        {
            var output = RunFull("(1+1)*2");
            Assert.Contains("4", output);
        }

        [Fact]
        public void Full_OperatorPrecedence_MultiplicationBeforeAddition()
        {
            var output = RunFull("2+3*4");
            Assert.Contains("14", output);
        }

        [Fact]
        public void Full_OperatorPrecedence_DivisionBeforeSubtraction()
        {
            var output = RunFull("10-6/3");
            Assert.Contains("8", output);
        }

        [Fact]
        public void Full_OperatorPrecedence_PowerBeforeMultiplication()
        {
            var output = RunFull("2*3^2");
            Assert.Contains("18", output);
        }

        [Fact]
        public void Full_NestedParentheses()
        {
            var output = RunFull("((2+3)*2)+1");
            Assert.Contains("11", output);
        }

        [Fact]
        public void Full_ComplexExpression()
        {
            var output = RunFull("(4+6)/(2+3)");
            Assert.Contains("2", output);
        }

        [Fact]
        public void Full_SingleInputExpression()
        {
            var output = RunFull("5+5");
            Assert.Contains("10", output);
        }
    }

    public class EuropeanDecimalFormatTests
    {
        private string RunSimple(string expression)
        {
            return TestRunner.RunWithTimeout($"1\n1\n{expression}\n3\n9\n");
        }

        private string RunFull(string expression)
        {
            return TestRunner.RunWithTimeout($"1\n2\n{expression}\n3\n9\n");
        }

        [Fact]
        public void Simple_CommaAsDecimalSeparator()
        {
            var output = RunSimple("1,5+2,5");
            Assert.Contains("4", output);
        }

        [Fact]
        public void Simple_DotAsDecimalSeparator()
        {
            var output = RunSimple("1.5+2.5");
            Assert.Contains("4", output);
        }

        [Fact]
        public void Simple_CommaAndDotBothWork()
        {
            var output = RunSimple("1,5+2.5");
            Assert.Contains("4", output);
        }

        [Fact]
        public void Full_CommaDecimalWithParentheses()
        {
            var output = RunFull("(1,5+1,5)*2");
            Assert.Contains("6", output);
        }

        [Fact]
        public void ColonAsSeparator_InLogExpression()
        {
            var output = RunSimple("log(100:10)");
            Assert.Contains("2", output);
        }

        [Fact]
        public void Full_DotDecimalInComplexExpression()
        {
            var output = RunFull("2.5*4");
            Assert.Contains("10", output);
        }
    }

    public class TrigonometricTests
    {
        private string RunTrigIds(string angle)
        {
            return TestRunner.RunWithTimeout($"2\n1\n{angle}\n");
        }

        private string RunTrigInverse(string angle)
        {
            return TestRunner.RunWithTimeout($"2\n2\n{angle}\n");
        }

        [Fact]
        public void Trig_SinCosTan_StillWork()
        {
            var output = RunTrigIds("30");
            Assert.Contains("Sine", output);
            Assert.Contains("Cosine", output);
            Assert.Contains("Tangent", output);
        }

        [Fact]
        public void Trig_ArcSin_Exists()
        {
            var output = RunTrigInverse("0.5");
            Assert.Contains("arcsin", output.ToLower());
        }

        [Fact]
        public void Trig_ArcCos_Exists()
        {
            var output = RunTrigInverse("0.5");
            Assert.Contains("arccos", output.ToLower());
        }

        [Fact]
        public void Trig_ArcTan_Exists()
        {
            var output = RunTrigInverse("0.5");
            Assert.Contains("arctan", output.ToLower());
        }

        [Fact]
        public void Trig_ArcSin_CorrectValue()
        {
            var output = RunTrigInverse("0.5");
            double arcsinResult = Math.Asin(0.5) * 180 / Math.PI;
            Assert.Contains(Math.Round(arcsinResult, 2).ToString().Substring(0, 2), output);
        }

        [Fact]
        public void Trig_EuropeanDecimal_Angle()
        {
            var output = RunTrigIds("30,5");
            Assert.Contains("Sine", output);
        }
    }

    public class SolverTests
    {
        private string RunSolver(string equation, string unknown)
        {
            return TestRunner.RunWithTimeout($"SOLVER\n{unknown}\n{equation}\n9\n");
        }

        [Fact]
        public void Solver_SimpleLinearEquation()
        {
            var output = RunSolver("x+7=0", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("-7", output);
        }

        [Fact]
        public void Solver_MultiplicationIsolation()
        {
            var output = RunSolver("2*x=5", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("2.5", output.Replace(",", "."));
        }

        [Fact]
        public void Solver_DivisionIsolation()
        {
            var output = RunSolver("x/2=3", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("6", output);
        }

        [Fact]
        public void Solver_SubtractionIsolation()
        {
            var output = RunSolver("x-3=7", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("10", output);
        }

        [Fact]
        public void Solver_PowerIsolation()
        {
            var output = RunSolver("x^2=9", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("3", output);
        }

        [Fact]
        public void Solver_LogIsolation()
        {
            var output = RunSolver("log(x)=2", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("100", output);
        }

        [Fact]
        public void Solver_LogBase10()
        {
            var output = RunSolver("log(x)=1", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("10", output);
        }

        [Fact]
        public void Solver_SinIsolation()
        {
            var output = RunSolver("sin(x)=0", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("0", output);
        }

        [Fact]
        public void Solver_CosIsolation()
        {
            var output = RunSolver("cos(x)=1", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("0", output);
        }

        [Fact]
        public void Solver_TanIsolation()
        {
            var output = RunSolver("tan(x)=0", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("0", output);
        }

        [Fact]
        public void Solver_WithOtherSideCalculation()
        {
            var output = RunSolver("x+3=2+5", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("4", output);
        }

        [Fact]
        public void Solver_AsksForUnknown()
        {
            var output = RunSolver("a+7=0", "a");
            Assert.Contains("unknown", output.ToLower());
        }

        [Fact]
        public void Solver_OutputFormat_UnknownEqualsResult()
        {
            var output = RunSolver("x+7=0", "x");
            Assert.Contains("x=-7", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_GivesDecimals()
        {
            var output = RunSolver("3*x=10", "x");
            Assert.Matches(@"x=3[.,]3+", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_RejectMultipleUnknowns_ErrorMessage()
        {
            var output = RunSolver("x^2+x=5", "x");
            Assert.Contains("error", output.ToLower());
        }

        [Fact]
        public void Solver_RejectMultipleUnknowns_ExplainsReason()
        {
            var output = RunSolver("x^2+x=5", "x");
            Assert.True(
                output.ToLower().Contains("more than one") ||
                output.ToLower().Contains("multiple") ||
                output.ToLower().Contains("once"),
                "Error message should explain the cause of rejection"
            );
        }

        [Fact]
        public void Solver_UsesBuiltInOperations()
        {
            var output = RunSolver("2*x+1=7", "x");
            Assert.Contains("x=3", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_ComplexEquation_WithLog()
        {
            var output = RunSolver("log(x)+1=3", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
            Assert.Contains("100", output);
        }

        [Fact]
        public void Solver_EuropeanDecimalFormat()
        {
            var output = RunSolver("x+1,5=3,5", "x");
            Assert.Contains("x=2", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_ColonSeparatorInLog()
        {
            var output = RunSolver("7*log(4:5,6)+log(x)=cos(6)", "x");
            Assert.Contains("x=", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_RequiresExplicitMultiplication()
        {
            var output = RunSolver("2*x=10", "x");
            Assert.Contains("x=5", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_ArcsinInEquation()
        {
            var output = RunSolver("arcsin(x)=0", "x");
            Assert.Contains("x=0", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_ArccosInEquation()
        {
            var output = RunSolver("arccos(x)=0", "x");
            Assert.Contains("x=1", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_ArctanInEquation()
        {
            var output = RunSolver("arctan(x)=0", "x");
            Assert.Contains("x=0", output.Replace(" ", ""));
        }

        [Fact]
        public void Solver_InvalidEquation_ErrorWithCause()
        {
            var output = RunSolver("invalid", "x");
            Assert.Contains("error", output.ToLower());
        }
    }

    public class SolverMenuIntegrationTests
    {
        [Fact]
        public void Solver_AccessibleFromMainMenu()
        {
            var output = TestRunner.RunWithTimeout("SOLVER\nx\nx=5\n9\n");
            Assert.True(
                output.ToLower().Contains("solver") || output.Contains("x="),
                "Solver should be accessible from the main menu"
            );
        }
    }
}
