#if UNITY_EDITOR
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Tools
{
    public class StructGenerator : BaseGenerator
    {
        private DataTable sheet;

        public void Init(string pathValue, DataTable sheetValue)
        {
            GeneratorType = Type.Struct;
            FilePath = pathValue;
            sheet = sheetValue;
        }

        public bool Generate(ref StringBuilder log)
        {
            //��Ʈ���� ������ Ÿ�԰� �̸��� �̾Ƴ���
            List<string> columnNames = new();
            List<string> columnTypes = new();

            DataRow dataTypeRow = sheet.Rows[DataTypeIndex];
            DataRow nameRow = sheet.Rows[NameIndex];

            for (int j = 0; j < sheet.Columns.Count; j++)
            {
                string dataType = dataTypeRow[j].ToString();
                string name = nameRow[j].ToString();
                columnTypes.Add(dataType);
                columnNames.Add(name);
            }

            StringBuilder sb = new();

            sb.AppendLine(GetDataTemplate(PathDefine.StartDataTemplate, name: FileNameWithoutExtension));

            for (int i = 0; i < columnNames.Count; i++)
            {
                string type = columnTypes[i].Replace("enum:", "");
                string name = GetNaming(columnNames[i]);
                string modifier = GetAccessModifier(name);

                if (type.Contains("struct:"))
                {
                    type = type.Replace("struct:", "");
                    sb.AppendLine(GetDataTemplate(PathDefine.StructValueTemplate, type, name, modifier));
                }
                else
                {
                    sb.AppendLine(GetDataTemplate(PathDefine.DataValueTemplate, type, name, modifier));
                }
            }

            sb.AppendLine(GetDataTemplate(PathDefine.EndDateTemplate));

            string newStruct = sb.ToString();

            if (File.Exists(SavePath) && File.ReadAllText(SavePath) == newStruct)
                return false;

            log.AppendLine($"{FileNameWithExtension} {(File.Exists(SavePath) ? "����" : "����")} �Ϸ�");
            
            File.WriteAllText(SavePath, newStruct);

            return true;
        }
    }
}
#endif
