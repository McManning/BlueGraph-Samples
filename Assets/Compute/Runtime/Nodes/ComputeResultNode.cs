
using System;
using BlueGraph;

namespace BlueGraphSamples
{ 
    /// <summary>
    /// Node that represents the final output of the compute function. 
    /// 
    /// For this demo, we just have a singular output calculation.
    /// </summary>
    public class ComputeResultNode : Node
    {
        public ComputeResultNode() : base()
        {
            AddPort(new Port
            {
                Name = "Result",
                Direction = PortDirection.Output,
                Type = typeof(float),
            });
        }

        /// <summary>
        /// Output from the node is HLSL code composed from 
        /// this node's function call and other nodes.  
        /// </summary>
        public override object OnRequestValue(Port port)
        {
            // TODO: ?
            throw new NotImplementedException();
        }
    }
}
