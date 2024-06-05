using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Engine
{

    // This is the more complex version. (undeterministic way - more randmoness but less clear code)
    public static class RandomNumberGenerator
    {
        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();
        
        public static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            _generator.GetBytes(randomNumber);
            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);
            // We are using Math.Max, and substracting 0.00000000001,
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;
            double randomValueInRange = Math.Floor(multiplier * range);// This line scales the multiplier by the size of the range, and then rounds down to the nearest whole number.
            return (int)(minimumValue + randomValueInRange);//This line shifts the scaled, rounded number into the desired range by adding the minimum value, and then returns the result.
        }


        // Simple version, with less randomness. (this is a deterministic way, and the user can figure out the pattern (less randomness but clearer code)
        private static readonly Random _simpleGenerator = new Random();

        public static int SimpleNumberBetween(int minimumValue, int maximumValue)
        {
            return _simpleGenerator.Next(minimumValue, maximumValue + 1);
        }
    }
}