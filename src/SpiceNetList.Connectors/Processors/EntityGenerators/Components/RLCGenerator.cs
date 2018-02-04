﻿using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceObjects.Parameters;
using SpiceSharp;
using SpiceSharp.Circuits;
using SpiceSharp.Components;
using System.Collections.Generic;

namespace SpiceNetlist.SpiceSharpConnector.Processors.EntityGenerators.Components
{
    public class RLCGenerator : EntityGenerator
    {
        public override Entity Generate(string name, string type, ParameterCollection parameters, NetList currentNetList)
        {
            switch (type)
            {
                case "r": return GenerateRes(name, parameters, currentNetList);
                case "l": return GenerateInd(name, parameters, currentNetList);
                case "c": return GenerateCap(name, parameters, currentNetList);
                case "k": return GenerateMut(name, parameters, currentNetList);
            }
            return null;
        }

        private Entity GenerateMut(string name, ParameterCollection parameters, NetList currentNetList)
        {
            return new MutualInductance(name);
        }

        private Entity GenerateCap(string name, ParameterCollection parameters, NetList currentNetList)
        {
            var capacitor = new Capacitor(name);
            CreateNodes(parameters, capacitor);

            if (parameters.Values.Count == 3)
            {
                var capacitance = (parameters.Values[2] as SingleParameter).RawValue;
                capacitor.ParameterSets.SetProperty("capacitance", currentNetList.ParseDouble(capacitance));

                return capacitor;
            }
            else
            {
                //TODO !!!!!
                throw new System.Exception();
            }
        }

        private Entity GenerateInd(string name, ParameterCollection parameters, NetList currentNetList)
        {
            if (parameters.Values.Count != 3)
            {
                throw new System.Exception();
            }

            var inductor = new Inductor(name);
            CreateNodes(parameters, inductor);
           
            var inductance = (parameters.Values[2] as SingleParameter).RawValue;
            inductor.ParameterSets.SetProperty("inductance", currentNetList.ParseDouble(inductance));
            return inductor;
        }

        private Entity GenerateRes(string name, ParameterCollection parameters, NetList currentNetList)
        {
            if (parameters.Values.Count == 3)
            {
                var res = new Resistor(name);
                CreateNodes(parameters, res);

                var value = (parameters.Values[2] as SingleParameter).RawValue;
                res.ParameterSets.SetProperty("resistance", currentNetList.ParseDouble(value));
                return res;
            }
            else
            {
                //TODO !!!!!!
                throw new System.Exception();
            }
        }

        public override List<string> GetGeneratedTypes()
        {
            return new List<string> { "r", "l", "c", "k" };
        }
    }
}