
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using BlueGraph;
using BlueGraph.Editor;
using System;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using System.Linq;

namespace BlueGraphSamples
{
    /// <summary>
    /// Custom element that renders a navigation button for each loaded subgraph.
    /// 
    /// Clicking a button will remove subgraphs lower in the stack and bring
    /// editor focus to the selected subgraph or root graph.
    /// </summary>
    public class SubgraphNavElement : VisualElement
    {
        public Stack<CanvasView> CanvasStack { get; } = new Stack<CanvasView>();
        
        private CanvasView activeCanvas;
        private CanvasView rootCanvas;

        public SubgraphNavElement(CanvasView rootCanvas) : base()
        {
            name = "subgraphs-nav";
            Add(new IMGUIContainer(OnIMGUI));

            this.rootCanvas = rootCanvas;
            this.activeCanvas = rootCanvas;
        }
        
        private void OnIMGUI()
        {
            if (CanvasStack.Count < 1)
            {
                return;
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Root", EditorStyles.toolbarButton))
            {
                FocusRoot();
            }
            
            // Icon from https://unitylist.com/p/5c3/Unity-editor-icons
            var icon = EditorGUIUtility.FindTexture("tab_next");

            // Render buttons for the rest of the stack
            foreach (var canvas in CanvasStack.Reverse())
            {
                var content = new GUIContent(canvas.name, icon);
                
                if (canvas == activeCanvas)
                {
                    GUILayout.Label(content, EditorStyles.toolbarButton);
                } 
                else // Render as a button to go back to that subgraph
                {
                    if (GUILayout.Button(content, EditorStyles.toolbarButton))
                    {
                        FocusSubgraph(canvas);
                    }
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public CanvasView GetExistingCanvas(Graph graph)
        {
            if (graph == rootCanvas.Graph)
            {
                return rootCanvas;
            }

            foreach (var canvas in CanvasStack)
            {
                if (canvas.Graph == graph)
                {
                    return canvas;
                }
            }

            return null;
        }

        /// <summary>
        /// Focus on the given CanvasView, destroying anything higher in the stack
        /// </summary>
        public void FocusSubgraph(CanvasView canvas)
        {
            // Remove items from the stack until we reach the canvas
            if (CanvasStack.Contains(canvas))
            {
                while (CanvasStack.Peek() != canvas)
                {
                    CanvasStack.Pop();
                }
            } 
            else
            {
                CanvasStack.Push(canvas);
            }

            SetActiveCanvas(canvas);
        }

        public void FocusRoot()
        {
            CanvasStack.Clear();
            SetActiveCanvas(rootCanvas);
        }

        private void SetActiveCanvas(CanvasView canvas)
        {
            var root = canvas.EditorWindow.rootVisualElement;
            
            root.Remove(activeCanvas);
            activeCanvas = canvas;
            root.Add(canvas);
        }
    }

    /// <summary>
    /// Extensions to the GraphEditorWindow to support toggling the view 
    /// between the root graph and its child subgraphs.
    /// </summary>
    public static class SubgraphEditorWindowExtensions
    {
        private static SubgraphNavElement GetOrCreateSubgraphsNav(this GraphEditorWindow self)
        {
            var nav = self.rootVisualElement.Q<SubgraphNavElement>("subgraphs-nav");
            if (nav == null) 
            {
                nav = new SubgraphNavElement(self.Canvas);
                self.rootVisualElement.Insert(0, nav);
            }
            
            return nav;
        }

        /// <summary>
        /// Close out any subgraphs and switch back to the root 
        /// </summary>
        public static void ShowRootGraph(this GraphEditorWindow self)
        {
            var nav = self.GetOrCreateSubgraphsNav();
            nav.FocusRoot();
        }

        /// <summary>
        /// Replace the currently displayed canvas with one for the given 
        /// subgraph and add the new canvas to the navigation stack. 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="graph"></param>
        public static void ShowSubgraph(this GraphEditorWindow self, Graph graph)
        {
            var nav = self.GetOrCreateSubgraphsNav();

            // Get or instantiate a CanvasView for the graph
            var canvas = nav.GetExistingCanvas(graph);
            if (canvas == null)
            {
                canvas = new CanvasView(self);
                canvas.Load(graph);
                canvas.name = graph.name;
            }

            nav.FocusSubgraph(canvas);
        }
    }
}
