using UnityEngine;
using System;

/// <summary>
/// simulating random event success 
/// </summary>
public class DiceRoll
{
    /// <summary>
    /// get percentage chance result limit 0.0000001%
    /// </summary>
    /// <param name="percentage">require percentage</param>
    /// <returns>reuslt dice rolled</returns>
    public static bool RollResult(double percentage)
    {
        bool success = false;

        //high limit
        if (percentage > 100f)
        {
            percentage = 100f;
        }

        //under limit
        if (percentage < 0.0000001f)
        {
            percentage = 0.0000001f;
        }

        //get percentage
        percentage *= 0.01f;

        int higherLimit = 10000000;
        double successRange = percentage * higherLimit;
        int Rand = UnityEngine.Random.Range(1, higherLimit + 1);

        //check success or fail
        if (Rand <= successRange)
        {
            success = true;
        }

        return success;
    }
    
    /// <summary>
    /// get random range in double
    /// </summary>
    /// <param name="minDouble">include min value</param>
    /// <param name="maxDouble">include max value</param>
    /// <returns></returns>
    public static double RandomDouble(double minDouble, double maxDouble)
    {
        System.Random rand = new System.Random();

        //create random double
        double randDouble = rand.NextDouble();

        //range picking
        double rangeDouble = randDouble * (maxDouble - minDouble) + minDouble;

        return rangeDouble;
    }
}


