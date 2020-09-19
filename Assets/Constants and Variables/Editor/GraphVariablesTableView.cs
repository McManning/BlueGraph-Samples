using BlueGraph.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace BlueGraphSamples
{
    public enum GraphVariableTableColumn
    {
        Name,
        Type,
        DefaultValue,
        Constant
    }

    /// <summary>
    /// Header/column configurations for the table
    /// </summary>
    public class GraphVariablesTableHeader : MultiColumnHeader
    {
        // TODO: This class is kinda pointless unless I can find a reason
        // to extend off of it. The header state can just be a factory
        // inside the table class.

        public static MultiColumnHeaderState CreateDefaultState()
        {
            return new MultiColumnHeaderState(new []
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
					headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 200
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Type"),
					headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 60
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Default Value"),
					headerTextAlignment = TextAlignment.Left,
                    canSort = false,
					width = 150, 
					minWidth = 60
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Constant"),
					headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 60,
                    minWidth = 60
                }
            });
        }

        public GraphVariablesTableHeader(MultiColumnHeaderState state) : base(state)
        {

        }

        protected override void ColumnHeaderGUI(MultiColumnHeaderState.Column column, Rect headerRect, int columnIndex)
        {
            base.ColumnHeaderGUI(column, headerRect, columnIndex);
        }
    }
    
    /// <summary>
    /// A row representing a GraphVariable instance in the table
    /// </summary>
    public class GraphVariableRow : TreeViewItem
    {
        public GraphVariable Variable { get; private set; }

        public GraphVariableRow(int id, GraphVariable variable)
        {
            this.id = id;
            Variable = variable;
            displayName = variable.Name;
            // icon = EditorGUIUtility.FindTexture("Folder Icon"); // could be cached.
        }
    }

    /// <summary>
    /// Editor GUI table representation of a GraphVariables object.
    /// </summary>
    public class GraphVariablesTable : TreeView
    {
        TreeViewItem root;
        readonly List<TreeViewItem> rows = new List<TreeViewItem>();

        readonly SerializedProperty serializedArray;
        
        public SerializedObject SerializedGraph
        {
            get { return serializedArray.serializedObject; }
        }

        public GraphWithVariables Graph
        {
            get { return SerializedGraph.targetObject as GraphWithVariables; }
        }

		public GraphVariablesTable(
            TreeViewState state, 
            GraphVariablesTableHeader headers, 
            SerializedProperty property
        ) : base(state, headers)
		{
            serializedArray = property;

            showAlternatingRowBackgrounds = true;
            showBorder = true;

            Reload();
		}

        protected override TreeViewItem BuildRoot()
        {
            root = new TreeViewItem
            {
                displayName = "Variables",
                depth = -1
            };

            return root;
        }
        
        /// <summary>
        /// Fill table with GraphVariableRows that match the current search
        /// </summary>
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
            rows.Clear();

            foreach (var variable in Graph.Variables)
            {
                if (MatchesSearchString(variable))
                {
                    rows.Add(new GraphVariableRow(rows.Count, variable));
                }
            }
            
            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            // serializedModel.serializedObject.Update();

            var row = args.item as GraphVariableRow;

			for (int i = 0; i < args.GetNumVisibleColumns(); i++)
			{
				CellGUI(
                    args.GetCellRect(i), 
                    row, 
                    (GraphVariableTableColumn)args.GetColumn(i), 
                    ref args
                );
			}
        }

        /// <summary>
        /// Add custom GUI controls for each component of a variable
        /// </summary>
        private void CellGUI(
            Rect cellRect, 
            GraphVariableRow row, 
            GraphVariableTableColumn column, 
            ref RowGUIArgs args
        ) {			
            CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
                case GraphVariableTableColumn.Name:
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                    break;
                case GraphVariableTableColumn.Type:
                    EditorGUI.LabelField(cellRect, row.Variable.Type.Name);
                    break;
                case GraphVariableTableColumn.DefaultValue:
                    DefaultValueGUI(cellRect, row, ref args);
                    break;
                case GraphVariableTableColumn.Constant:
                    var current = row.Variable.IsConstant;
                    row.Variable.IsConstant = EditorGUI.Toggle(cellRect, current);

                    if (row.Variable.IsConstant != current)
                    {
                        // Modification was made to the underlying graph model, not the SO.
                        // Could instead be a change check + editorgui.propertyfield for a bool.
                        EditorUtility.SetDirty(Graph);
                        Graph.RevalidateVariableNodes();
                    }
                    break;
                default:
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                    break;
            }
        }

        /// <summary>
        /// Render an editor for the value of the GraphVariable
        /// </summary>
        private void DefaultValueGUI(Rect cellRect, GraphVariableRow row, ref RowGUIArgs args)
        {
            // Here's some dumb magic - we traverse through the serialization
            // of the GraphVariablesList to retrieve the serialized copy of the 
            // variable - and then access the correct field by a name matching
            // the type. 
            var serializedVariable = serializedArray.GetArrayElementAtIndex(row.id);
            
            var name = row.Variable.Type.Name.ToLower();
            name = $"{name}Value";

            var serializedValue = serializedVariable.FindPropertyRelative(name);

            if (serializedValue == null)
            {
                throw new Exception(
                    $"Cannot resolve field GraphVariable.{name}. " +
                    "You may have added an entry to GraphVariable.SupportedTypes without " +
                    "adding a matching value field."
                );
            }
            
            // Let Unity take over the heavy lifting of rendering an editor 
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.PropertyField(cellRect, serializedValue, GUIContent.none, true);
            
            if (EditorGUI.EndChangeCheck()) 
            {
                // Debug.Log($"Found change for {row.Variable.Name}");
               // SerializedGraph.ApplyModifiedProperties();
            }

            // TODO: Vector4 is unusable, since it's multiple lines and our graph rows
            // don't support that. Ideas? 
        }

        /*
		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
            return 80f;
		}*/

        private bool MatchesSearchString(GraphVariable variable)
        {
            return string.IsNullOrEmpty(searchString) 
                || variable.Name.IndexOf(
                    searchString, 
                    StringComparison.OrdinalIgnoreCase
                ) >= 0;
        }

        public List<GraphVariable> GetSelectedVariables()
        {
            // A copy is made here to avoid issues with updating
            // the variables list from within the iterator when 
            // we're using indices as IDs.

            var selected = new List<GraphVariable>();

            foreach (var index in GetSelection())
            {
                selected.Add((rows[index] as GraphVariableRow).Variable);
            }

            return selected;
        }

        protected override bool CanRename(TreeViewItem item)
        {
			// Only allow rename if we can show the rename overlay 
            // with a certain width (label might be clipped by other columns)
			Rect renameRect = GetRenameRect(treeViewRect, 0, item);
			return renameRect.width > 30;
        }
        
		protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
		{
			Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
			return base.GetRenameRect(cellRect, row, item);
		}

		protected override void RenameEnded(RenameEndedArgs args)
		{
			if (args.acceptedRename)
			{
                Graph.RenameVariable(args.originalName, args.newName);
                EditorUtility.SetDirty(Graph);

				Reload();
			}
		}
    }
}
