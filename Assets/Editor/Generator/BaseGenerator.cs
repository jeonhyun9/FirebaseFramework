#if UNITY_EDITOR
using System.IO;

namespace Tools
{
    public class BaseGenerator
    {
        protected enum Type
        {
            Json,
            Struct,
            ContainerManager,
            JsonList,
            VersionText,
        }

        protected string folderPath;
        private Type generatorType;
        protected int DataTypeIndex => 1;
        protected int NameIndex => DataTypeIndex + 1;
        protected int ValueIndex => NameIndex + 1;
        protected string GetDataTemplate(string path, string type = null, string name = null, string modifier = null)
        {
            if (!File.Exists(path))
            {
                Logger.Warning($"There is no template at {path}");
                return null;
            }

            var template = File.ReadAllText(path);

            if (!string.IsNullOrEmpty(type))
                template = template.Replace("#type#", type);

            if (!string.IsNullOrEmpty(name))
                template = template.Replace("#name#", name);

            if (!string.IsNullOrEmpty(modifier))
                template = template.Replace("#modifier#", modifier);

            return template;
        }
        protected string GetNaming(string name, string dataType = null)
        {
            //Id, NameId => id => nameId Ä«¸á·Î º¯°æ
            if (name.Contains("Id") || name.Contains("NameId"))
                name = char.ToLower(name[0]) + name[1..];

            if (!string.IsNullOrEmpty(dataType) && dataType.Contains("struct:"))
                name += "NameId";

            return name;
        }
        protected void OnEndGenerate(string folderPath, string fileNameWithExtension, string generatedValue)
        {
            string savePath = Path.Combine(folderPath, fileNameWithExtension);

            if (string.IsNullOrEmpty(generatedValue))
            {
                Logger.Null("Generated string value");
                return;
            }

            if (File.Exists(savePath) && File.ReadAllText(savePath) == generatedValue)
            {
                Logger.Log($"{fileNameWithExtension} No changed");
                return;
            }

            Logger.Log($"{fileNameWithExtension} {(File.Exists(savePath) ? "Edited" : "Created")}");
            File.WriteAllText(savePath, generatedValue);
        }
        protected void InitType(Type type)
        {
            generatorType = type;
        }
    }
}
#endif
