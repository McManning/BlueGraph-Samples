using System;
using System.Collections.Generic;
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphSamples
{
    /// <summary>
    /// Custom search provider to add getters and setters for declared variables.
    /// 
    /// Each variable will add "Get (variable name)" and "Set (variable name)" 
    /// nodes to the search results, which instantiate the appropriate GetVariable 
    /// and SetVariable nodes. 
    /// </summary>
    public class VariablesSearchProvider : ISearchProvider
    {
        public bool IsSupported(IGraph graph)
        {
            return typeof(IContainsVariables).IsAssignableFrom(graph.GetType());
        }

        /// <summary>
        /// Returns a getter (and for non-constants, a setter) for every
        /// variable on the IContainsVariable graph. 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<SearchResult> GetSearchResults(SearchFilter filter)
        {
            // Variables could also be an extension on the graph, not an interface.
            var graph = (filter.Graph as IContainsVariables);
            if (graph == null) 
            { 
                throw new Exception("VariablesSearchProvider can only be used with an IContainsVariables graph");
            }

            foreach (var variable in graph.Variables)
            {
                if (IsCompatibleWithGetter(filter.SourcePort, variable.Type))
                {
                    yield return new SearchResult
                    {
                        Name = $"Get {variable.Name}",
                        UserData = variable
                    };
                }

                if (!variable.IsConstant && IsCompatibleWithSetter(filter.SourcePort, variable.Type))
                {
                    yield return new SearchResult
                    {
                        Name = $"Set {variable.Name}",
                        UserData = variable
                    };
                }
            }
        }
        
        /// <summary>
        /// Ensure that a source port is compatible with a getter node of the
        /// given type prior to adding to the search results.
        /// </summary>
        private bool IsCompatibleWithGetter(Port sourcePort, Type type)
        {
            if (sourcePort == null)
            {
                return true;
            }
            
            if (sourcePort.Direction == PortDirection.Input)
            {
                return type.IsCastableTo(sourcePort.Type, true);
            }

            // Cannot output to a getter
            return false;
        }
        
        /// <summary>
        /// Ensure that a source port is compatible with a setter node of the
        /// given type prior to adding to the search results.
        /// </summary>
        private bool IsCompatibleWithSetter(Port sourcePort, Type type)
        {
            if (sourcePort == null)
            {
                return true;
            }
            
            var setterReflection = NodeReflection.GetNodeType(typeof(SetVariableNode));

            if (sourcePort.Direction == PortDirection.Input)
            {
                // Handle exec flow outputs
                return setterReflection.HasOutputOfType(sourcePort.Type);
            }

            // Handle both Exec flow data and the actual data type
            return setterReflection.HasInputOfType(sourcePort.Type)
                    || sourcePort.Type.IsCastableTo(type, true);
        }

        public Node Instantiate(SearchResult result)
        {
            if (result.Name.IndexOf("Set") == 0)
            {
                return InstantiateSetter(result.UserData as GraphVariable);
            }

            return InstantiateGetter(result.UserData as GraphVariable);
        }

        /// <summary>
        /// Instantiate a getter node with an output matching the variable type
        /// </summary>
        private Node InstantiateGetter(GraphVariable variable)
        {
            var node = NodeReflection.Instantiate<GetVariableNode>();
            node.Name = variable.Name;

            node.AddPort(new Port
            {
                Type = variable.Type,
                Name = "Value",
                Direction = PortDirection.Output,
                Capacity = PortCapacity.Multiple
            });

            return node;
        }

        /// <summary>
        /// Instantiate a setter node with an input matching the variable type
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private Node InstantiateSetter(GraphVariable variable)
        {
            var node = NodeReflection.Instantiate<SetVariableNode>();
            node.variableName = variable.Name;

            node.AddPort(new Port
            {
                Type = variable.Type,
                Name = variable.Name,
                Direction = PortDirection.Input,
                Capacity = PortCapacity.Single
            });

            return node;
        }
    }
}
