#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class ScriptGenerateorEditorWindow : BaseEdtiorWindow
    {
        private const float width = 400f;
        private const float height = 200f;
        private const float spacing = 5f;
        private ScriptGenerator scriptGenerator = null;

        private ScriptGenerator.ScriptType ScriptType => GetParameter<ScriptGenerator.ScriptType>("ScriptType");
        private ScriptGenerator.ManagerType ManagerType => GetParameter<ScriptGenerator.ManagerType>("ManagerType");
        private string ContentsName => GetParameter<string>("ContentsName");

        [MenuItem("Tools/Generate Script")]
        public static void OpenScriptGenerateorWindow()
        {
            ScriptGenerateorEditorWindow window = (ScriptGenerateorEditorWindow)GetWindow(typeof(ScriptGenerateorEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
            {
                if (string.IsNullOrEmpty(ContentsName))
                {
                    Logger.Error("������ �̸��� �Է����ּ���");
                    return;
                }

                if (scriptGenerator == null)
                {
                    scriptGenerator = new();

                    switch (ScriptType)
                    {
                        case ScriptGenerator.ScriptType.Manager:
                            scriptGenerator.GenerateManager(ManagerType, ContentsName);
                            break;

                        default:
                            scriptGenerator.Generate(ScriptType, ContentsName);
                            break;
                    }

                    Close();
                }
            }
        }

        protected override void InitializeParameters()
        {
            //���������� �߸� �Է��ϸ� ��ũ��Ʈ ���� ���� �־ �ʱ�ȭ
            EditorPrefs.SetString("ContentsName", "");

            AddParameter("ScriptType", ScriptGenerator.ScriptType.MVC, typeof(ScriptGenerator.ScriptType));

            AddConditionalParameter("ManagerType",
                ScriptGenerator.ManagerType.Base,
                "ScriptType",
                ScriptGenerator.ScriptType.Manager,
                typeof(ScriptGenerator.ManagerType));

            AddLabel("[��ũ��Ʈ �̸� ����] - {ContentsName}{ScriptType}.cs");

            AddParameter("ContentsName", "");
        }
    }
}
#endif
