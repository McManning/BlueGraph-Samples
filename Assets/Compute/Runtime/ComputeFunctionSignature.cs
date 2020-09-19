
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueGraphSamples
{ 
    /// <summary>
    /// Metadata for a compute function
    /// </summary>
    [Serializable]
    public class ComputeFunctionSignature
    {
        public string name;
        public Type retval;
        public Dictionary<string, Type> args = new Dictionary<string, Type>();

        public bool HasOutputOfType(Type type)
        {
            // TODO: test retval
            return true;
        }

        public bool HasInputOfType(Type type)
        {
            // TODO: Test arguments
            return true;
        }
    }
}
