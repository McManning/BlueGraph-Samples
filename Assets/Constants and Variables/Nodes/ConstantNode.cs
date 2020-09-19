using System;
using UnityEngine;
using BlueGraph;

namespace BlueGraphSamples
{
    /// <summary>
    /// Non-generic base class so that the ConstantNodeView can apply to 
    /// all inherited concrete nodes simultaneously. 
    /// </summary>
    public abstract class ConstantNode : Node
    {
        public virtual Type OutputType { get; }
    }

    /// <summary>
    /// Base class for constants that are value types (floats, structs, etc)
    /// or types that can be nullable and not instantiated by default.
    /// </summary>
    public abstract class ConstantValueTypeNode<T> : ConstantNode
    {
        public override Type OutputType { get { return typeof(T); } }

        [Editable, Output] public T value;

        public override object OnRequestValue(Port port) => value;
    }

    /// <summary>
    /// Base class for constants requiring a parameterless constructor 
    /// </summary>
    public abstract class ConstantNewableTypeNode<T> : ConstantNode where T : new()
    {
        public override Type OutputType { get { return typeof(T); } }

        [Editable, Output] public T value = new T();

        public override object OnRequestValue(Port port) => value;
    }
    
    // Concretes for all the constants!

    [Node("String Constant")]
    [Tags("Constants")]
    public class StringConstant : ConstantValueTypeNode<string> { }
    
    [Node("Int Constant")]
    [Tags("Constants")]
    public class ConstantInt : ConstantValueTypeNode<int> { }

    [Node("Float Constant")]
    [Tags("Constants")]
    public class ConstantFloat : ConstantValueTypeNode<float> { }

    [Node("Vector3 Constant")]
    [Tags("Constants")]
    public class Vector3Constant : ConstantValueTypeNode<Vector3> { }
    
    [Node("Vector4 Constant")]
    [Tags("Constants")]
    public class Vector4Constant : ConstantValueTypeNode<Vector4> { }

    [Node("Curve Constant")]
    [Tags("Constants")]
    public class CurveConstant : ConstantValueTypeNode<AnimationCurve> { }

    // TODO: This one isn't safe. If they reference something in the scene
    // it'll fail with a serialized value of { fileId: 0 }
    [Node("GameObject Constant")]
    [Tags("Constants")]
    public class GameObjectConstant : ConstantValueTypeNode<GameObject> { }

    // This one works. Because we know SO's are an asset
    [Node("Asset Constant")]
    [Tags("Constants")]
    public class AssetConstant : ConstantValueTypeNode<ScriptableObject> { }
}
