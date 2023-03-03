using System;
using UnityEngine;

namespace Snorlax.BlackboardTest
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ParametersAttribute : PropertyAttribute
    {
        public string AttributeName { get; private set; }
        public ParameterType? AttributeType { get; private set; }

        public ParametersAttribute(string name)
        {
            AttributeName = name;
            AttributeType = null;
        }

        public ParametersAttribute(string name, ParameterType type)
        {
            AttributeName = name;
            AttributeType = type;
        }
    }
}