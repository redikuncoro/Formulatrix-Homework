class GeneratorLogic
{
    private readonly SortedDictionary<int, string> _rules = new SortedDictionary<int, string>();

    public void AddRule(int divisor, string output)
    {
        if (divisor <= 0)
        {
            throw new ArgumentException("Divisor must be a positive integer.");
        }
        if (string.IsNullOrEmpty(output))
        {
            throw new ArgumentException("Output cannot be null or empty.");
        }
        _rules[divisor] = output;
    }

    public SortedDictionary<int, string> GetRules()
    {
        return _rules;
    }

    public void RemoveRule(int divisor)
    {
        if (!_rules.ContainsKey(divisor))
        {
            throw new KeyNotFoundException("Divisor not found in rules.");
        }
        _rules.Remove(divisor);
    }

    public void ClearRules()
    {
        _rules.Clear();
    }

    public List<string> Generate(int number)
    {
        if (number <= 0)
        {
            throw new ArgumentException("Number must be a positive integer.");
        }

        List<string> results = new List<string>();

        for (int x = 1; x <= number; x++)
        {
            string result = "";

            foreach (var rule in _rules)
            {
                if (x % rule.Key == 0)
                {
                    result += rule.Value;
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                results.Add(x.ToString());
            }
            else
            {
                results.Add(result);
            }
        }

        return results;
    }

    public void PrintResults(List<string> results)
    {
        Console.WriteLine(string.Join(", ", results));
    }
}