
using UnityEngine;
using UnityEngine.UIElements;
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphSamples.Editor
{
    [CustomNodeView(typeof(ConstantNode))]
    class ConstantNodeView : NodeView
    {
        protected override void OnInitialize()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("BlueGraphSamples/ConstantNodeView"));
        
            // Add the same type classes as the output port 
            // so we can colorize the node itself in a similar way
            var node = (Target as ConstantNode);

            var classes = node.OutputType.ToUSSClasses();
            foreach (var cls in classes) {
                AddToClassList(cls);
            }
        }
    }
}