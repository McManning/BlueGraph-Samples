using System;
using UnityEngine;

namespace BlueGraphSamples
{
    /// <summary>
    /// Metadata and value of a variable exposed on a Graph
    /// </summary>
    [Serializable]
    public class GraphVariable : ISerializationCallbackReceiver
    {
        public static Type[] SupportedTypes
        {
            get { return supportedTypes; }
        }

        // TODO: Any way to support extensions to this? 
        // Would also need to support custom editors
        // in the inspector for default value...

        private static readonly Type[] supportedTypes = new[]
        {
            typeof(float),
            typeof(int),
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Color),
            typeof(AnimationCurve),
            typeof(Gradient),
            typeof(UnityEngine.Object),
            // And so on.
        };
        
        public string Name
        { 
            get { return name; }
            set { name = value; }
        }
        
        [SerializeField] private string name;

        public Type Type { get; set; }

        [SerializeField] string type;

        public bool IsConstant 
        { 
            get { return isConstant; }
            set { isConstant = value; }
        }

        [SerializeField] private bool isConstant;
        
        // Bit of magic here - each field is named 
        // "(lowercase Type without namespace)Value"
        // so that we can quickly access the value entry by
        // combining the variable type + FindPropertyRelative
        public float singleValue;
        public int int32Value;
        public string stringValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Vector4 vector4Value;
        public Color colorValue;
        public AnimationCurve animationcurveValue;
        public Gradient gradientValue;

        // General container for any object types 
        public UnityEngine.Object objectValue;

        /// <summary>
        /// Temp value storage on the heap to be
        /// passed around between nodes on the graph
        /// </summary>
        public object BoxedValue
        {
            get
            {
                if (boxedValue == null)
                {
                    CopyValueToHeap();
                }

                return boxedValue;
            }
            set
            {
                boxedValue = value;
            }
        }
        
        [NonSerialized] private object boxedValue;

        public void OnBeforeSerialize()
        {
            if (Type != null)
            {
                type = Type.AssemblyQualifiedName;
            }
        }

        public void OnAfterDeserialize()
        {
            Type = Type.GetType(type);
        }

        private void CopyValueToHeap()
        {
            if (Type == typeof(float))
            {
                boxedValue = singleValue;
            }
            else if (Type == typeof(int))
            {
                boxedValue = int32Value;
            }
            else if (Type == typeof(string))
            {
                boxedValue = stringValue;
            }
            else if (Type == typeof(Vector2))
            {
                boxedValue = vector2Value;
            }
            else if (Type == typeof(Vector3))
            {
                boxedValue = vector3Value;
            }
            else if (Type == typeof(Vector4))
            {
                boxedValue = vector4Value;
            }
            else if (Type == typeof(Color))
            {
                boxedValue = colorValue;
            }
            else if (Type == typeof(AnimationCurve))
            {
                boxedValue = animationcurveValue;
                // TODO: To copy or not. That is the question.
                // Because I don't want to be able to modify these types
                // and accidentally persist that (or share it) between
                // different instances or runs of the graph. 
                // boxedValue = new AnimationCurve(animationcurveValue.keys);
            }
            else if (Type == typeof(Gradient))
            {
                boxedValue = gradientValue;
            }
            else if (Type == typeof(UnityEngine.Object))
            {
                boxedValue = objectValue;
            }
            else
            {
                throw new Exception(
                    $"Unhandled GraphVariable type ${Type.Name}"
                );
            }
        }
    }
}
