using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.Linq;

namespace Snorlax.BlackboardTest
{
    [Serializable]
    public class BlackboardEditorWindow : EditorWindow
    {
        #region Variables
        // Editor Values
        private string SearchString = "";
        private Vector2 scrollView = Vector2.zero;
        private int previousSelected = -1;
        private int currentSelected = -1;
        private int selectedBlackboard = -1;

        private const float height = 20f;
        private const float width = 100f;
        private Vector3 PopupSize = new Vector2(100, 100);
        private Vector3 CratePopupSize = new Vector2(300, 95);
        // Serialized 
        private SerializedProperty serializedProperty = null;
        private SerializedObject serializedObject = null;

        private ReorderableList reorderable;
        private static Blackboard currentBlackboard = null;
        private List<Blackboard> blackboards = new List<Blackboard>();

        // Textures
        private Texture2D iconPlusMore;
        private Texture2D iconDelete;

        // Editor Events
        private Event currentEvent;
        private bool isEditName = false;
        #endregion

        #region Default Methods
        [MenuItem("Snorlax's Tools/Blackboard")]
        public static void ShowWindow()
        {
            GetWindow<BlackboardEditorWindow>("Blackboard");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is Blackboard)
            {
                currentBlackboard = (Blackboard)Selection.activeObject;
                ShowWindow();
                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            this.minSize = new Vector2(200, 300);

            RefreshBlackboards(currentBlackboard);
            iconPlusMore = EditorGUIUtility.FindTexture("d_Toolbar Plus More");
            iconDelete = EditorGUIUtility.FindTexture("d_Toolbar Minus");
        }

        private void OnGUI()
        {
            if (serializedObject != null) serializedObject?.Update();

            currentEvent = Event.current;

            GUIUtilities.HorizontalWrapper(1, BlackboardList);

            InputHandler();

            GUIUtilities.VerticalWrapper(1, Main);

            if (serializedObject != null) serializedObject?.ApplyModifiedProperties();
            EditorUtility.SetDirty(currentBlackboard);
        }

        #endregion

        #region Editor Window Methods

        public void BlackboardList()
        {
            if (blackboards.Count <= 0)
            {
                GUILayout.Label("No blackboards found");
            }
            else
            {
                int newIndex = EditorGUILayout.Popup(selectedBlackboard, GetNames());

                if (selectedBlackboard != newIndex)
                {
                    currentBlackboard = blackboards[newIndex];
                    Reset();
                }
            }

            if (GUILayout.Button("-", GUILayout.Width(30f)))
            {
                if (blackboards.Count <= 0) return;
                serializedObject = null;
                serializedProperty = null;
                string assetPath = AssetDatabase.GetAssetPath(new SerializedObject(blackboards[selectedBlackboard]).targetObject);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                RefreshBlackboards();
            }

            if (GUILayout.Button("+", GUILayout.Width(30f)))
            {
                PopupWindow.Show(new Rect(currentEvent.mousePosition.x, currentEvent.mousePosition.y, 20, 20), new CreatePopupMenu() { action = RefreshBlackboards, size = CratePopupSize }); ;
            }
        }

        private void Main()
        {
            if (currentBlackboard == null || reorderable == null) return;

            GUIUtilities.HorizontalWrapper(0, SearchBarAndMore);

            if (!String.IsNullOrEmpty(SearchString) || reorderable.count == 0)
                GUIUtilities.ScrollViewWrapper(ref scrollView, NonReorderableList);
            else
                GUIUtilities.ScrollViewWrapper(ref scrollView, reorderable.DoLayoutList);
        }

        private void ReorderableList(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);

            Rect LabelRect = new Rect(rect.x, rect.y, rect.width - (5 + width), height);
            Rect ParaRect = new Rect(rect.width - (width - height), rect.y, width, height);

            if (isEditName == true && previousSelected == index)
                EditorGUI.PropertyField(LabelRect, element.FindPropertyRelative("Name"), GUIContent.none);
            else
                EditorGUI.LabelField(LabelRect, element.FindPropertyRelative("Name").stringValue);

            switch (currentBlackboard.parameters[index].Type)
            {
                case ParameterType.Float:
                    EditorGUI.PropertyField(ParaRect, element.FindPropertyRelative("Float"), GUIContent.none);
                    break;
                case ParameterType.Int:
                    EditorGUI.PropertyField(ParaRect, element.FindPropertyRelative("Int"), GUIContent.none);
                    break;
                case ParameterType.Bool:
                    EditorGUI.PropertyField(ParaRect, element.FindPropertyRelative("Bool"), GUIContent.none);
                    break;
                case ParameterType.String:
                    EditorGUI.PropertyField(ParaRect, element.FindPropertyRelative("String"), GUIContent.none);
                    break;
                case ParameterType.Vector3:
                    EditorGUI.PropertyField(ParaRect, element.FindPropertyRelative("Vector3"), GUIContent.none);
                    break;
            }

