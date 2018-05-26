﻿using System;
using System.Collections.Generic;
using SpiceSharpParser.ModelReader.Spice.Context;
using SpiceSharpParser.ModelReader.Spice.Exceptions;
using SpiceSharpParser.ModelReader.Spice.Processors;

namespace SpiceSharpParser.ModelReader.Spice.Evaluation.CustomFunctions
{
    public class RandomFunctions
    {
        /// <summary>
        /// Creates random custom functions.
        /// </summary>
        public static IEnumerable<KeyValuePair<string, SpiceFunction>> Create()
        {
            var result = new List<KeyValuePair<string, SpiceFunction>>();
            result.Add(new KeyValuePair<string, SpiceFunction>("random", CreateRandom()));
            return result;
        }

        /// <summary>
        /// Create a random() user function.
        /// </summary>
        /// <returns>
        /// A new instance of random spice function.
        /// </returns>
        public static SpiceFunction CreateRandom()
        {
            Random randomGenerator = new Random(Environment.TickCount);

            SpiceFunction function = new SpiceFunction();
            function.Name = "random";
            function.VirtualParameters = false;
            function.ArgumentsCount = 0;

            function.Logic = (args, simulation) =>
            {
                if (args.Length != 0)
                {
                    throw new Exception("random expects no arguments");
                }
                return randomGenerator.NextDouble();
            };

            return function;
        }
    }
}
