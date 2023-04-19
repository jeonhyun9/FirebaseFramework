#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class ScriptGenerateorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 200f;
        private const float spacing = 5f;
        private ScriptGenerator scriptGenerator = null;

        private ScriptGenerator.ScriptType ScriptType => GetParameter<ScriptGenerator.ScriptType>("ScriptType");
        private string ContentsName => GetParameter<string>("ContentsName");

        [MenuItem("Tools/Generate Script")]
        public static void OpenScriptGenerateorWindow()
        {
            ScriptGenerateorWindow window = (ScriptGenerateorWindow)GetWindow(typeof(ScriptGenerateorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("ScriptGenerator", GUILayout.Height(50)))
            {
                if (scriptGenerator == null)
                {
                    scriptGenerator = new();
                    scriptGenerator.Generate(ScriptType, ContentsName);
                }
            }
        }

        protected override void InitializeParameters()
        {
            AddParameter("ScriptType", ScriptGenerator.ScriptType.MVC, typeof(ScriptGenerator.ScriptType));
            AddParameter("ContentsName", "Default");
        }
    }
}
#endif
