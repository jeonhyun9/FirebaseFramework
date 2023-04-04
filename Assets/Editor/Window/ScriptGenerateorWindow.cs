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
        public static void OpenDataUploaderWindow()
        {
            ScriptGenerateorWindow window = (ScriptGenerateorWindow)GetWindow(typeof(ScriptGenerateorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void InitializeParameters()
        {
            throw new System.NotImplementedException();
        }

        protected override void DrawActionButton()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif
