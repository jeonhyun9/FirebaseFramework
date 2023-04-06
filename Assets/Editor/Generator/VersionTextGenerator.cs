#if UNITY_EDITOR

using System.IO;

namespace Tools
{
    public class VersionTextGenerator : BaseGenerator
    {
        private string VersionTextName => Path.GetFileName(PathDefine.VersionText);
        public VersionTextGenerator()
        {
            InitType(Type.VersionText);
        }

        public void Generate(string jsonFolderPath, string version)
        {
            OnEndGenerate(jsonFolderPath, VersionTextName, version);
        }
    }
}
#endif