            if (reorderable.IsSelected(index)) currentSelected = index;

            serializedObject.ApplyModifiedProperties();
        }

        private void NonReorderableList()
        {
            Color defaultColor = GUI.backgroundColor;
            for (int i = 0; i < currentBlackboard.parameters.Count; i++)
            {
                Parameter parameter = currentBlackboard.parameters[i];

                if (!GUIUtilities.StringContains(parameter.Name, SearchString))
                    continue;
                GUI.backgroundColor = currentSelected == i ? Color.blue : defaultColor;
                Rect rect = EditorGUILayout.BeginHorizontal("Box", GUILayout.Height(height));
                {
                    GUI.backgroundColor = defaultColor;
                    if (isEditName == true && currentSelected == i)
                    {
                        parameter.Name = EditorGUILayout.TextField(parameter.Name);

                    }
                    else EditorGUILayout.LabelField(parameter.Name);

                    switch (parameter.Type)
                    {
                        case ParameterType.Float:
                            currentBlackboard.parameters[i].Float = EditorGUILayout.FloatField(parameter.Float, GUILayout.Width(width));
                            break;
                        case ParameterType.Int:
                            currentBlackboard.parameters[i].Int = EditorGUILayout.IntField(parameter.Int, GUILayout.Width(width));
                            break;
                        case ParameterType.Bool:
                            currentBlackboard.parameters[i].Bool = EditorGUILayout.Toggle(parameter.Bool, GUILayout.Width(width));
                            break;
                        case ParameterType.String:
                            parameter.String = EditorGUILayout.TextField(parameter.String, GUILayout.Width(width));
                            break;
                        case ParameterType.Vector3:
                            parameter.Vector3 = EditorGUILayout.Vector3Field(GUIContent.none, parameter.Vector3, GUILayout.Width(width));
                            break;
                    }

                    if (rect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                    {
                        currentSelected = i;
                    }

                }
                EditorGUILayout.EndHorizontal();
                EditorUtility.SetDirty(currentBlackboard);
            }
        }

        private void InputHandler()
        {
            if (currentEvent.keyCode == KeyCode.Return)
            {
                isEditName = false;
                this.Repaint();
            }

            if (previousSelected != currentSelected)
            {
                isEditName = false;
                previousSelected = currentSelected;
                this.Repaint();
            }

            if (currentEvent.clickCount == 2)
            {
                isEditName = true;
            }
        }

        #endregion

        #region Utilities 
        public void SearchBarAndMore()
        {
            SearchString = GUILayout.TextField(SearchString, GUI.skin.FindStyle("ToolbarSeachTextField"));

            if (GUILayout.Button(iconDelete, GUI.skin.FindStyle("toolbarbutton"), GUILayout.Width(30)))
            {
                Delete();
            }

            if (GUILayout.Button(iconPlusMore, GUI.skin.FindStyle("toolbarbutton"), GUILayout.Width(30)))
            {
                PopupWindow.Show(new Rect(currentEvent.mousePosition.x, currentEvent.mousePosition.y, 20, 20), new PopupMenu() { action = TypePopup, size = PopupSize });
            }
        }

        public void TypePopup(ParameterType type)
        {
            currentBlackboard.AddParameter($"New {type}", type);
            Reset();
        }

        private void Reset()
        {
            if (currentBlackboard == null)
                return;

            selectedBlackboard = blackboards.IndexOf(currentBlackboard);

            serializedObject = new SerializedObject(currentBlackboard);
            serializedProperty = serializedObject.FindProperty("parameters");

            if (reorderable != null)
            {
                reorderable.drawElementCallback -= ReorderableList;
            }

            reorderable = new ReorderableList(serializedObject, serializedProperty, true, false, false, false);


            reorderable.drawElementCallback += ReorderableList;
            this.Repaint();
        }

        private void Delete()
        {
            if (previousSelected == -1) return;
            currentBlackboard.RemoveParameter(previousSelected);
            Reset();
            currentSelected = -1;
            previousSelected = -1;
        }

        public string[] GetNames()
        {
            List<string> names = new List<string>();

            foreach(Blackboard board in blackboards)
            {
                names.Add(board.name);
            }

            return names.ToArray();
        }

        private void RefreshBlackboards(Blackboard current = null)
        {
            blackboards = GUIUtilities.GetAllInstances<Blackboard>();
            if (blackboards.Count <= 0) return;
            currentBlackboard = current != null ? current : blackboards[blackboards.Count - 1];
            selectedBlackboard = blackboards.IndexOf(currentBlackboard);
            Reset();
        }

        #endregion
    }
}