using BlueGraph;
using UnityEngine;

namespace BlueGraphSamples
{
    /// <summary>
    /// Check if <c>DecisionTree.Target</c> is a different
    /// faction than <c>DecisionTree.Agent</c>
    /// </summary>
    [Node]
    [Tags("AI")]
    public class IsOppositeFaction : Node
    {
        [Output("Boolean")] public bool value;

        public override object OnRequestValue(Port port)
        {
            var dt = Graph as DecisionTree;

            // Yup, totes. TODO: Check...
            return true;
        }
    }
}
