using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

public class StructGenerator : BaseGenerator
{
    private DataTable sheet;

    public StructGenerator(string pathValue, DataTable sheetValue)
    {
        GeneratorType = Type.Struct;
        FilePath = pathValue;
        sheet = sheetValue;
    }

    public bool Generate(ref StringBuilder log)
    {
        //시트에서 데이터 타입과 이름만 뽑아놓기
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
            string name = columnNames[i];

            if (type.Contains("struct:"))
            {
                type = type.Replace("struct:", "");
                sb.AppendLine(GetDataTemplate(PathDefine.StructValueTemplate, type, name));
            }
            else
            {
                sb.AppendLine(GetDataTemplate(PathDefine.DataValueTemplate, type, name));
            }
        }

        sb.AppendLine(GetDataTemplate(PathDefine.EndDateTemplate));

        string newStruct = sb.ToString();

        bool changed = false;

        if (File.Exists(SavePath))
        {
            if (File.ReadAllText(SavePath).Equals(newStruct))
            {
                return false;
            }
            else
            {
                changed = true;
            }
        }

        File.WriteAllText(SavePath, newStruct);

        log.AppendLine($"{FileNameWithExtension} {(changed ? "수정" : "생성")} 완료");

        return true;
    }
}
