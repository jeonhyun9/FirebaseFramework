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

        private Type generatorType;
        private string folderPath;

        protected int DataTypeIndex => 1;
        protected int NameIndex => DataTypeIndex + 1;
        protected int ValueIndex => NameIndex + 1;
        protected string FileNameWithoutExtension { get; private set; }
        protected string FileNameWithExtension
        {
            get
            {
                switch (generatorType)
                {
                    case Type.Json:
                        return $"{FileNameWithoutExtension}.json";
                    case Type.Struct:
                        return $"{FileNameWithoutExtension}.cs";

                    //이름이 고정된 것들
                    case Type.ContainerManager:
                        return Path.GetFileName(PathDefine.DataContainerManager);
                    case Type.JsonList:
                        return Path.GetFileName(PathDefine.JsonListText);
                    case Type.VersionText:
                        return Path.GetFileName(PathDefine.VersionText);
                    default:
                        return null;
                }
            }
        }
        protected string SavePath
        {
            get
            {
                switch (generatorType)
                {
                    case Type.Json:
                    case Type.VersionText:
                    case Type.Struct:
                    case Type.JsonList:
                        return Path.Combine(folderPath, FileNameWithExtension);
                    case Type.ContainerManager:
                        return PathDefine.DataContainerManager;
                    default:
                        return null;
                }
            }
        }
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
            //Id, NameId => id => nameId 카멜로 변경
            if (name.Contains("Id") || name.Contains("NameId"))
                name = char.ToLower(name[0]) + name[1..];

            if (!string.IsNullOrEmpty(dataType) && dataType.Contains("struct:"))
                name += "NameId";

            return name;
        }
        protected string GetAccessModifier(string name)
        {
            if (name.Contains("Id") || name.Contains("NameId") || name.Contains("id") || name.Contains("nameid"))
                return "private";

            return "public";
        }
        protected bool CheckExist(string savePath, string contents, bool isJson)
        {
            if (isJson)
                return File.Exists(savePath) && Newtonsoft.Json.Linq.JToken.DeepEquals(contents, File.ReadAllText(savePath));

            return File.Exists(savePath) && File.ReadAllText(savePath) == contents;
        }
        protected void OnEndGenerate(string savePath, string generatedValue, bool isJson = false)
        {
            if (CheckExist(savePath, generatedValue, isJson))
            {
                Logger.Log($"{FileNameWithExtension} No changed");
                return;
            }

            Logger.Log($"{FileNameWithExtension} {(File.Exists(savePath) ? "Edited" : "Created")}");
            File.WriteAllText(SavePath, generatedValue);
        }
        protected void SetFolderPath(string path)
        {
            folderPath = path;
        }
        protected void SetFileNameWithoutExtension(string path)
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        }
        protected void InitType(Type type)
        {
            generatorType = type;
        }
    }
}
#endif
