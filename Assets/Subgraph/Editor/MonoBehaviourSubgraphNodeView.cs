using UnityEngine;
using UnityEngine.UIElements;
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphSamples
{
    /// <summary>
    /// Modification on top of SubgraphNodeView to render
    /// execution flow ports the same way that other ExecutableNodes do.
    /// </summary>
    [CustomNodeView(typeof(MonoBehaviourSubgraphNode))]
    public class MonoBehaviourSubgraphNodeView : SubgraphNodeView
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            styleSheets.Add(Resources.Load<StyleSheet>("BlueGraphSamples/ExecutableNodeView"));
            AddToClassList("executableNodeView");

            // Customize placement of the default exec IO ports 
            PortView inView = GetInputPort("ExecIn");
            if (inView != null)
            {
                inView.AddToClassList("execInPortView");
            }
            
            PortView outView = GetOutputPort("ExecOut");
            if (outView != null) 
            {
                outView.AddToClassList("execOutPortView");
            }
        }
    }
}
