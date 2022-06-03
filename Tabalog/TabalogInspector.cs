using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

namespace Tabalog
{
    [CustomEditor(typeof(TabalogHost))]
    public class TabalogInspector : Editor
    {
        bool showLoadedText = false;
        string TestKey = "";

        TabalogHost tabalogHost;

        public override void OnInspectorGUI()
        {
            tabalogHost = (TabalogHost)target;

            if (tabalogHost.GetLoadedText().IsValid())
            {
                showLoadedText = EditorGUILayout.Foldout(showLoadedText, $"Loaded Tabalog Data : {tabalogHost.GetLoadedText().Length} chars");
                if (showLoadedText) EditorGUILayout.LabelField(tabalogHost.GetLoadedText(), EditorStyles.textArea);
            }
            else
            {
                EditorGUILayout.LabelField("No data loaded.");
            }


            if (!tabalogHost.GetPath().IsValid())
            {
                if (GUILayout.Button("Open File"))
				{
					tabalogHost.OpenFile();
				}
            }
            else
            {
                EditorGUILayout.LabelField(tabalogHost.GetPath());
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reload File")) tabalogHost.LoadFile();
                if (GUILayout.Button("Unload File")) tabalogHost.ResetLoadedData();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Pack and Save")) tabalogHost.Save();
                if (GUILayout.Button("Pack and Save As")) tabalogHost.SaveAs();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(32);

            if (tabalogHost.GetLoadedText().IsValid() && tabalogHost.GetData() != null)
            {
                float inspectorWidth = EditorGUIUtility.currentViewWidth;
                float size = Mathf.Min(Mathf.Clamp(GUI.skin.textField.CalcSize(new GUIContent(TestKey)).x + 8, 48, inspectorWidth), inspectorWidth - 24);
                GUILayoutOption guiSize = GUILayout.Width(size);


                EditorGUILayout.BeginHorizontal();

                TestKey = EditorGUILayout.TextField(TestKey, guiSize);

                if (TestKey.IsValid())
                {
                    if (!tabalogHost.Exists(TestKey))
                    {
                        if(GUILayout.Button("Add Key", GUILayout.Width(GUI.skin.textField.CalcSize(new GUIContent("Add Key")).x + 12)))
                        {
                            tabalogHost[TestKey] = "";
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Remove Key", GUILayout.Width(GUI.skin.textField.CalcSize(new GUIContent("Remove Key")).x + 12)))
                        {
                            tabalogHost.Remove(TestKey);
                        }
                        else
                        {
                            EditorGUILayout.EndHorizontal();
                            string value = tabalogHost[TestKey, false];
                            tabalogHost[TestKey] = EditorGUILayout.TextField(value);
                            EditorGUILayout.BeginHorizontal();
                        }
                    }
                }
                else
                {
                    GUILayout.Label("Input a Key to add, remove, or modify\t(Ex : \"Key/SubKey/A\")");
                }

                EditorGUILayout.EndHorizontal();
            }

            if (tabalogHost.GetPath().IsValid())
            {
                EditorGUILayout.Space(24);

                if (tabalogHost.GetData() != null)
                    display(tabalogHost.GetData());
                else
                    EditorGUILayout.LabelField("Data is Invalid or There is nothing to display");
            }

        }

        void display(Dictionary<string, string> data)
        {
            EditorGUILayout.TextArea(tabalogHost.Pack(), EditorStyles.label);
        }
    }
}
