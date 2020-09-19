
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BlueGraph;
using UnityEditor.IMGUI.Controls;
using BlueGraph.Editor;

namespace BlueGraphSamples
{
    [CustomPropertyDrawer(typeof(VariableList))]
    public class VariableListDrawer : PropertyDrawer
    {
        public const int TABLE_HEIGHT = 200;
        
        [NonSerialized] bool initialized;
        SearchField search;
        
        GraphVariablesTable table;
        [SerializeField] TreeViewState tableState;

        /// <summary>
        /// Setup UI elements if not already
        /// </summary>
        private void Initialize(SerializedProperty arrayProperty)
        {
            if (!initialized)
            {
                if (tableState == null)
                {
                    tableState = new TreeViewState();
                }

                var headerState = GraphVariablesTableHeader.CreateDefaultState();
                var header = new GraphVariablesTableHeader(headerState);

                table = new GraphVariablesTable(tableState, header, arrayProperty);
                search = new SearchField();

                initialized = true;
                Debug.Log("drawer is init");
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var graph = (property.serializedObject.targetObject) as IContainsVariables;
            
            var arr = property.FindPropertyRelative("entries");
            if (arr == null)
            {
                throw new Exception("Could not find `entries` of serialized property");
            }

            Initialize(arr);

            GUILayout.Label(property.displayName);
            
            // Header search
            using (var scope = new EditorGUILayout.VerticalScope(GUILayout.Height(20)))
            {
                table.searchString = search.OnGUI(scope.rect, table.searchString);
                EditorGUILayout.Space();
            }
            
            // The table of variables
            using (var scope = new EditorGUILayout.VerticalScope(GUILayout.Height(200)))
            {
                table.OnGUI(scope.rect);
                EditorGUILayout.Space();
            }

            // Footer buttons
            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                // Delete selected variables from the underlying model
                if (table.HasSelection())
                {
                    if (GUILayout.Button("Delete Selected"))
                    {
                        foreach (var selected in table.GetSelectedVariables())
                        {
                            graph.RemoveVariable(selected.Name); // todo: just pass ref.
                        }

                        table.SetSelection(new int[0]);
                        
                        EditorUtility.SetDirty(graph as UnityEngine.Object);
                        table.Reload();
                    }
                }

                // Add a new variable from a menu of enumerated types that are supported
			    if (EditorGUILayout.DropdownButton(
                    new GUIContent("Add Variable"), 
                    FocusType.Passive
                )) {
                    GenericMenu menu = new GenericMenu();

                    foreach (var type in GraphVariable.SupportedTypes)
                    {
                        menu.AddItem(new GUIContent(type.ToPrettyName()), false, () => {
                            graph.AddVariable(type);
                            EditorUtility.SetDirty(graph as UnityEngine.Object);
                            table.Reload();
                        });
                    }

                    menu.ShowAsContext();
			    }
            }
            
            EditorGUI.EndProperty();
        }
    }
}
