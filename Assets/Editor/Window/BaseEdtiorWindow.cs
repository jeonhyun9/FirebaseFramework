#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public abstract class BaseEdtiorWindow : EditorWindow
    {
        private Dictionary<string, object> parameters = new();
        private Dictionary<string, object> defaultParameters = new();
        private float spacing;
        private Type useEnumType;

        /// <summary> Window 사이즈 초기화 .. spacing - 변수 사이 간격 </summary>
        protected void InitializeWindow(BaseEdtiorWindow window, float width, float height, float spacingValue)
        {
            window.minSize = new Vector2(width, height);
            window.maxSize = new Vector2(width * 2, height);
            spacing = spacingValue;
        }

        private void Awake()
        {
            InitializeParameters();
        }

        private void OnEnable()
        {
            SetParameterFromEditorPrefs();
        }

        private void OnGUI()
        {
            DrawParameters();
            DrawActionButton();
        }

        private void OnDisable()
        {
            SaveParameters();
        }

        /// <summary> AddParameter로 사용할 변수들을 추가하도록 구현 </summary>
        protected abstract void InitializeParameters();

        protected abstract void DrawActionButton();

        protected void AddParameter(string name, object defaultValue)
        {
            defaultParameters[name] = defaultValue;
        }

        protected void AddEnumType(Type enumType)
        {
            useEnumType = enumType;
        }

        private void SetParameterFromEditorPrefs()
        {
            foreach(string name in defaultParameters.Keys)
            {
                Type type = defaultParameters[name].GetType();

                switch (type)
                {
                    case Type t when t == typeof(string):
                        parameters[name] = EditorPrefs.GetString(name, (string)defaultParameters[name]);
                        break;

                    case Type t when t == typeof(int):
                        parameters[name] = EditorPrefs.GetInt(name, (int)defaultParameters[name]);
                        break;

                    case Type t when t == typeof(bool):
                        parameters[name] = EditorPrefs.GetBool(name, (bool)defaultParameters[name]);
                        break;

                    case Type t when t == useEnumType:
                        string enumString = EditorPrefs.GetString(name, defaultParameters[name].ToString());
                        parameters[name] = Enum.Parse(useEnumType, enumString);
                        break;
                }
            }
        }

        private void SaveParameters()
        {
            foreach (string name in defaultParameters.Keys)
            {
                Type type = defaultParameters[name].GetType();

                switch (type)
                {
                    case Type t when t == typeof(string):
                        EditorPrefs.SetString(name, (string)parameters[name]);
                        break;

                    case Type t when t == typeof(int):
                        EditorPrefs.SetInt(name, (int)parameters[name]);
                        break;

                    case Type t when t == typeof(bool):
                        EditorPrefs.SetBool(name, (bool)parameters[name]);
                        break;

                    case Type t when t == useEnumType:
                        EditorPrefs.SetString(name, parameters[name].ToString());
                        break;
                }
            }
        }

        private void DrawParameters()
        {
            foreach (string name in defaultParameters.Keys)
            {
                Type type = parameters[name].GetType();

                EditorGUILayout.LabelField(GetParameterLabel(name));

                switch (type)
                {
                    case Type t when t == typeof(string):
                        parameters[name] = EditorGUILayout.TextField((string)parameters[name]);
                        break;

                    case Type t when t == typeof(int):
                        parameters[name] = EditorGUILayout.IntField((int)parameters[name]);
                        break;

                    case Type t when t == typeof(bool):
                        parameters[name] = EditorGUILayout.Toggle((bool)parameters[name]);
                        break;

                    case Type t when t == useEnumType:
                        parameters[name] = EditorGUILayout.EnumPopup(useEnumType.Name, (Enum)parameters[name]);
                        break;
                }

                EditorGUILayout.Space(spacing);
            }
        }

        private string GetParameterLabel(string name)
        {
            //파스칼 케이스로 작성된 name 사이 사이에 띄어쓰기 추가
            //ex)LocalDataJson => Local Data Json
            return System.Text.RegularExpressions.Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1 $2");
        }

        /// <summary> name은 AddParameter에서 추가한 것과 동일해야함 </summary>
        public T GetParameter<T>(string name)
        {
            if (!parameters.ContainsKey(name))
                return default;

            return parameters[name] is T ? (T)parameters[name] : default;
        }
    }
}
#endif
