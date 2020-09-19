using System;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;
using System.Collections;

namespace BlueGraphSamples
{
    /// <summary>
    /// Wrapper around a GraphVariable list to support custom drawers
    /// </summary>
    [Serializable]
    public class VariableList : IEnumerable<GraphVariable>
    {
        [SerializeField]
        private List<GraphVariable> entries = new List<GraphVariable>();

        public GraphVariable Find(string name) => entries.Find((v) => v.Name == name);

        public void Add(GraphVariable variable) => entries.Add(variable);

        public void Remove(GraphVariable variable) => entries.Remove(variable);

        public IEnumerator<GraphVariable> GetEnumerator() => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator( )=> entries.GetEnumerator();
    }

    [CreateAssetMenu(
        menuName = "BlueGraph Samples/Experimental/Graph with Variables", 
        fileName = "New Graph with Variables"
    )]
    [IncludeTags("Variables", "Constants", "Math")]
    public class GraphWithVariables : Graph, IContainsVariables
    {
        [SerializeField]
        private VariableList variables = new VariableList();

        public VariableList Variables
        {
            get { return variables; }
        }

        protected override void OnGraphEnable()
        {
            Debug.Log($"graphs onenable foo {this.GetFoo()}");
        }
    }
}

