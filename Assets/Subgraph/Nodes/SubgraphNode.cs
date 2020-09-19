using System;
using UnityEngine;
using BlueGraph;
using UnityEditor;

namespace BlueGraphSamples
{
    /// <summary>
    /// Concrete instance so that CustomNodeView can work for inherited classes
    /// </summary>
    public abstract class BaseSubgraphNode : Node
    { 
        public abstract Graph GetSubgraph();
    }

    /// <summary>
    /// Abstract class for any node that contains a subgraph. 
    /// <br/><br/>
    /// Inherit from this to make use of SubgraphNodeView
    /// </summary>
    public abstract class SubgraphNode<T> : BaseSubgraphNode where T : Graph
    {
        public T Subgraph 
        { 
            get { return subgraph; }
        }

        [SerializeField] private T subgraph;

        /// <summary>
        /// Remove the subgraph asset on removal of this node.
        /// </summary>
        public override void OnRemovedFromGraph()
        {
            if (subgraph)
            {
                Debug.Log("removing subgraph asset");

                #if UNITY_EDITOR 
                AssetDatabase.RemoveObjectFromAsset(Subgraph);
                Debug.Log("removed from asset db");
                #endif

                subgraph = null;
            }
        }

        /// <summary>
        /// Add a new subgraph asset to the root graph asset when adding this node
        /// </summary>
        public override void OnAddedToGraph()
        {
            // TODO: Eventually handle duplication of the subgraph.
            // This gets messy since it also involves trying to duplicate
            // subgraphs of subgraphs all the way down.
            // For now - it'll just always create a blank and warn us.

            Debug.Log("Adding subgraph asset");

            if (subgraph != null)
            {
                Debug.LogWarning(
                    "Duplicating a subgraph node will not duplicate the subgraph. " +
                    "The new node will have a default graph"
                );
            }

            subgraph = ScriptableObject.CreateInstance<T>();
            subgraph.name = ID; // TODO: renamable

            // This is due to Unity issue #1189089
            // See: https://issuetracker.unity3d.com/issues/parent-and-child-nested-scriptable-object-assets-switch-places-when-parent-scriptable-object-asset-is-renamed
            subgraph.hideFlags = HideFlags.HideInHierarchy;
                
            #if UNITY_EDITOR 
            // We need to add the graph as a sub-asset, otherwise 
            // it won't be persisted outside of the editor. 
            AssetDatabase.AddObjectToAsset(
                subgraph, 
                AssetDatabase.GetAssetPath(Graph.GetInstanceID())
            );
            
            Debug.Log("Added to asset db");
            #endif
        }
        
        public override Graph GetSubgraph()
        {
            // TODO: I want to just use the property but the view
            // that uses BaseSubGraph needs a stricter getter than
            // what anything actually USING the subgraphs would need.
            return subgraph;
        }
    }
}
