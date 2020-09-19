
using System;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphSamples
{ 
    /// <summary>
    /// Search provider for compute shader functions.
    ///
    /// Reads one or more HLSL files to extract functions that can be 
    /// converted into ComputeFunction nodes. 
    /// </summary>
    public class ComputeSearchProvider : ISearchProvider
    {
        static List<ComputeFunctionSignature> k_Signatures;
        
        public bool IsSupported(IGraph graph)
        {
            return false;
        }

        /// <summary>
        /// Generate results from scanned HLSL function signatures
        /// </summary>
        public IEnumerable<SearchResult> GetSearchResults(SearchFilter filter)
        {
            if (k_Signatures == null)
            {
                LoadSignatures();
            }

            foreach (var signature in k_Signatures)
            {
                if (IsCompatible(filter.SourcePort, signature))
                {
                    yield return new SearchResult
                    {
                        Name = signature.name,
                        Path = new string[] { "Compute" },
                        UserData = signature,
                    };
                }
            }
        }
        
        /// <summary>
        /// Create a new ComputeFunctionNode from the function signature
        /// tied to the search result passed in. 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Node Instantiate(SearchResult result)
        {
            var signature = result.UserData as ComputeFunctionSignature;
            var node = new ComputeFunctionNode();
            node.LoadSignature(signature);

            return node;
        }

        /// <summary>
        /// Check if the source port can be connected to the given signature.
        /// 
        /// Based on the port, this will either test for type compatibility
        /// of the function's retval or arguments
        bool IsCompatible(Port sourcePort, ComputeFunctionSignature signature)
        {
            if (sourcePort == null)
            {
                return true;
            }

            if (sourcePort.Direction == PortDirection.Input)
            {
                return signature.HasOutputOfType(sourcePort.Type);
            }

            return signature.HasInputOfType(sourcePort.Type);
        }

        void LoadSignatures()
        {
            k_Signatures = new List<ComputeFunctionSignature>();
            
            // TODO: File parsing and loading
        }
    }
}
