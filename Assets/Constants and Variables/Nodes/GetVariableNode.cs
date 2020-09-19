using System;
using UnityEngine;
using BlueGraph;

namespace BlueGraphSamples
{
    /// <summary>
    /// Get a variable from the graph.
    /// <br/><br/>
    /// These are not available through the default search provider
    /// and are instead dynamically constructed via the 
    /// <c>ConstantsAndVariablesSearchProvider</c>
    /// </summary>
    [Node]
    [Tags("Hidden")]
    public class GetVariableNode : Node
    {
        public GraphVariable Variable { get; private set; }
        
        // An output port is automatically added via ConstantsAndVariablesSearchProvider.
        // The .Name of this node matches the variable name. 

        public override object OnRequestValue(Port port)
        {   
            // We just pass a boxed value because it's going to 
            // box anyway on the retval.
            return Variable.BoxedValue;
        }

        /// <summary>
        /// Sync this node with the backing variable.
        /// </summary>
        /// <returns></returns>
        private void LoadVariable()
        {
            Error = null;

            // Default to the cached variable instance if we have one, 
            // otherwise use the node's name. This lets us safely handle
            // renamed variables.
            var variableName = (Variable != null) ? Variable.Name : Name;

            var graph = (Graph as IContainsVariables);
            if (graph == null)
            {
                throw new Exception(
                    "Can only use a Get Variable node on an IContainsVariables graph"
                );
            }

            var variable = graph.FindVariable(variableName);
            if (variable == null)
            {
                Error = $"References unknown variable";
                Variable = null;
            }
            else
            {
                Variable = variable;
                Name = variable.Name;
            }
            
            // Could auto disconnect - but I'd rather leave this 
            // up to the designer to correct manually in the canvas.
            // Otherwise it can be confusing what the prior connection state was,
            // or we can recover automatically by fixing the variable type.
            var port = GetPort("Value");
            if (variable.Type != port.Type)
            {
                Error = $"Port does not match the variable type";
            }
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
    }
}
