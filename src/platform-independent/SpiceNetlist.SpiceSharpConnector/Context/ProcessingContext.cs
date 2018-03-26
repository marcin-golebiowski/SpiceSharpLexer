﻿using System;
using System.Collections.Generic;
using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceSharpConnector.Evaluation;
using SpiceSharp;
using SpiceSharp.Circuits;

namespace SpiceNetlist.SpiceSharpConnector.Context
{
    /// <summary>
    /// Processing context
    /// </summary>
    public class ProcessingContext : IProcessingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingContext"/> class.
        /// </summary>
        /// <param name="contextName">Name of the context</param>
        /// <param name="evaluator">Evaluator for the context</param>
        /// <param name="resultService">Result service for the context</param>
        /// <param name="nodeNameGenerator">Node name generator for the context</param>
        /// <param name="objectNameGenerator">Object name generator for the context</param>
        /// <param name="parent">Parent of th econtext</param>
        public ProcessingContext(
            string contextName,
            IEvaluator evaluator,
            IResultService resultService,
            INodeNameGenerator nodeNameGenerator,
            IObjectNameGenerator objectNameGenerator,
            IProcessingContext parent = null)
        {
            ContextName = contextName ?? throw new ArgumentNullException(nameof(contextName));
            Evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
            Result = resultService ?? throw new ArgumentNullException(nameof(resultService));
            NodeNameGenerator = nodeNameGenerator ?? throw new ArgumentNullException(nameof(nodeNameGenerator));
            ObjectNameGenerator = objectNameGenerator ?? throw new ArgumentNullException(nameof(objectNameGenerator));
            Parent = parent;

            if (Parent != null)
            {
                AvailableSubcircuits = new List<SubCircuit>(Parent.AvailableSubcircuits);
            }
            else
            {
                AvailableSubcircuits = new List<SubCircuit>();
            }

            Children = new List<IProcessingContext>();
        }

        /// <summary>
        /// Gets or sets the name of context
        /// </summary>
        public string ContextName { get; protected set; }

        /// <summary>
        /// Gets or sets the parent of context
        /// </summary>
        public IProcessingContext Parent { get; protected set; }

        /// <summary>
        /// Gets available subcircuits in context
        /// </summary>
        public ICollection<SubCircuit> AvailableSubcircuits { get; }

        /// <summary>
        /// Gets or sets the evaluator
        /// </summary>
        public IEvaluator Evaluator { get; protected set; }

        /// <summary>
        /// Gets or sets the result service
        /// </summary>
        public IResultService Result { get; protected set; }

        /// <summary>
        /// Gets or sets the node name generator
        /// </summary>
        public INodeNameGenerator NodeNameGenerator { get; protected set; }

        /// <summary>
        /// Gets or sets the object name generator
        /// </summary>
        public IObjectNameGenerator ObjectNameGenerator { get; protected set; }

        /// <summary>
        /// Gets or sets the children of the processing context
        /// </summary>
        public ICollection<IProcessingContext> Children { get; protected set; }

        /// <summary>
        /// Sets voltage initial condition for node
        /// </summary>
        /// <param name="nodeName">Name of node</param>
        /// <param name="expression">Expression</param>
        public void SetICVoltage(string nodeName, string expression)
        {
            //TODO: Add dynamic 
            Result.SetInitialVoltageCondition(NodeNameGenerator.Generate(nodeName), Evaluator.EvaluateDouble(expression));
        }

        /// <summary>
        /// Parses an expression to double
        /// </summary>
        /// <param name="expression">Expression to parse</param>
        /// <returns>
        /// A value of expression
        /// </returns>
        public double ParseDouble(string expression)
        {
            try
            {
                return Evaluator.EvaluateDouble(expression);
            }
            catch (Exception)
            {
                throw new Exception("Exception during evaluation of expression: " + expression);
            }
        }

        /// <summary>
        /// Sets the parameter of entity and enables updates
        /// </summary>
        /// <param name="entity">An entity of parameter</param>
        /// <param name="parameterName">A parameter name</param>
        /// <param name="expression">An expression</param>
        /// <returns>
        /// True if the parameter has been set
        /// </returns>
        public bool SetParameter(Entity entity, string parameterName, string expression)
        {
            double value;
            try
            {
                value = Evaluator.EvaluateDouble(expression);
            }
            catch (Exception ex)
            {
                Result.AddWarning("Exception during parsing expression '" + expression + "': " + ex);
                return false;
            }

            var set = entity.SetParameter(parameterName.ToLower(), value);

            if (set)
            {
                var variables = Evaluator.GetVariables(expression);
                var setter = entity.ParameterSets.GetSetter(parameterName.ToLower());

                // re-evaluation makes sense only if there is a setter
                if (setter != null)
                {
                    Evaluator.AddDynamicExpression(new DoubleExpression(expression, setter), variables);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the parameter of entity
        /// </summary>
        /// <param name="entity">An entity of parameter</param>
        /// <param name="parameterName">A parameter name</param>
        /// <param name="object">An parameter value</param>
        /// <returns>
        /// True if the parameter has been set
        /// </returns>
        public bool SetParameter(Entity entity, string parameterName, object @object)
        {
            return entity.SetParameter(parameterName.ToLower(), @object);
        }

        /// <summary>
        /// Finds model in the context and in parent contexts
        /// </summary>
        /// <param name="modelName">Name of model to find</param>
        /// <returns>
        /// A reference to model
        /// </returns>
        public T FindModel<T>(string modelName)
            where T : Entity
        {
            IProcessingContext context = this;
            while (context != null)
            {
                var modelNameToSearch = context.ObjectNameGenerator.Generate(modelName);

                Entity model;
                if (Result.FindObject(modelNameToSearch, out model))
                {
                    return (T)model;
                }

                context = context.Parent;
            }

            return null;
        }

        /// <summary>
        /// Creates nodes for a component
        /// </summary>
        /// <param name="component">A component</param>
        /// <param name="parameters">Parameters of component</param>
        public void CreateNodes(SpiceSharp.Components.Component component, ParameterCollection parameters)
        {
            Identifier[] nodes = new Identifier[component.PinCount];
            for (var i = 0; i < component.PinCount; i++)
            {
                string pinName = parameters.GetString(i);
                nodes[i] = NodeNameGenerator.Generate(pinName);
            }

            component.Connect(nodes);
        }
    }
}
