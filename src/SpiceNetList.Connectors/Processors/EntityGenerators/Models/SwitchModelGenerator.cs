﻿using SpiceSharp.Circuits;
using SpiceSharp.Components;
using System.Collections.Generic;

namespace SpiceNetlist.SpiceSharpConnector.Processors.EntityGenerators.Models
{
    class SwitchModelGenerator : ModelGenerator
    {
        public override List<string> GetGeneratedTypes()
        {
            return new List<string>() { "sw", "csw" };
        }

        internal override Entity GenerateModel(string name, string type)
        {
            switch (type)
            {
                case "sw": return new VoltageSwitchModel(name);
                case "csw": return new CurrentSwitchModel(name);
            }
            return null;
        }
    }
}