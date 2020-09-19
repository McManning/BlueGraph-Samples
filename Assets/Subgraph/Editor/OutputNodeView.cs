﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using BlueGraph.Editor;
using BlueGraph;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace BlueGraphSamples
{
    [CustomNodeView(typeof(OutputNode))]
    public class OutputNodeView : NodeView
    {
        private TextField nameField;
        private PopupField<string> typeField;
        private PortView portField;
        
        /// <summary>
        /// Input types allowed for this subgraph
        /// </summary>
        static readonly Type[] k_SupportedTypes = new Type[]
        {
            typeof(float),
            typeof(string),
            typeof(bool),
            typeof(Vector2),
            typeof(Vector3)
        };

        /// <summary>
        /// Add custom classes and field editors on initialize
        /// </summary>
        protected override void OnInitialize()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("BlueGraphSamples/OutputNodeView"));
            AddToClassList("subgraphOutput");

            // Replace the title bar with an editable name
            nameField = new TextField
            {
                value = Target.Name
            };

            nameField.RegisterValueChangedCallback((change) => OnSettingsChange());
            nameField.RegisterCallback<FocusOutEvent>((e) => { 
                if (nameField.value.Length < 1)
                {
                    nameField.value = "New Output";
                }
            });
            
            var icon = new VisualElement();
            icon.name = "icon";

            var title = this.Q("title");
            title.Clear();

            title.Add(nameField);
            title.Add(icon);
        }
        
        /// <summary>
        /// Add a custom PopupField associated with the input port 
        /// to dynamically modify the type of input for this node 
        /// </summary>
        /// <param name="port"></param>
        protected override void AddInputPort(Port port)
        {
            portField = PortView.Create(port, ConnectorListener);
            
            var defaultIndex = 0;
            var names = new List<string>();
            for (var i = 0; i < k_SupportedTypes.Length; i++)
            {
                names.Add(k_SupportedTypes[i].Name);

                if (k_SupportedTypes[i] == port.Type) 
                {
                    defaultIndex = i;
                }
            }

            typeField = new PopupField<string>(names, defaultIndex);
            typeField.RegisterValueChangedCallback((change) => OnSettingsChange());

            var container = new VisualElement();
            container.AddToClassList("property-field-container");
            container.Add(typeField);

            portField.SetEditorField(container);
            portField.HideEditorFieldOnConnection = false;

            Inputs.Add(portField);
            inputContainer.Add(portField);
        }

        void OnSettingsChange()
        {
            title = nameField.value;
            Target.Name = nameField.value;

            foreach (var type in k_SupportedTypes)
            {
                if (type.Name == typeField.value)
                {
                    UpdateInputType(type);
                }
            }
        }

        void UpdateInputType(Type type)
        {
            // If the port is already of the same type, don't regenerate
            var port = portField.Target;
            if (port.Type == type)
            {
                return;
            }

            // TODO: Cleaner way of refactoring a port. This is awful.

            // Disconnect all existing connections. 
            // This has to be done from the canvas view.
            var canvas = GetFirstAncestorOfType<CanvasView>();
            var edges = new List<Edge>(portField.connections);
            foreach (var edge in edges) 
            {
                canvas.RemoveEdge(edge, false);
            }

            // Remove references
            inputContainer.Remove(portField);
            Inputs.Remove(portField);
            Target.RemovePort(port);
            portField = null;

            // Create a new replacement port of the new type
            port = new Port
            {
                Direction = PortDirection.Input,
                Type = type
            };
            
            Target.AddPort(port);
            AddInputPort(port);
        }
    }
}
