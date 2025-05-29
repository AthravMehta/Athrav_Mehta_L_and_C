namespace DivisorCount
{
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
}
