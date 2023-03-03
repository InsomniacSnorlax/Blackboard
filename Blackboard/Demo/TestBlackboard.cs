using UnityEngine;
using Snorlax.BlackboardTest;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestBlackboard : MonoBehaviour
{
    public Blackboard Target;
    [Parameters("Target")]
    public Parameter parameters;
    public bool Changeit;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TestBlackboard))]
public class TestBlackboardEditor : Editor
{
    TestBlackboard board;

    private void OnEnable()
    {
        board = (TestBlackboard)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Parameter propertyParameters = board.parameters;
        if (GUILayout.Button("Test"))
        {
            Debug.Log($"Name = {propertyParameters.Name}");
            Debug.Log($"Type = {propertyParameters.Type}");
            Debug.Log($"Float = {propertyParameters.Float}");
            Debug.Log($"Int = {propertyParameters.Int}");
            Debug.Log($"Bool = {propertyParameters.Bool}");
            Debug.Log($"String = {propertyParameters.String}");
            Debug.Log($"Vector3 = {propertyParameters.Vector3}");
        }

        if(GUILayout.Button("Change It"))
        {
            board.Target.ChangeParamter(propertyParameters, board.Changeit);
        }
    }
}
#endif
