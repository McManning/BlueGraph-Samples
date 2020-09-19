using System;
using BlueGraph;
using UnityEngine;

namespace BlueGraphSamples
{
    /// <summary>
    /// Set a named variable.
    /// <br/><br/>
    /// Requires Execution Flow to operate since we need to 
    /// set it at a particular time of execution.
    /// </summary>
    [Node("Set")]
    [Tags("Hidden")]
    public class SetVariableNode : ExecutableNode
    {
        public GraphVariable Variable { get; private set; }

        // Output is added through ConstantsAndVariablesSearchProvider
        // with a port named the same as variableName.

        public string variableName;

        public override IExecutableNode Execute(ExecutionFlowData data)
        {
            if (GetPort(variableName).ConnectionCount < 1)
            { 
                throw new Exception(
                    $"Setter for {variableName} requires an input connection"
                );
            }

            Variable.BoxedValue = GetInputValue<object>(variableName);

            return base.Execute(data);
        }

        /// <summary>
        /// Load and cache the GraphVariable reference on enable
        /// </summary>
        public override void OnEnable()
        {
            LoadVariable();
        }

        public override void OnValidate()
        {
            LoadVariable();
        }

        private void LoadVariable()
        {
            Error = null;

            // Default to the cached variable instance if we have one, 
            // otherwise use the node's name. This lets us safely handle
            // renamed variables.
            var name = (Variable != null) ? Variable.Name : variableName;

            var graph = (Graph as IContainsVariables);
            if (graph == null)
            {
                throw new Exception(
                    "Can only use a Set Variable node on an IContainsVariables graph"
                );
            }
            
            var variable = graph.FindVariable(name);
            if (variable == null)
            {
                Error = $"References unknown variable";
                Variable = null;
            }
            else
            {
                // Sync our input port name with the variable name
                Variable = variable;
                var port = GetPort(variableName);
                if (port == null)
                {
                    Error = $"Could not find port for {variableName}";
                    return;
                }

                port.Name = Variable.Name;
                variableName = Variable.Name;

                if (variable.IsConstant)
                {
                    Error = $"Cannot set a constant";
                }

                // Could auto disconnect - but I'd rather leave this 
                // up to the designer to correct manually in the canvas.
                // Otherwise it can be confusing what the prior connection state was,
                // or we can recover automatically by fixing the variable type.
                if (variable.Type != port.Type)
                {
                    Error = $"Port does not match the variable type";
                }
            }
        }
    }
}
