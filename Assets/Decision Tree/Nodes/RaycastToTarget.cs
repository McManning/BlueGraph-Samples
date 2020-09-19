using BlueGraph;
using UnityEngine;

namespace BlueGraphSamples
{
    [Node]
    [Tags("AI")]
    public class RaycastToTarget : Node
    {
        [Input("Max Distance (meters)")] public float maxDistance;
        [Input("Ignore Layers")] public float ignoreLayers;

        [Output("Distance")] public float distance;
        [Output("Hit Target")] public bool hitTarget;

        public override object OnRequestValue(Port port)
        {
            // TODO: Execute raycaster and cache results.
            // Not quite sure what a good example of caching looks like here.
            // It should be stored until the next evaluation call from the graph.
            // Maybe an int on the graph to indicate cache state? Or graph to 
            // call all nodes to clear their cache on each run? Or leave it up 
            // to the implementer to figure something better out...

            if (port.Name == "Distance")
            {
                return distance;
            }

            // "Hit Target" output
            return hitTarget;
        }
    }
}
