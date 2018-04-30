using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinforcementLearning
{
    public class RandomEngine
    {
        private static Random random = new Random();
        public static double NextDouble()
        {
            return random.NextDouble();
        }
    }
}
