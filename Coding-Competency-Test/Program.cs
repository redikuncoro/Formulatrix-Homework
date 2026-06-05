class Program
{
    static void Main(string[] args)
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
}