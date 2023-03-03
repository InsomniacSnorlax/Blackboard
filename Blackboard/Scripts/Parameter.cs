using UnityEngine;

namespace Snorlax.BlackboardTest
{
    [System.Serializable]
    public class Parameter
    {
        public string Name;

        public int nameHash
        {
            get { return Name.GetHashCode(); }
        }

        public ParameterType Type;
        public float Float;
        public int Int;
        public bool Bool;
        public string String;
        public Vector3 Vector3;

        public override bool Equals(object o)
        {
            Parameter other = (Parameter)o;
            return Name == other.Name && Type == other.Type && Float == other.Float && Int == other.Int && Bool == other.Bool;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public enum ParameterType
    {
        //
        // Summary:
        //     Float type parameter.
        Float = 1,
        //
        // Summary:
        //     Int type parameter.
        Int = 3,
        //
        // Summary:
        //     Boolean type parameter.
        Bool = 4,
        //
        // Summary:
        //     String type parameter.
        String = 9,
        //
        // Summary:
        //     String type parameter.
        Vector3 = 12
    }
}