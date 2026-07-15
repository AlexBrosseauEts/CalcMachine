namespace CalcMachine
{
    public static class Solver
    {
        public static void Open()
        {
            Console.WriteLine(@"
            Equation Solver
            
            Enter an equation with one unknown variable (e.g., x+7=0 or 2*x-3=10)
            The solver will isolate the variable and give you the answer.
            
            Note: The unknown must appear exactly once in the equation.
            __________________________________________________________");
            
            Console.WriteLine("Enter the name of the unknown variable:");
            string? unknown = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(unknown))
            {
                Console.WriteLine("Invalid variable name.");
                return;
            }
            unknown = unknown.Trim().ToLower();
            
            Console.WriteLine($"Enter your equation (use '{unknown}' as the variable):");
            string? equation = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(equation))
            {
                Console.WriteLine("Invalid equation.");
                return;
            }
            
            try
            {
                double result = SolveEquation(equation, unknown);
                Console.WriteLine($"{unknown}={result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
            Solver.Open();
        }
        
        private static double SolveEquation(string equation, string unknown)
        {
            string[] sides = equation.Split('=');
            if (sides.Length != 2)
                throw new Exception("Equation must have exactly one '=' sign");
            
            string leftSide = sides[0].Trim();
            string rightSide = sides[1].Trim();
            
            int leftCount = CountOccurrences(leftSide, unknown);
            int rightCount = CountOccurrences(rightSide, unknown);
            
            if (leftCount + rightCount == 0)
                throw new Exception($"Variable '{unknown}' not found in equation");
            
            if (leftCount + rightCount > 1)
                throw new Exception($"Variable '{unknown}' appears more than once. Only single-occurrence equations are supported.");
            
            string unknownSide;
            string constantSide;
            
            if (leftCount == 1)
            {
                unknownSide = leftSide;
                constantSide = rightSide;
            }
            else
            {
                unknownSide = rightSide;
                constantSide = leftSide;
            }
            
            double constantValue = EvaluateExpression(constantSide, unknown, 0);
            
            return IsolateVariable(unknownSide, unknown, constantValue);
        }
        
        private static int CountOccurrences(string expression, string variable)
        {
            int count = 0;
            int index = 0;
            expression = expression.ToLower();
            variable = variable.ToLower();
            
            while ((index = expression.IndexOf(variable, index)) != -1)
            {
                bool isWholeWord = true;
                
                if (index > 0 && (char.IsLetterOrDigit(expression[index - 1]) || expression[index - 1] == '_'))
                    isWholeWord = false;
                
                if (index + variable.Length < expression.Length && 
                    (char.IsLetterOrDigit(expression[index + variable.Length]) || expression[index + variable.Length] == '_'))
                    isWholeWord = false;
                
                if (isWholeWord)
                    count++;
                
                index += variable.Length;
            }
            
            return count;
        }
        
        private static double IsolateVariable(string expression, string unknown, double equalTo)
        {
            expression = expression.Trim().Replace(" ", "").ToLower();
            unknown = unknown.ToLower();
            
            return IsolateRecursive(expression, unknown, equalTo);
        }
        
        private static double IsolateRecursive(string expr, string unknown, double target)
        {
            expr = expr.Trim();
            
            if (expr == unknown)
                return target;
            
            if (expr.StartsWith("(") && expr.EndsWith(")"))
            {
                string inner = expr.Substring(1, expr.Length - 2);
                return IsolateRecursive(inner, unknown, target);
            }
            
            if (expr.StartsWith("sin(") && expr.EndsWith(")"))
            {
                string inner = expr.Substring(4, expr.Length - 5);
                double newTarget = Math.Asin(target);
                return IsolateRecursive(inner, unknown, newTarget);
            }
            
            if (expr.StartsWith("cos(") && expr.EndsWith(")"))
            {
                string inner = expr.Substring(4, expr.Length - 5);
                double newTarget = Math.Acos(target);
                return IsolateRecursive(inner, unknown, newTarget);
            }
            
            if (expr.StartsWith("tan(") && expr.EndsWith(")"))
            {
                string inner = expr.Substring(4, expr.Length - 5);
                double newTarget = Math.Atan(target);
                return IsolateRecursive(inner, unknown, newTarget);
            }
            
            if (expr.StartsWith("log(") && expr.EndsWith(")"))
            {
                string inner = expr.Substring(4, expr.Length - 5);
                double newTarget = Math.Pow(10, target);
                return IsolateRecursive(inner, unknown, newTarget);
            }
            
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == '+' && GetParenDepth(expr, i) == 0)
                {
                    string left = expr.Substring(0, i);
                    string right = expr.Substring(i + 1);
                    
                    if (ContainsVariable(left, unknown))
                    {
                        double rightValue = EvaluateExpression(right, unknown, 0);
                        return IsolateRecursive(left, unknown, target - rightValue);
                    }
                    else
                    {
                        double leftValue = EvaluateExpression(left, unknown, 0);
                        return IsolateRecursive(right, unknown, target - leftValue);
                    }
                }
            }
            
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == '-' && i > 0 && GetParenDepth(expr, i) == 0)
                {
                    string left = expr.Substring(0, i);
                    string right = expr.Substring(i + 1);
                    
                    if (ContainsVariable(left, unknown))
                    {
                        double rightValue = EvaluateExpression(right, unknown, 0);
                        return IsolateRecursive(left, unknown, target + rightValue);
                    }
                    else
                    {
                        double leftValue = EvaluateExpression(left, unknown, 0);
                        return IsolateRecursive(right, unknown, leftValue - target);
                    }
                }
            }
            
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == '*' && GetParenDepth(expr, i) == 0)
                {
                    string left = expr.Substring(0, i);
                    string right = expr.Substring(i + 1);
                    
                    if (ContainsVariable(left, unknown))
                    {
                        double rightValue = EvaluateExpression(right, unknown, 0);
                        return IsolateRecursive(left, unknown, target / rightValue);
                    }
                    else
                    {
                        double leftValue = EvaluateExpression(left, unknown, 0);
                        return IsolateRecursive(right, unknown, target / leftValue);
                    }
                }
            }
            
            for (int i = expr.Length - 1; i >= 0; i--)
            {
                if (expr[i] == '/' && GetParenDepth(expr, i) == 0)
                {
                    string left = expr.Substring(0, i);
                    string right = expr.Substring(i + 1);
                    
                    if (ContainsVariable(left, unknown))
                    {
                        double rightValue = EvaluateExpression(right, unknown, 0);
                        return IsolateRecursive(left, unknown, target * rightValue);
                    }
                    else
                    {
                        double leftValue = EvaluateExpression(left, unknown, 0);
                        return IsolateRecursive(right, unknown, leftValue / target);
                    }
                }
            }
            
            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '^' && GetParenDepth(expr, i) == 0)
                {
                    string left = expr.Substring(0, i);
                    string right = expr.Substring(i + 1);
                    
                    if (ContainsVariable(left, unknown))
                    {
                        double rightValue = EvaluateExpression(right, unknown, 0);
                        return IsolateRecursive(left, unknown, Math.Pow(target, 1.0 / rightValue));
                    }
                    else
                    {
                        double leftValue = EvaluateExpression(left, unknown, 0);
                        return IsolateRecursive(right, unknown, Math.Log(target) / Math.Log(leftValue));
                    }
                }
            }
            
            throw new Exception($"Unable to isolate variable in expression: {expr}");
        }
        
        private static int GetParenDepth(string expr, int index)
        {
            int depth = 0;
            for (int i = 0; i < index; i++)
            {
                if (expr[i] == '(') depth++;
                if (expr[i] == ')') depth--;
            }
            return depth;
        }
        
        private static bool ContainsVariable(string expr, string variable)
        {
            return CountOccurrences(expr, variable) > 0;
        }
        
        private static double EvaluateExpression(string expression, string unknown, double value)
        {
            expression = expression.Replace(" ", "").ToLower().Replace(',', '.');
            int pos = 0;
            return ParseExpression(expression, ref pos);
        }

        private static double ParseExpression(string expr, ref int pos)
        {
            double left = ParseTerm(expr, ref pos);

            while (pos < expr.Length)
            {
                char op = expr[pos];
                if (op != '+' && op != '-')
                    break;
                
                pos++;
                double right = ParseTerm(expr, ref pos);
                
                if (op == '+')
                    left += right;
                else
                    left -= right;
            }

            return left;
        }

        private static double ParseTerm(string expr, ref int pos)
        {
            double left = ParseFactor(expr, ref pos);

            while (pos < expr.Length)
            {
                char op = expr[pos];
                if (op != '*' && op != '/' && op != 'x')
                    break;
                
                pos++;
                double right = ParseFactor(expr, ref pos);
                
                if (op == '*' || op == 'x')
                    left *= right;
                else
                    left /= right;
            }

            return left;
        }

        private static double ParseFactor(string expr, ref int pos)
        {
            double left = ParsePower(expr, ref pos);
            return left;
        }

        private static double ParsePower(string expr, ref int pos)
        {
            double left = ParseUnary(expr, ref pos);

            while (pos < expr.Length && expr[pos] == '^')
            {
                pos++;
                double right = ParsePower(expr, ref pos);
                left = Math.Pow(left, right);
            }

            return left;
        }

        private static double ParseUnary(string expr, ref int pos)
        {
            if (pos < expr.Length && expr[pos] == '-')
            {
                pos++;
                return -ParseUnary(expr, ref pos);
            }
            if (pos < expr.Length && expr[pos] == '+')
            {
                pos++;
                return ParseUnary(expr, ref pos);
            }
            return ParsePrimary(expr, ref pos);
        }

        private static double ParsePrimary(string expr, ref int pos)
        {
            if (pos >= expr.Length)
                throw new Exception("Unexpected end of expression");

            if (expr[pos] == '(')
            {
                pos++;
                double result = ParseExpression(expr, ref pos);
                if (pos >= expr.Length || expr[pos] != ')')
                    throw new Exception("Missing closing parenthesis");
                pos++;
                return result;
            }

            if (pos + 3 <= expr.Length && expr.Substring(pos, 3) == "sin")
            {
                pos += 3;
                if (pos >= expr.Length || expr[pos] != '(')
                    throw new Exception("sin requires format: sin(value)");
                pos++;
                double value = ParseExpression(expr, ref pos);
                if (pos >= expr.Length || expr[pos] != ')')
                    throw new Exception("Missing closing parenthesis for sin");
                pos++;
                return Math.Sin(value);
            }

            if (pos + 3 <= expr.Length && expr.Substring(pos, 3) == "cos")
            {
                pos += 3;
                if (pos >= expr.Length || expr[pos] != '(')
                    throw new Exception("cos requires format: cos(value)");
                pos++;
                double value = ParseExpression(expr, ref pos);
                if (pos >= expr.Length || expr[pos] != ')')
                    throw new Exception("Missing closing parenthesis for cos");
                pos++;
                return Math.Cos(value);
            }

            if (pos + 3 <= expr.Length && expr.Substring(pos, 3) == "tan")
            {
                pos += 3;
                if (pos >= expr.Length || expr[pos] != '(')
                    throw new Exception("tan requires format: tan(value)");
                pos++;
                double value = ParseExpression(expr, ref pos);
                if (pos >= expr.Length || expr[pos] != ')')
                    throw new Exception("Missing closing parenthesis for tan");
                pos++;
                return Math.Tan(value);
            }

            if (pos + 3 <= expr.Length && expr.Substring(pos, 3) == "log")
            {
                pos += 3;
                if (pos >= expr.Length || expr[pos] != '(')
                    throw new Exception("log requires format: log(value) or log(value:base)");
                pos++;
                double value = ParseExpression(expr, ref pos);
                
                if (pos < expr.Length && expr[pos] == ':')
                {
                    pos++;
                    double baseValue = ParseExpression(expr, ref pos);
                    if (pos >= expr.Length || expr[pos] != ')')
                        throw new Exception("Missing closing parenthesis for log");
                    pos++;
                    return Math.Log(value, baseValue);
                }
                else if (pos < expr.Length && expr[pos] == ')')
                {
                    pos++;
                    return Math.Log10(value);
                }
                else
                {
                    throw new Exception("Invalid log format");
                }
            }

            int start = pos;
            if (expr[pos] == '-' || expr[pos] == '+')
                pos++;

            bool hasDigit = false;
            bool hasDot = false;
            while (pos < expr.Length && (char.IsDigit(expr[pos]) || expr[pos] == '.' || expr[pos] == ','))
            {
                if (expr[pos] == '.' || expr[pos] == ',')
                {
                    if (hasDot)
                        throw new Exception("Invalid number format");
                    hasDot = true;
                }
                else
                {
                    hasDigit = true;
                }
                pos++;
            }

            if (!hasDigit)
                throw new Exception($"Expected number at position {start}");

            string numberStr = expr.Substring(start, pos - start).Replace(',', '.');
            return double.Parse(numberStr, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
