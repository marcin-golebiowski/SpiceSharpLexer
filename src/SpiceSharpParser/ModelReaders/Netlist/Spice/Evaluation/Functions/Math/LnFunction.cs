﻿using SpiceSharpParser.Common.Evaluation;
using System;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.Functions.Math
{
    public class LnFunction : Function<double, double>
    {
        public LnFunction()
        {
            Name = "ln";
            ArgumentsCount = 1;
        }

        public override double Logic(string image, double[] args, EvaluationContext context)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("ln() function expects one argument");
            }

            double x = args[0];
            return System.Math.Log(x);
        }
    }
}