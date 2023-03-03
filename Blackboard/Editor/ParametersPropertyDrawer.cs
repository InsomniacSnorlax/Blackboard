using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Snorlax.BlackboardTest
{
    [CustomPropertyDrawer(typeof(ParametersAttribute))]
    public class ParametersPropertyDrawer : PropertyDrawer
    {
        ParametersAttribute parameterAttributes;
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            parameterAttributes = GUIUtilities.GetAttribute<ParametersAttribute>(property);

            Blackboard blackboard = GetBlackboard(property, parameterAttributes.AttributeName);

            if (blackboard == null)
            {
                EditorGUI.HelpBox(rect, "No blackboard found", MessageType.Warning);
                return;
            }

            int parametersCount = blackboard.parameters.Count;
            List<Parameter> parameters = new List<Parameter>(); 

            for(int i = 0; i < parametersCount; i++)
            {
                Parameter parameter = blackboard.parameters[i];
                if(parameterAttributes.AttributeType == null || parameter.Type == parameterAttributes.AttributeType)
                {
                    parameters.Add(parameter);
                }
            }
            DrawProperties(rect, property, label, parameters, blackboard);

            EditorGUI.EndProperty();
        }

        private void DrawProperties(Rect rect, SerializedProperty property, GUIContent label, List<Parameter> parameters, Blackboard blackboard)
        {
            string name = property.FindPropertyRelative("Name").stringValue;
            int index = 0;
            if(parameters.Count < 1)
            {
                EditorGUI.HelpBox(rect, "No parameters found", MessageType.Warning);
                return;
            }


            for (int i = 0; i < parameters.Count; i++)
            {
                if (name.Equals(parameters[i].Name, System.StringComparison.Ordinal))
                {
                    index = i;
                    break;
                }
            }

            string[] displayOptions = blackboard.ReturnNames(parameterAttributes.AttributeType);

            int newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            string newValue =  parameters[newIndex].Name;
            int originalIndex = blackboard.FindIndexOfName(newValue);

            SerializedProperty element = new SerializedObject(blackboard).FindProperty("parameters").GetArrayElementAtIndex(originalIndex);
            if (!property.FindPropertyRelative("Name").stringValue.Equals(newValue, System.StringComparison.Ordinal))
            {
                property.FindPropertyRelative("Name").stringValue = element.FindPropertyRelative("Name").stringValue;
                property.FindPropertyRelative("Type").enumValueIndex = element.FindPropertyRelative("Type").enumValueIndex;
                property.FindPropertyRelative("Float").floatValue = element.FindPropertyRelative("Float").floatValue;
                property.FindPropertyRelative("Int").intValue = element.FindPropertyRelative("Int").intValue;
                property.FindPropertyRelative("Bool").boolValue = element.FindPropertyRelative("Bool").boolValue;
                property.FindPropertyRelative("String").stringValue = element.FindPropertyRelative("String").stringValue;
                property.FindPropertyRelative("Vector3").vector3Value = element.FindPropertyRelative("Vector3").vector3Value;
            }
        } 
    
        private static Blackboard GetBlackboard(SerializedProperty property, string name)
        {
            object target = GUIUtilities.GetTargetObjectWithProperty(property);

            FieldInfo fieldInfo = GUIUtilities.GetField(target, name);
            if (fieldInfo != null && fieldInfo.FieldType == typeof(Blackboard))
            {
                return fieldInfo.GetValue(target) as Blackboard;
            }

            return null;
        }
    }
}