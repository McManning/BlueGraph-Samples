using UnityEngine;
using BlueGraph;

namespace BlueGraphSamples
{
    [IncludeTags("AI", "Decision")]
    public class DecisionTree : Graph
    {
        public GameObject Agent { get; private set; }

        public GameObject Target { get; private set; }

        /// <summary>
        /// Execute the decision tree to determine if the target
        /// can be attacked by the given agent
        /// </summary>
        public bool CanAttackTarget(GameObject agent, GameObject target)
        {
            // Set references on the Graph to make 
            // them globally accessible to all nodes
            Agent = agent;
            Target = target;

            // Get a node matching the decision we're looking for 
            var decisionNode = GetNode<CanAttackTarget>();
            if (decisionNode == null)
            {
                Debug.LogWarning($"Missing \"Can Attack Target\" decision");
                return false;
            }

            // Determine success by reading the input boolean to the node.
            // This will walk down the tree to all other connected nodes
            // to come to a true/false decision.
            return decisionNode.GetInputValue("Boolean", false);
        }
    }
}
