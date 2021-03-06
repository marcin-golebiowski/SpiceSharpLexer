﻿namespace SpiceSharpParser.Models.Netlist.Spice.Objects
{
    /// <summary>
    /// A SPICE component.
    /// </summary>
    public class Component : Statement
    {
        public Component(string name, ParameterCollection pinsAndParameters, SpiceLineInfo lineInfo) 
            : base(lineInfo)
        {
            Name = name;
            PinsAndParameters = pinsAndParameters;
        }

        /// <summary>
        /// Gets or sets the name of component.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets pins and components parameters.
        /// </summary>
        public ParameterCollection PinsAndParameters { get; }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>A clone of the object.</returns>
        public override SpiceObject Clone()
        {
            return new Component(Name, PinsAndParameters, LineInfo);
        }
    }
}