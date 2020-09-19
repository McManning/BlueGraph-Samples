
using UnityEngine;
using UnityEngine.UIElements;
using BlueGraph;
using BlueGraph.Editor;
using System;

namespace BlueGraphSamples.Editor
{
    [CustomNodeView(typeof(GetVariableNode))]
    class GetVariableNodeView : NodeView
    {
        protected override void OnInitialize()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("BlueGraphSamples/GetVariableNodeView"));
        
            // Add the same type classes as the output port 
            // so we can colorize the node itself in a similar way
            var node = (Target as GetVariableNode);

            var classes = node.Variable.Type.ToUSSClasses();
            foreach (var cls in classes) {
                AddToClassList(cls);
            }

            titleContainer.Add(new VisualElement { name = "type-icon" });
        }
        
        /// <summary>
        /// Sync with potential state changes on the node
        /// </summary>
        protected override void OnValidate()
        {
            title = Target.Name;

            // TODO: If the type differs, we need to recreate the port
            // and break connections. This can happen if someone deletes
            // a variable and creates a new one under the same name but
            // a different type. OR if they change the type in the 
            // GraphVariablesTableView (if/when supported)

            // Should be @ the node's OnValidate not the view.
        }
    }
}