﻿using System;
using SpiceSharp.Simulations;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Context;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Exceptions;
using SpiceSharpParser.Models.Netlist.Spice.Objects;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Simulations
{
    using SpiceSharpParser.ModelReaders.Netlist.Spice.Mappings;
    using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters;

    /// <summary>
    /// Reads .AC <see cref="Control"/> from SPICE netlist object model.
    /// </summary>
    public class ACControl : SimulationControl
    {
        public ACControl(IMapper<Exporter> mapper)
            : base(mapper)
        {

        }

        /// <summary>
        /// Reads <see cref="Control"/> statement and modifies the context
        /// </summary>
        /// <param name="statement">A statement to process</param>
        /// <param name="context">A context to modify</param>
        public override void Read(Control statement, IReadingContext context)
        {
            CreateSimulations(statement, context, CreateACSimulation);
        }

        private AC CreateACSimulation(string name, Control statement, IReadingContext context)
        {
            switch (statement.Parameters.Count)
            {
                case 0: throw new Exception("LIN, DEC or OCT expected");
                case 1: throw new Exception("Number of points expected");
                case 2: throw new Exception("Starting frequency expected");
                case 3: throw new Exception("Stopping frequency expected");
            }

            AC ac;

            string type = statement.Parameters.GetString(0).ToLower();
            var numberSteps = context.EvaluateDouble(statement.Parameters.GetString(1));
            var start = context.EvaluateDouble(statement.Parameters.GetString(2));
            var stop = context.EvaluateDouble(statement.Parameters.GetString(3));

            switch (type)
            {
                case "lin": ac = new AC(name, new LinearSweep(start, stop, (int)numberSteps)); break;
                case "oct": ac = new AC(name, new OctaveSweep(start, stop, (int)numberSteps)); break;
                case "dec": ac = new AC(name, new DecadeSweep(start, stop, (int)numberSteps)); break;
                default:
                    throw new WrongParameterException("LIN, DEC or OCT expected");
            }

            ConfigureCommonSettings(ac, context);
            ConfigureAcSettings(ac.Configurations.Get<FrequencyConfiguration>(), context);

            ac.BeforeFrequencyLoad += (object sender, LoadStateEventArgs args) =>
                {
                    if (ac.ComplexState != null)
                    {
                        var freq = ac.ComplexState.Laplace.Imaginary / (2.0 * Math.PI);
                        context.SimulationExpressionContexts.GetContext(ac).SetParameter("FREQ", freq);
                    }
                };

            context.Result.AddSimulation(ac);
            return ac;
        }

        private void ConfigureAcSettings(FrequencyConfiguration frequencyConfiguration, IReadingContext context)
        {
            if (context.Result.SimulationConfiguration.KeepOpInfo.HasValue)
            {
                frequencyConfiguration.KeepOpInfo = context.Result.SimulationConfiguration.KeepOpInfo.Value;
            }
        }
    }
}
