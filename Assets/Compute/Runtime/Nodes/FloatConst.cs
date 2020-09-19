
using System;
using BlueGraph;

namespace BlueGraphSamples
{ 
    /// <summary>
    /// Constant value node
    /// </summary>
    [Node("float", Path = "Compute/Const")]
    public class FloatConst : Node
    {
        [Input, Output("")] public float value;

        /// <summary>
        /// Output from the node is HLSL code composed from 
        /// this node's function call and other nodes.  
        /// </summary>
        public override object OnRequestValue(Port port)
        {
            /*
                Should be something like:

                float f = 123.0;

                and return f for reference 
            */
            throw new NotImplementedException();
        }
    }
}
