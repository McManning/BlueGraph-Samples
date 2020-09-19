using BlueGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueGraphSamples
{
    [Node]
    [Tags("AI", "Decision")]
    public class CanAttackTarget : Node
    {
        [Input("Boolean")] public bool value;

        // This node contains no outputs.
        public override object OnRequestValue(Port port) => null;
    }
}
