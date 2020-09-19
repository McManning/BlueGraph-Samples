using System.Linq;
using UnityEngine;
using BlueGraph;

#if UNITY_EDITOR
using BlueGraph.Editor;
#endif 

namespace BlueGraphSamples
{
    /// <summary>
    /// Subgraph that can be stored within a <c>MonoBehaviourGraph</c>.
    /// 
    /// Only a subset of nodes are allowed from the root graph (e.g. no event nodes)
    /// </summary>
    [IncludeTags("Math", "Executable", "Flow Control", "MonoBehaviour Subgraph")]
    public class MonoBehaviourSubgraph : Graph, IExecutes
    {
#if UNITY_EDITOR
        /// <summary>
        /// On asset creation in the editor, initialize with some default event nodes.
        /// </summary>
        protected override void OnGraphEnable()
        {
            if (Nodes.Count > 0) return;

            // TODO: Not an OnEnable - should be a function name or 
            // something (or named after the subgraph)
            if (GetNode<OnEnable>() == null)
            {
                var node = NodeReflection.Instantiate<OnEnable>();
                AddNode(node);
            }
        }
#endif

        public void Execute(IExecutableNode root, ExecutionFlowData data)
        {
            // Execute through the graph until we run out of nodes to execute.
            // Each node will return the next node to be executed in the path. 
            IExecutableNode next = root;
            int iterations = 0;
            while (next != null)
            {
                next = next.Execute(data);

                iterations++;
                if (iterations > 2000)
                {
                    Debug.LogError("Potential infinite loop detected. Stopping early.", this);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// The node that contains and executes a MonoBehaviourSubgraph
    /// </summary>
    [Node("New Subgraph")]
    [Tags("MonoBehaviour Subgraph")]
    [Output("ExecOut", typeof(ExecutionFlowData), Multiple = false)]
    public class MonoBehaviourSubgraphNode : 
        SubgraphNode<MonoBehaviourSubgraph>, 
        IExecutableNode
    {
        [Input("ExecIn", Multiple = true)] public ExecutionFlowData execIn;
        
        public override object OnRequestValue(Port port) => null;

        public IExecutableNode Execute(ExecutionFlowData data)
        {
            if (Subgraph == null)
            {
                Error = "No subgraph found";
                return GetNextExecutableNode();
            }

            var entry = Subgraph.GetNode<OnEnable>();
            if (entry == null)
            {
                Error = "No entry point node found";
                return GetNextExecutableNode();
            }

            Subgraph.Execute(entry, data);
            
            // TODO: Eventually read some outputs.

            return GetNextExecutableNode();
        }
        
        /// <summary>
        /// Get the next node that should be executed along the edge
        /// </summary>
        /// <returns></returns>
        public IExecutableNode GetNextExecutableNode(string portName = "ExecOut")
        {
            var port = GetPort(portName);
            if (port.ConnectionCount < 1) 
            {
                return null;
            }
            
            var node = port.ConnectedPorts.First()?.Node;
            if (node is IExecutableNode execNode)
            {
                return execNode;
            }

            Debug.LogWarning(
                $"<b>[{Name}]</b> Connected output node {node.Name} to port {port.Name} is not an ICanExec. " +
                $"Cannot execute past this point."
            );

            return null;
        }
    }
}
