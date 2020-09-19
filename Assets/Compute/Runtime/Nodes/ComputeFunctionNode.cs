
using System;
using BlueGraph;

namespace BlueGraphSamples
{ 
    // Cannot be instantiated via default search - only through ComputeSearchProvider
    // [Node(module = "Hidden")]
    public class ComputeFunctionNode : Node
    {
        public ComputeFunctionSignature signature;

        /// <summary>
        /// Output from the node is HLSL code composed from 
        /// this node's function call and other nodes.  
        /// </summary>
        public override object OnRequestValue(Port port)
        {
            // TODO: Return a function call string composing all the inputs
            // TODO: Underlying function should only be called once then cached:
            // so this should be able to store a variable name the results were
            // cached under when it DOES execute, and then return that variable.

            /*
                E.g. output:

                [previous input node executions]

                RetvalType varX = MyReferencedFunc(varA, varB, varC);

                ... and when called multiple times, it'll just provide varX.
                Or something to that effect. I have a code generator somewhere
                I can reuse for this after simplifying. 
            */
            throw new NotImplementedException();
        }

        /// <summary>
        /// Configure this node with IO ports and metadata 
        /// to match the given function signature
        /// </summary>
        public void LoadSignature(ComputeFunctionSignature signature)
        {
            Name = signature.name;

            // Add an output port for the return value
            AddPort(new Port 
            {
                Name = "Retval",
                Direction = PortDirection.Output,
                Capacity = PortCapacity.Multiple,
                Type = signature.retval,
            });

            // Add input ports for the function arguments
            foreach (var arg in signature.args)
            {
                AddPort(new Port
                {
                    Name = arg.Key,
                    Direction = PortDirection.Input,
                    Capacity = PortCapacity.Single,
                    Type = arg.Value,
                });
            }

            // TODO: How would inline editing constants work here?
            // We don't have anything stored on the class. Just externalize?
        }
    }
}
