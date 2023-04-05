#if UNITY_EDITOR

namespace Tools
{
    public class VersionTextGenerator : BaseGenerator
    {
        public VersionTextGenerator()
        {
            InitType(Type.VersionText);
        }

        public void Generate(string jsonFolderPath, string version)
        {
            SetFolderPath(jsonFolderPath);

            OnEndGenerate(SavePath, version);
        }
    }
}
#endif
