public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Enter the Number of Inputs");
        if (!int.TryParse(Console.ReadLine(), out int numberOfCases) || numberOfCases < 0)
        {
            Console.WriteLine("Invalid input for number of inputs. Please enter a non-negative integer.");
            return;
        }

        Console.WriteLine($"Enter {numberOfCases} Inputs");
        List<int> testInputs = new List<int>();
        var result = new List<int>();

        int inputCount = numberOfCases;

        while (inputCount-- > 0)
        {
            string inputNumber = Console.ReadLine();
            if (int.TryParse(inputNumber, out int input))
            {
                testInputs.Add(input);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter an integer.");
                inputCount++;
            }
        }

        foreach (int number in testInputs)
        {
            try
            {
                result.Add(DivisorCountSolver.CountMatchingDivisorPairs(number));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error: " + exception.Message);
            }
        }

        Console.WriteLine("\n Results:");
        foreach (int answer in result)
        {
            Console.WriteLine(answer);
        }
    }
}

public class DivisorCountSolver
{
    public static int CountMatchingDivisorPairs(int number)
    {
        if (number < 0)
        {
            throw new Exception($"Input must be non-negative {number}.");
        }

        int matchingPairCount = 0;
        for (int i = 1; i < number; i++)
        {
            if (GetDivisorCount(i) == GetDivisorCount(i + 1))
            {
                matchingPairCount++;
            }
        }
        return matchingPairCount;
    }

    private static int GetDivisorCount(int number)
    {
        int divisorCount = 0;
        for (int i = 1; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0)
            {
                divisorCount += (i == number / i) ? 1 : 2;
            }
        }
        return divisorCount;
    }
}