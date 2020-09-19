using System;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace BlueGraphSamples
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "BlueGraph Samples/Compute Graph", fileName = "New ComputeGraph")]
    [IncludeTags("Compute")]
    public class ComputeGraph : Graph
    {
        /*void OnEnable()
        {
            // Add an initial node to represent the result of this compute graph
            var node = GetNode<ComputeResultNode>();
            if (node == null)
            {
                AddNode(new ComputeResultNode());
            }
        }*/
    }
}
