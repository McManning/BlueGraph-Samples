using UnityEngine.UIElements;
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphSamples
{
    /// <summary>
    /// Node view that adds an "Open" button in the title bar
    /// to create/open an associated subgraph asset 
    /// </summary>
    [CustomNodeView(typeof(BaseSubgraphNode))]
    public class SubgraphNodeView : NodeView
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();

            var btn = new Button(OnOpenSubgraph)
            {
                text = "Open"
            };

            titleContainer.Add(btn);
        }

        void OnOpenSubgraph()
        {
            var node = Target as BaseSubgraphNode;

            var subgraph = node.GetSubgraph();
            if (subgraph == null)
            {
                Target.Error = "Missing Subgraph";
                return;
            }

            Canvas.EditorWindow.ShowSubgraph(subgraph);
        }
    }
}
