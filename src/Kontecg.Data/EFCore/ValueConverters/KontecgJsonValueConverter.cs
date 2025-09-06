using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontecg.EFCore.ValueConverters
{
    public class KontecgJsonValueConverter<TModel> : ValueConverter<TModel, string>
    {
        public static Func<JsonSerializerSettings> Create = () =>
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy(false, true) },
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.None,
            };
            // ignore error
            settings.Error += JsonErrorHandle;
            return settings;
        };

        public KontecgJsonValueConverter()
            : base(s => Serialize(s), x => Deserialize<TModel>(x))
        {
        }

        private static void JsonErrorHandle(object sender, ErrorEventArgs e)
        {
            // ignore error.
            e.ErrorContext.Handled = true;
        }

        private static string Serialize<T>(T value)
        {
            var settings = Create();
            return JsonConvert.SerializeObject(value, Formatting.None, settings);
        }

        private static T Deserialize<T>(string json)
        {
            var settings = Create();
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
