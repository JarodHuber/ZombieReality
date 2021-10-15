using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Returns an int based on a skewed chance
    /// </summary>
    /// <param name="values">The number of values that could be returned</param>
    /// <param name="probabilities">Array of chances for each value</param>
    /// <returns>Returns value between 0 and values-1</returns>
    public static int SkewedNum(float[] probabilities) 
    {
        if (probabilities.Length == 0)
        {
            Debug.LogError("Don't pass through an empty aray");
            return -1;
        }

        decimal cur = 0;
        decimal rand = (decimal)probabilities[0];

        for (int x = 1; x < probabilities.Length; ++x)
            rand += (decimal)probabilities[x];

        rand *= (decimal)Random.value;

        for(int x = 0; x < probabilities.Length; ++x)
        {
            cur += (decimal)probabilities[x];

            if (rand > cur)
                continue;

            return x;
        }

        return probabilities.Length - 1;
    }
}
