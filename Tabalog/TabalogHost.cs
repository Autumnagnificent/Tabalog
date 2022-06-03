using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Tabalog
{
    public class TabalogHost : MonoBehaviour
    {
        private string _path;
        private string _loadedtext;
        private Dictionary<string, string> _data;

        void OnValidate()
        {
            if (_path.IsValid()) LoadFile();
        }

        public string this[string key, bool ThrowErrorIfNotFound = true]
        {
            get
            {
                if (_data == null)
                {
                    Debug.LogError("Data is null");
                    return null;
                }
                    
                if (_data.ContainsKey(key))
                {
                    return _data[key];
                }
                else
                {
                    if (ThrowErrorIfNotFound) Debug.LogError("Key not found");
                    return null;
                }
            }

            set
            {
                if (_data == null)
                {
                    Debug.LogError("Data is null");
                    return;
                }

                if (_data.ContainsKey(key))
                {
                    _data[key] = value;
                }
                else
                {
                    // Debug.LogWarning("Creating new key(s)");
                    string[] keys = key.Split('/');
                    string[] newkeys = new string[keys.Length];

                    for (int i = 0; i < keys.Length; i++)
                    {
                        var pastkeys = keys.Take(i + 1).ToList();
                        string p = pastkeys.Aggregate((a, b) => a + "/" + b);
                        newkeys[i] = p;
                    }

                    for (int i = 0; i < newkeys.Length; i++)
                    {
                        string k = newkeys[i];
                        if (!_data.ContainsKey(k))
                        {
                            _data.Add(k, "");
                        }
                    }
                }
            }
        }

        public void OpenFile()
        {
            string newpath = EditorUtility.OpenFilePanel("Open Tabalog File", Application.dataPath, "tabalog");

            if (!newpath.IsValid()) { Debug.LogError("[TabalogHost OpenFile] Path is null or Empty"); return; }
            
            SetPath(newpath);
            LoadFile();
        }

        public void LoadFile()
        {
            if (!_path.IsValid()) { Debug.LogError("[TabalogHost LoadFile] Path is null or Empty"); return; }


            Stream file = File.Open(_path, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string text = reader.ReadToEnd();
            _loadedtext = text;
            file.Dispose();
            
            ConvertToData();
        }

        void ConvertToData()
        {
            if (_loadedtext == null) return;
            _data = TabalogUnpacker.Unpack(_loadedtext);
        }

        public void SaveAs()
        {
            string newpath = EditorUtility.SaveFilePanel("Save Tabalog File", _path, $"{_path.Split('/').Last().Split('.')[0]} Copy", "tabalog");
            if (newpath.Length == 0) { return; }
            
            SetPath(newpath);
            Save();
        }

        public void Save(bool ReloadFile = true)
        {
            if (!_path.IsValid())
            {
                Debug.LogError("No path set");
                return;
            }

            Stream file = File.Open(_path, FileMode.Create);
            StreamWriter writer = new StreamWriter(file);
            writer.Write(Pack());
            writer.Flush();
            file.Dispose();

            if (ReloadFile) LoadFile();
        }

        public string Pack()
        {
            string packed = TabalogPacker.Pack(_data);
            return packed;
        }

        public void ResetLoadedData()
        {
            _path = "";
            _loadedtext = null;
            _data = null;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public string GetPath()
        {
            return _path;
        }

        public string GetLoadedText()
        {
            return _loadedtext;
        }

        public void SetLoadedText(string text)
        {
            _loadedtext = text;
        }

        public Dictionary<string, string> GetData()
        {
            return _data;
        }

        public bool Exists(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Remove(string key)
        {
            // Remove all children
            GetSub(key).ToList().ForEach(k => _data.Remove(k.Key));

            _data.Remove(key);
        }

        public Dictionary<string, string> GetSub(string key)
        {
            return _data.Where(x => x.Key.StartsWith(key + "/")).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}