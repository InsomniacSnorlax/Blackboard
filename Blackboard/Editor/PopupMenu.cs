using UnityEditor;
using UnityEngine;
using System;
using System.IO;

namespace Snorlax.BlackboardTest
{
    public class PopupMenu : PopupWindowContent
    {
        public Action<ParameterType> action;
        public Vector2 size = new Vector2();

        ParameterType type;
        bool triggered = false;

        public override Vector2 GetWindowSize()
        {
            return size;
        }

        public override void OnGUI(Rect rect)
        {
            GUIStyle style = GUI.skin.FindStyle("toolbarbutton");

            if (GUILayout.Button("Float", style))
            {
                CloseStatement(ParameterType.Float);
            }
            if (GUILayout.Button("Int", style))
            {
                CloseStatement(ParameterType.Int);
            }
            if (GUILayout.Button("Boolean", style))
            {
                CloseStatement(ParameterType.Bool);
            }
            if (GUILayout.Button("String", style))
            {
                CloseStatement(ParameterType.String);
            }
            if (GUILayout.Button("Vector3", style))
            {
                CloseStatement(ParameterType.Vector3);
            }
        }

        private void CloseStatement(ParameterType paraType)
        {
            type = paraType;
            triggered = true;
            editorWindow.Close();
        }

        public override void OnClose()
        {
            if (!triggered)
                return;
            action(type);
        }
    }

    public class CreatePopupMenu : PopupWindowContent
    {
        public Action<Blackboard> action;
        public Vector2 size = new Vector2();

        private DefaultAsset targetFolder = null;
        public readonly string BlackboardPaths = "Assets/Blackboard/ScriptableObjects";
        public readonly string BlackboardKeyName = "BlackboardSaves";

        string InputName = "";

        public override Vector2 GetWindowSize()
        {
            return size;
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginVertical("Box");

            HandleSaveFolderEditorPref(BlackboardKeyName, BlackboardPaths, "Blackboards");
            GUIUtilities.HorizontalWrapper(1, CreateButton);

            EditorGUILayout.EndVertical();
        }

        private void CreateButton()
        {
            GUILayout.Label("Blackboard Name", GUILayout.Width(120f));
            InputName = GUILayout.TextField(InputName);
            if(GUILayout.Button("Create", "toolbarbutton", GUILayout.Width(60f)))
            {
                SaveScriptableObject(BlackboardKeyName, BlackboardPaths, ScriptableObject.CreateInstance<Blackboard>(), InputName);
                action(null);
                editorWindow.Close();
            }
        }


        #region Save Methods
        public static void SaveScriptableObject(string keyName, string defaultPath, ScriptableObject saveObject, string name)
        {
            if (name == null)
            {
                Debug.Log("Empty Name");
                return;
            }

            string path = defaultPath;
            if (PlayerPrefs.HasKey(keyName)) path = PlayerPrefs.GetString(keyName);
            else PlayerPrefs.SetString(keyName, defaultPath);
            path += "/";
            if (!System.IO.Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("The desired save folder doesn't exist",
                    "Make sure you select a valid folder", "Ok");
                return;
            }

            path += name;
            string fullPath = path + ".asset";
            if (System.IO.File.Exists(fullPath))
            {
                SaveScriptableObjectWithOtherName(path, saveObject);
            }
            else DoSaving(fullPath, saveObject);
        }

        private static void SaveScriptableObjectWithOtherName(string path, ScriptableObject saveObject, int i = 1)
        {
            int number = i;
            string newPath = path + "_" + number.ToString();
            string fullPath = newPath + ".asset";
            if (File.Exists(fullPath))
            {
                number++;
                SaveScriptableObjectWithOtherName(path, saveObject, number);
            }
            else
            {
                DoSaving(fullPath, saveObject);
            }
        }

        private static void DoSaving(string fileName, ScriptableObject saveObject)
        {
            AssetDatabase.CreateAsset(saveObject, fileName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void HandleSaveFolderEditorPref(string keyName, string defaultPath, string logsFeatureName)
        {
            if (!PlayerPrefs.HasKey(keyName))
                PlayerPrefs.SetString(keyName, defaultPath);

            targetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(keyName), typeof(DefaultAsset));

            if (targetFolder == null)
            {
                PlayerPrefs.SetString(keyName, defaultPath);
                targetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath(PlayerPrefs.GetString(keyName), typeof(DefaultAsset));

                if (targetFolder == null)
                {
                    targetFolder = (DefaultAsset)AssetDatabase.LoadAssetAtPath("Assets/", typeof(DefaultAsset));
                    if (targetFolder == null)
                        Debug.LogWarning("The desired save folder doesn't exist. " + PlayerPrefs.GetString(keyName) +
                                                                     "\n Make sure to set a valid folder");
                    else
                        PlayerPrefs.SetString("Assets/", defaultPath);
                }
            }

            targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("New " + logsFeatureName + " Folder", targetFolder, typeof(DefaultAsset), false);

            if (targetFolder != null && IsAssetAFolder(targetFolder))
            {
                string path = AssetDatabase.GetAssetPath(targetFolder); //EditorUtility.OpenFilePanel("Open Folder", "", "");
                PlayerPrefs.SetString(keyName, path);
                EditorGUILayout.HelpBox("Valid folder! " + logsFeatureName + " save path: " + path, MessageType.Info, true);
            }
            else EditorGUILayout.HelpBox("Select the new " + logsFeatureName + " Folder", MessageType.Warning, true);
        }

        private static bool IsAssetAFolder(UnityEngine.Object obj)
        {
            string path = "";

            if (obj == null) return false;

            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (path.Length > 0)
            {
                if (Directory.Exists(path)) return true;
                else return false;
            }

            return false;
        }
        #endregion
    }
}