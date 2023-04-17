#if UNITY_EDITOR
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

        [MenuItem("Tools/Generate Script")]
        public static void OpenScriptGenerateorWindow()
        {
            ScriptGenerateorWindow window = (ScriptGenerateorWindow)GetWindow(typeof(ScriptGenerateorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void DrawActionButton()
        {
            //throw new System.NotImplementedException();
        }

        //¹Ì±¸Çö
        protected override void InitializeParameters()
        {
            AddEnumType(typeof(ScriptGenerator.ScriptType));
            AddParameter("ScriptType", ScriptGenerator.ScriptType.MVC);
        }
    }
}
#endif
