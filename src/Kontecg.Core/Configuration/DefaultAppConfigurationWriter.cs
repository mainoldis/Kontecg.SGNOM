using System.IO;
using System.Linq;
using Castle.Core.Logging;
using Kontecg.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kontecg.Configuration
{
    public class DefaultAppConfigurationWriter : IAppConfigurationWriter
    {
        public DefaultAppConfigurationWriter()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Write(string key, string value)
        {
            if (!File.Exists("config.json"))
            {
                throw new UserFriendlyException("config.json file does not exist");
            }

            if (File.Exists($"config.json"))
            {
                WriteInternal($"config.json", key, value);
            }
        }

        protected virtual void WriteInternal(string filename, string key, string value)
        {
            Check.NotNullOrWhiteSpace(key, nameof(key));
            Check.NotNull(value, nameof(value));

            var jsonFile = JObject.Parse(File.ReadAllText(filename));

            var objectNames = key.Split(':').ToList();
            if (objectNames.Count == 1)
            {
                objectNames.Clear();
            }
            else
            {
                key = objectNames.Last();
                objectNames.RemoveAt(objectNames.Count - 1);
            }

            var jobj = jsonFile;

            foreach (var objectName in objectNames)
            {
                jobj = (JObject) jobj[objectName];
                if (jobj == null)
                {
                    Logger.Error($"Key {key} does not exist!");
                    return;
                }
            }

            var jProperty = jobj.Property(key);
            if (jProperty == null)
                jobj.Add(key, value);
            else
                jProperty.Value.Replace(value);

            using (var file = File.CreateText(filename))
            {
                using (var writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    jsonFile.WriteTo(writer);
                }
            }
        }
    }
}
