#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace Tools
{
    public class BaseGenerator
    {
        protected enum Type
        {
            Json,
            Struct,
            Container,
        }

        protected int DataTypeIndex => 1;
        protected int NameIndex => DataTypeIndex + 1;
        protected int ValueIndex => NameIndex + 1;
        protected Type GeneratorType { get; set; }
        protected string FilePath { get; set; }
        protected string FileName => Path.GetFileNameWithoutExtension(FilePath);
        protected string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileNameWithExtension);
        protected string FileNameWithExtension
        {
            get
            {
                switch (GeneratorType)
                {
                    case Type.Json:
                        return $"{FileName}.json";
                    case Type.Struct:
                        return $"Data{FileName}.cs";
                    case Type.Container:
                        return $"DataContainerManager.cs";
                    default:
                        return null;
                }
            }
        }

        protected string SavePath
        {
            get
            {
                string savePath = null;

                switch (GeneratorType)
                {
                    case Type.Json:
                        savePath = PathDefine.Json;
                        break;
                    case Type.Struct:
                        savePath = PathDefine.DataStruct;
                        break;
                    case Type.Container:
                        savePath = PathDefine.Manager;
                        break;
                }

                return Path.Combine(savePath, FileNameWithExtension);
            }
        }

        protected string GetDataTemplate(string path, string type = null, string name = null, string modifier = null)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"{path}에 해당 템플릿이 없습니다.");
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
    }
}
#endif
