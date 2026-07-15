namespace CalcMachine
{
        public static class Trigonometric
        {
            public static void Open()
            {

                Console.WriteLine(@"
                Write the following commands for what the object is:

                |1 or IDS      - Gets the Sin, Cos, Tg, Sec, Cossec and Ctg.
                |2 or INVERSE  - Gets the Arcsin, Arccos and Arctan.
                |3 or BACK     - Goes Back to Main Menu.
                |__________________________________________________________");
                string? Switch = Console.ReadLine() ?? "0";
                Switch = Switch.ToUpper();

                switch(Switch)
                {
                case "IDS" or "1":
                    Ids();
                break;

                case "INVERSE" or "2":
                    Inverse();
                break;

                case "BACK" or "3":
                    Programa.MainMenu();
                break;               
                }
            
                if (Switch is not "BACK") { Trigonometric.Open(); }

            }

            public static void Ids()
            {
                Console.WriteLine("Write your angle (in degrees)");
                double angle = Programa.E.ParseDouble(Console.ReadLine());
                double radian = (angle * (Math.PI)) / 180; //converts the degrees to radians
                Programa.E.ClearLine(2, 0);
                Console.WriteLine($"{angle}° in radians is: {radian}");

                Console.WriteLine($"The Sine of {angle}° is {Math.Sin(radian)}");
                Console.WriteLine($"The Cosine of {angle}° is {Math.Cos(radian)}");
                Console.WriteLine($"The Tangent of {angle}° is {Math.Tan(radian)}");
                Console.WriteLine($"The Cosecant of {angle}° is {1/(Math.Sin(radian))}");
                Console.WriteLine($"The Secant of {angle}° is {1/(Math.Cos(radian))}");
                Console.WriteLine($"The Cotangent of {angle}° is {1/(Math.Tan(radian))}");                
            }

            public static void Inverse()
            {
                Console.WriteLine("Write your value (between -1 and 1 for arcsin/arccos)");
                double value = Programa.E.ParseDouble(Console.ReadLine());
                Programa.E.ClearLine(2, 0);

                double arcsinRad = Math.Asin(value);
                double arccosRad = Math.Acos(value);
                double arctanRad = Math.Atan(value);

                double arcsinDeg = (arcsinRad * 180) / Math.PI;
                double arccosDeg = (arccosRad * 180) / Math.PI;
                double arctanDeg = (arctanRad * 180) / Math.PI;

                Console.WriteLine($"The Arcsine of {value} is {arcsinDeg}° ({arcsinRad} radians)");
                Console.WriteLine($"The Arccosine of {value} is {arccosDeg}° ({arccosRad} radians)");
                Console.WriteLine($"The Arctangent of {value} is {arctanDeg}° ({arctanRad} radians)");
            }
        }

}