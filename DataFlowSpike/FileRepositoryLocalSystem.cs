using System.Configuration;
using System.IO;
using System.Linq;

namespace DataFlowSpike
{
    public class FileRepositoryLocalSystem : IFileRepository
    {
        public string GetFile()
        {
            var path = ConfigurationManager.AppSettings["FilePath"];

            if (string.IsNullOrEmpty(path))
                throw new System.Exception("Directory missing from [FilePath] in app.config file.");

            var files = Directory.GetFiles(path);

            var file = files.First();
            var text = File.ReadAllText(file);

            return text;
        }
    }
}
