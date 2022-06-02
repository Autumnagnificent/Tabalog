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

        void Awake()
        {
            ConvertToData();
        }

        public string this[string key]
        {
            get
            {
                if (_data == null)
                    return null;
                    
                if (_data.ContainsKey(key))
                    return _data[key];
                else
                    return null;
            }
        }

        public void OpenFile()
        {
            string newpath = EditorUtility.OpenFilePanel("Open Tabalog File", Application.dataPath, "tabalog");
            if (newpath.Length == 0) { return; }
            
            SetPath(newpath);
            LoadFile();
        }

        public void LoadFile()
        {
            Stream file = File.Open(_path, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string text = reader.ReadToEnd();
            _loadedtext = text;
            file.Dispose();
        }

        public void ConvertToData()
        {
            if (_loadedtext == null) return;
            _data = Unpacker.Unpack(_loadedtext);
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

        public Dictionary<string, string> GetData()
        {
            return _data;
        }
    }
}