
using UnityEngine;
using UnityEngine.UIElements;
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphSamples.Editor
{
    [CustomNodeView(typeof(SetVariableNode))]
    class SetVariableNodeView : ExecutableNodeView
    {
        private string variableName;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            styleSheets.Add(Resources.Load<StyleSheet>("BlueGraphSamples/SetVariableNodeView"));

            variableName = (Target as SetVariableNode).variableName;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            // Dirty the node for a redraw - in case a port was renamed
            // and we need to redraw the new port name
            Canvas.Dirty(this);
        }
    }
}
