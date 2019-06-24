﻿using System;
using SpiceSharpParser.Common.Evaluation;
using SpiceSharpParser.Common.Mathematics.Probability;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.Functions.Random
{
    public class ExtendedGaussFunction : Function<double, double>
    {
        public ExtendedGaussFunction()
        {
            Name = "gauss";
            ArgumentsCount = 3;
        }

        public override double Logic(string image, double[] args, IEvaluator evaluator, ExpressionContext context)
        {
            if (args.Length != 3)
            {
                throw new Exception("gauss expects three arguments: nominal_val, rel_variation and sigma");
            }

            IRandom random = context.Randomizer.GetRandom(context.Seed);

            double p1 = 1 - random.NextDouble();
            double p2 = 1 - random.NextDouble();

            double normal = System.Math.Sqrt(-2.0 * System.Math.Log(p1)) * System.Math.Sin(2.0 * System.Math.PI * p2);
            double nominal = args[0];
            double stdDev = args[1];
            double sigma = args[2];

            double stdVar = nominal * stdDev / sigma;

            return nominal + (stdVar * normal);
        }
    }
}
