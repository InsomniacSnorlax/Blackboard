using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.Animations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Snorlax.BlackboardTest
{
    [CreateAssetMenu(menuName = "Snorlax's Tools/Blackboard")]
    public class Blackboard : ScriptableObject
    {
        public List<Parameter> parameters = new List<Parameter>();

        #region Editor Methods
#if UNITY_EDITOR

        public void AddParameter(string name, ParameterType type)
        {
            Parameter newParameter = new Parameter();
            newParameter.Name = MakeUniqueParameterName(name);
            newParameter.Type = type;

            AddParameter(newParameter);
        }

        public void AddParameter(Parameter paramater)
        {
            Undo.RecordObject(this, "Parameter added");
            // List<Parameter> parameterVector = parameters;
            //ArrayUtility.Add(ref parameterVector, paramater);
            parameters.Add(paramater);// = parameterVector;
        }

        public void RemoveParameter(int index)
        {
            Undo.RecordObject(this, "Parameter removed");
            //Parameter[] parameterVector = parameters;
           // ArrayUtility.Remove(ref parameterVector, parameterVector[index]);
            parameters.Remove(parameters[index]);
        }

        public void RemoveParameter(Parameter parameter)
        {
            Undo.RecordObject(this, "Parameter removed");
            //Parameter[] parameterVector = parameters;
            //ArrayUtility.Remove(ref parameterVector, parameter);
            parameters.Remove(parameter);// = parameterVector;
        }

        public Parameter GetIndex(int index)
        {
            return parameters[index];
        }

        public int FindIndexOfName(string name)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (name == parameters[i].Name)
                    return i;
            }

            return 0;
        }


        private bool ContainsName(string name)
        {
            for (int i = 0; i < parameters.Count; i++) // Parameters parameters in parameters)
            {
                if (parameters[i].Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public string MakeUniqueParameterName(string name)
        {
            if (ContainsName(name))
            {
                return NameWithNumber(name);
            }
            else return name;
        }

        private string NameWithNumber(string name, int i = 1)
        {
            int number = i;
            string fullName = name + $" {number}";
            if (ContainsName(fullName))
            {
                number++;
                return NameWithNumber(name, number);
            }
            else
            {
                return fullName;
            }
        }

        public string[] ReturnNames(ParameterType? type)
        {
            List<string> Names = new List<string>();
            for (int i = 0; i < parameters.Count; i++)
            {
                if(type == null || parameters[i].Type == type)
                    Names.Add(parameters[i].Name);
            }

            return Names.ToArray();
        }
#endif
        #endregion

        #region Edit Parameter Methods
        public void ChangeParamter(Parameter parameter, float Float)
        {
            parameters[IndexOf(parameter)].Float = Float;
        }

        public void ChangeParamter(Parameter parameter, int Int)
        {
            parameters[IndexOf(parameter)].Int = Int;
        }

        public void ChangeParamter(Parameter parameter, bool Bool)
        {
            parameters[IndexOf(parameter)].Bool = Bool;
        }

        public void ChangeParamter(Parameter parameter, string String)
        {
            parameters[IndexOf(parameter)].String = String;
        }

        public void ChangeParamter(Parameter parameter, Vector3 Vector3)
        {
            parameters[IndexOf(parameter)].Vector3 = Vector3;
        }

        public int IndexOf(Parameter parameter)
        {
            Debug.Log(parameter.Name);
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameter.Name == parameters[i].Name)
                    return i;
            }

            return 0;
        }
        #endregion
    }
}