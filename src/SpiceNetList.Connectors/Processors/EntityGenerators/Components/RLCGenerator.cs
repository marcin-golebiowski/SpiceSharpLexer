﻿using System;
using System.Collections.Generic;
using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceObjects.Parameters;
using SpiceSharp;
using SpiceSharp.Circuits;
using SpiceSharp.Components;

namespace SpiceNetlist.SpiceSharpConnector.Processors.EntityGenerators.Components
{
    public class RLCGenerator : EntityGenerator
    {
        public override Entity Generate(Identifier id, string originalName, string type, ParameterCollection parameters, ProcessingContext context)
        {
            switch (type)
            {
                case "r": return GenerateRes(id.Name, parameters, context);
                case "l": return GenerateInd(id.Name, parameters, context);
                case "c": return GenerateCap(id.Name, parameters, context);
                case "k": return GenerateMut(id.Name, parameters, context);
            }

            return null;
        }

        public Entity GenerateMut(string name, ParameterCollection parameters, ProcessingContext context)
        {
            var mut = new MutualInductance(name);

            switch (parameters.Count)
            {
                case 0: throw new Exception($"Inductor name expected for mutual inductance \"{name}\"");
                case 1: throw new Exception("Inductor name expected");
                case 2: throw new Exception("Coupling factor expected");
            }

            if (!(parameters[0] is SingleParameter)) 
            {
                throw new Exception("Component name expected");
            }

            if (!(parameters[1] is SingleParameter))
            {
                throw new Exception("Component name expected");
            }

            mut.InductorName1 = (parameters[0] as SingleParameter).RawValue;
            mut.InductorName2 = (parameters[1] as SingleParameter).RawValue;
            mut.ParameterSets.SetProperty("k", context.ParseDouble((parameters[2] as SingleParameter).RawValue));

            return mut;
        }

        public Entity GenerateCap(string name, ParameterCollection parameters, ProcessingContext context)
        {
            var capacitor = new Capacitor(name);
            context.CreateNodes(parameters, capacitor);

            if (parameters.Count == 3)
            {
                var capacitance = (parameters[2] as SingleParameter).RawValue;
                capacitor.ParameterSets.SetProperty("capacitance", context.ParseDouble(capacitance));

                return capacitor;
            }
            else
            {
                //TODO !!!!!
                throw new System.Exception();
            }
        }

        public Entity GenerateInd(string name, ParameterCollection parameters, ProcessingContext context)
        {
            if (parameters.Count != 3)
            {
                throw new Exception();
            }

            var inductor = new Inductor(name);
            context.CreateNodes(parameters, inductor);

            var inductance = (parameters[2] as SingleParameter).RawValue;
            inductor.ParameterSets.SetProperty("inductance", context.ParseDouble(inductance));
            return inductor;
        }

        public Entity GenerateRes(string name, ParameterCollection parameters, ProcessingContext context)
        {
            var res = new Resistor(name);
            context.CreateNodes(parameters, res);

            if (parameters.Count == 3)
            {
                var value = (parameters[2] as SingleParameter).RawValue;
                res.ParameterSets.SetProperty("resistance", context.ParseDouble(value));
            }
            else
            {
                var modelName = (parameters[2] as SingleParameter).RawValue;
                res.SetModel(context.FindModel<ResistorModel>(modelName));

                foreach (var equal in parameters.Skip(2))
                {
                    if (equal is AssignmentParameter ap)
                    {
                        res.ParameterSets.SetProperty(ap.Name, context.ParseDouble(ap.Value));
                    }
                }

                SpiceSharp.Components.ResistorBehaviors.BaseParameters bp = res.ParameterSets[typeof(SpiceSharp.Components.ResistorBehaviors.BaseParameters)] as SpiceSharp.Components.ResistorBehaviors.BaseParameters;
                if (!bp.Length.Given)
                {
                    throw new System.Exception("L needs to be specified");
                }
            }

            return res;
        }

        public override List<string> GetGeneratedTypes()
        {
            return new List<string> { "r", "l", "c", "k" };
        }
    }
}
