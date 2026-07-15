namespace CalcMachine
{
            public static class Basic
        {
             public static void Open()
            {

                Console.WriteLine(@"
                Write the following commands for what your needs are:

                |1 or SIMPLE - Does only one operation.
                |2 or FULL   - Does complex expressions with parentheses.
                |3 or BACK   - Goes Back to Main Menu.
                |__________________________________________________________");
                string? Switch = Console.ReadLine() ?? "0";
                Switch = Switch.ToUpper();

                Console.WriteLine(@"
                Symbol Sheet:

                |+   -> a + b = c
                |-   -> a - b = c
                |*   -> a * b = c
                |/   -> a / b = c
                |^   -> a ^ b = c
                |Log -> log(value:base) = C
                |
                |Decimals: Use either . or , (e.g., 3.14 or 3,14)
                |
                |Examples:
                |  Simple: 1+1
                |  Full:   (1+1)*2
                |  Log:    log(8:2)
                |__________________________________________________________");

                switch(Switch)
                {
                case "SIMPLE" or "1":
                    Simple();
                break;

                case "FULL" or "2":
                    Full();
                break;

                case "BACK" or "3":
                    Programa.MainMenu();
                break;
                
                default:
                break;               
                }
            
                if (Switch is not "BACK" && Switch is not "3") { Basic.Open(); }

            }

             public static void Simple()
             {
                Console.WriteLine("Enter your expression (e.g., 1+1):");
                string? expression = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(expression))
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                try
                {
                    double result = EvaluateExpression(expression);
                    Console.WriteLine($"{expression} = {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
             }

             public static void Full()
             {
                Console.WriteLine("Enter your expression with parentheses (e.g., (1+1)*2):");
                string? expression = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(expression))
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                try
                {
                    double result = EvaluateExpression(expression);
                    Console.WriteLine($"{expression} = {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
             }

             private static double EvaluateExpression(string expression)
             {
                expression = expression.Replace(" ", "").ToLower();
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

                if (pos + 3 <= expr.Length && expr.Substring(pos, 3) == "log")
                {
                    pos += 3;
                    if (pos >= expr.Length || expr[pos] != '(')
                        throw new Exception("log requires format: log(value:base)");
                    pos++;
                    double value = ParseExpression(expr, ref pos);
                    if (pos >= expr.Length || expr[pos] != ':')
                        throw new Exception("log requires format: log(value:base)");
                    pos++;
                    double baseValue = ParseExpression(expr, ref pos);
                    if (pos >= expr.Length || expr[pos] != ')')
                        throw new Exception("Missing closing parenthesis for log");
                    pos++;
                    return Math.Log(value, baseValue);
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