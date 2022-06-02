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
        bool showFullPath = false;
        string TestKey = "";

        public override void OnInspectorGUI()
        {
            TabalogHost tabalogHost = (TabalogHost)target;

            if (tabalogHost.GetLoadedText() != null)
            {
                showLoadedText = EditorGUILayout.Foldout(showLoadedText, "Loaded Tabalog Data");
                if (showLoadedText) EditorGUILayout.LabelField(tabalogHost.GetLoadedText(), EditorStyles.textArea);
            }
            else
            {
                EditorGUILayout.LabelField("No data loaded.");
            }


            if (tabalogHost.GetLoadedText() == null)
            {
                if (GUILayout.Button("Open File"))
				{
					tabalogHost.OpenFile();
					tabalogHost.ConvertToData();
				}

            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Reload File")) { tabalogHost.LoadFile(); tabalogHost.ConvertToData(); }
                if (GUILayout.Button("Unload File")) tabalogHost.ResetLoadedData();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (tabalogHost.GetLoadedText() != null)
            {
                float inspectorWidth = EditorGUIUtility.currentViewWidth;
                float size = Mathf.Min(Mathf.Clamp(GUI.skin.textField.CalcSize(new GUIContent(TestKey)).x, 48, inspectorWidth), inspectorWidth - 24);
                GUILayoutOption guiSize = GUILayout.Width(size);

                string valueText = tabalogHost[TestKey] ?? "Key not found";
                bool morelines = size > (inspectorWidth - GUI.skin.textField.CalcSize(new GUIContent(valueText)).x);

                if (!morelines) EditorGUILayout.BeginHorizontal();

                TestKey = EditorGUILayout.TextField(TestKey, guiSize);
                EditorGUILayout.LabelField(valueText);

                if (!morelines) EditorGUILayout.EndHorizontal();
            }

			EditorGUILayout.Space();

            if (tabalogHost.GetData() != null)
                display(tabalogHost.GetData());
            else
                EditorGUILayout.LabelField("Data is Invalid or There is nothing to display");
        }

        void display(Dictionary<string, string> data)
        {
            showFullPath = EditorGUILayout.Toggle("Show Full Path", showFullPath);

            foreach (KeyValuePair<string, string> kvp in data)
            {
                int indent = Regex.Matches(kvp.Key, @"/").Count;
                string key = kvp.Key;

                bool bold = false;
                if (key == TestKey) bold = true; 

                if (!showFullPath)
                {
                    var split = key.Split('/');
                    key = split[split.Length - 1];
                }

                EditorGUILayout.BeginHorizontal();

                for (int i = 0; i < indent; i++) EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(key, bold ? EditorStyles.boldLabel : EditorStyles.label);
                for (int i = 0; i < indent; i++) EditorGUI.indentLevel--;

                EditorGUILayout.LabelField((kvp.Value as string), bold ? EditorStyles.boldLabel : EditorStyles.label);

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
