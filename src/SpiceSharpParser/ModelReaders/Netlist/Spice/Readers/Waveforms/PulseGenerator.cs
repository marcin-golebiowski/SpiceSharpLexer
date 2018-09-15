﻿using SpiceSharp.Components;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Context;
using SpiceSharpParser.Models.Netlist.Spice.Objects.Parameters;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Waveforms
{
    /// <summary>
    /// Generator for pulse waveform
    /// </summary>
    public class PulseGenerator : WaveformGenerator
    {
        /// <summary>
        /// Generats a new waveform
        /// </summary>
        /// <param name="bracketParam">A parameter for waveform</param>
        /// <param name="context">A context</param>
        /// <returns>
        /// A new waveform
        /// </returns>
        public override Waveform Generate(BracketParameter bracketParam, IReadingContext context)
        {
            var w = new Pulse();

            if (bracketParam.Parameters.Count != 7)
            {
                throw new System.Exception("Wrong number of arguments for pulse");
            }

            w.InitialValue.Value = context.ParseDouble(bracketParam.Parameters.GetString(0));
            w.PulsedValue.Value = context.ParseDouble(bracketParam.Parameters.GetString(1));
            w.Delay.Value = context.ParseDouble(bracketParam.Parameters.GetString(2));
            w.RiseTime.Value = context.ParseDouble(bracketParam.Parameters.GetString(3));
            w.FallTime.Value = context.ParseDouble(bracketParam.Parameters.GetString(4));
            w.PulseWidth.Value = context.ParseDouble(bracketParam.Parameters.GetString(5));
            w.Period.Value = context.ParseDouble(bracketParam.Parameters.GetString(6));

            return w;
        }
    }
}
