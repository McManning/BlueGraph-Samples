using System;
using System.Text.RegularExpressions;
using UnityEngine;
using BlueGraph;
using System.Runtime.CompilerServices;

namespace BlueGraphSamples
{
    /// <summary>
    /// BlueGraph.Graph extension interface that adds support for managing variables.
    /// <br/><br/>
    /// Add the "Get Variables" and "Set Variables" tags in <c>[IncludeTags]</c>
    /// for nodes that work with this extension.
    /// </summary>
    public interface IContainsVariables : IGraph
    {
        VariableList Variables { get; }
    }

    public static class ContainsVariablesExtension
    {
        static ConditionalWeakTable<IContainsVariables, Fields> table;

        static ContainsVariablesExtension()
        {
            Debug.Log("init contains variables ext");
            table = new ConditionalWeakTable<IContainsVariables, Fields>();
        }

        private sealed class Fields
        {
            internal int foo = 5;
        }

        public static int GetFoo(this IContainsVariables self)
        {
            // But this isn't persisted/serialized on the graph in any way. 
            return table.GetOrCreateValue(self).foo;
        }

        public static GraphVariable FindVariable(this IContainsVariables self, string name)
        {
            return self.Variables.Find(name);
        }
        
        public static GraphVariable AddVariable(this IContainsVariables self, Type type)
        {
            var created = new GraphVariable()
            {
                Name = self.DeduplicateVariableName($"New {type.Name}"),
                Type = type
            };

            self.Variables.Add(created);
            return created;
        }

        public static void RemoveVariable(this IContainsVariables self, string name)
        {
            var variable = self.FindVariable(name);
            if (variable == null)
            {
                throw new Exception(
                    $"Tried to remove unknown variable {name}"
                );
            }

            self.Variables.Remove(variable);
            self.RevalidateVariableNodes();
        }

        public static void RenameVariable(this IContainsVariables self, string oldName, string newName)
        {
            if (oldName == newName) return;

            var variable = self.FindVariable(oldName);
            if (variable == null)
            {
                Debug.LogError(
                    $"Tried to rename unknown variable {oldName}"
                );
                return;
            }

            variable.Name = self.DeduplicateVariableName(newName, oldName);
            
            Debug.Log($"rename {oldName} to {variable.Name}");
            self.RevalidateVariableNodes();
        }

        /// <summary>
        /// Notify nodes that a change was made to one or more variables.
        /// </summary>
        public static void RevalidateVariableNodes(this IContainsVariables self)
        {
            // Rather than only notifying the nodes that refer to the
            // variable - we notify them ALL so references that
            // may have been dereferenced can recover themselves

            var getters = self.GetNodes<GetVariableNode>();
            foreach (var getter in getters)
            {
                getter.Validate();
            }

            var setters = self.GetNodes<SetVariableNode>();
            foreach (var setter in setters)
            {
                setter.Validate();
            }
        }

        /// <summary>
        /// Return a name like the input name but deduplicated from all other variables
        /// </summary>
        public static string DeduplicateVariableName(this IContainsVariables self, string name, string excludingName = null)
        {
            int largestIncrementalMatch = 1;
            bool foundMatch = false;

            // Deduplication is done by adding an incremented number
            // to the end of the input name, higher than any existing
            // incrementer for variables of the same name
            foreach (var v in self.Variables)
            {
                if (v.Name == excludingName)
                {
                    continue;
                }

                // Matches things like "Foo 1", "Foo 22", etc.
                var pattern = Regex.Escape(name) + @"(\s+\d+)?";
                var match = Regex.Match(v.Name, pattern);

                if (match.Success)
                {
                    foundMatch = true;
                    
                    string num = match.Groups[1].Value.Trim();
                    if (num.Length > 0)
                    {
                        largestIncrementalMatch = Mathf.Max(
                            largestIncrementalMatch, 
                            int.Parse(num)
                        );
                    }
                }
            }

            if (foundMatch)
            {
                return $"{name} {largestIncrementalMatch + 1}";
            }

            // Fine as-is
            return name;
        }
    }
}
