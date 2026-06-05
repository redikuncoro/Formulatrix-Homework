class Program
{
    static void Main(string[] args)
    {
        // make a decission based user input to use generator or use existing hardcoded rules
        Console.WriteLine("Do you want to use the generator logic? (y/n)");
        string choice = Console.ReadLine();
        if (choice.ToLower() == "y")
        {
            GeneratorLogic generator = new GeneratorLogic();
            // generator.AddRule(3, "foo");
            // generator.AddRule(4, "baz");
            // generator.AddRule(5, "bar");
            // generator.AddRule(7, "jazz");
            // generator.AddRule(9, "huzz");

            // generate random rules
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                int divisor = rand.Next(3, 20);
                string output = "rule" + divisor;
                generator.AddRule(divisor, output);
            }
            generator.AddRule(3, "foo");
            generator.AddRule(4, "baz");
            generator.RemoveRule(3);

            Console.WriteLine("Generated Rules:");
            foreach (var rule in generator.GetRules())
            {
                Console.WriteLine($"Divisor: {rule.Key}, Output: {rule.Value}");
            }

            Console.WriteLine("Input N :");
            string input = Console.ReadLine();
            int number;
            if (!int.TryParse(input, out number))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                return;
            }

            List<string> results = generator.Generate(number);
            generator.PrintResults(results);
        }
        else if (choice.ToLower() == "n")
        {
            Console.WriteLine("Input N :");
            string input = Console.ReadLine();
            int number;
            if (!int.TryParse(input, out number))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer.");
                return;
            }
            
            List<string> results = new List<string>();

            for (int x = 1; x <= number; x++)
            {
                string result = "";

                if (x % 3 == 0) result += "foo";
                if (x % 4 == 0) result += "baz";
                if (x % 5 == 0) result += "bar";
                if (x % 7 == 0) result += "jazz";
                if (x % 9 == 0) result += "huzz";

                if (string.IsNullOrEmpty(result))
                {
                    results.Add(x.ToString());
                }
                else
                {
                    results.Add(result);
                }
            }

            Console.WriteLine(string.Join(", ", results));
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter 'y' or 'n'.");
        }
    }
}