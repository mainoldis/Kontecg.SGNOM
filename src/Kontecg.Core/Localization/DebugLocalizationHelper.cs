#if DEBUG
using Castle.Components.DictionaryAdapter.Xml;
using Kontecg.Collections.Extensions;
using Kontecg.Configuration.Startup;
using Kontecg.Extensions;
using Kontecg.Localization.Dictionaries;
using Kontecg.Xml.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Kontecg.Localization
{
    public static class DebugLocalizationHelper
    {
        public static string ReturnGivenNameAndSaveToXml(
            ILocalizationConfiguration configuration,
            string sourceName,
            string name,
            CultureInfo culture)
        {
            string exceptionMessage = $"Can not find '{name}' in localization source '{sourceName}'!";

            if (!name.IsNullOrWhiteSpace())
                WriteToXml(sourceName, name, culture);

            string notFoundText = configuration.HumanizeTextIfNotFound
                ? name.ToSentenceCase(culture)
                : name;

            return configuration.WrapGivenTextIfNotFound
                ? $"[{notFoundText}]"
                : notFoundText;
        }

        public static List<string> ReturnGivenNamesAndSaveToXml(
            ILocalizationConfiguration configuration,
            string sourceName,
            List<string> names,
            CultureInfo culture)
        {
            string exceptionMessage =
                $"Can not find '{string.Join(",", names)}' in localization source '{sourceName}' with culture '{culture.Name}'!";

            if (!names.IsNullOrEmpty())
                WriteToXml(sourceName, names, culture);

            List<string> notFoundText = configuration.HumanizeTextIfNotFound
                ? names.Select(name => name.ToSentenceCase(culture)).ToList()
                : names;

            return configuration.WrapGivenTextIfNotFound
                ? notFoundText.Select(text => $"[{text}]").ToList()
                : notFoundText;
        }

        private static void WriteToXml(string sourceName, string name, CultureInfo culture)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string xmlString = "";
            string filePath = culture.Name != "en" ? $"{sourceName}-{culture.Name}.xml" : $"{sourceName}.xml";

            try
            {
                xmlString = File.ReadAllText(filePath);
            }
            catch( Exception e ) 
            {
                xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><localizationDictionary culture=\"" + culture.Name + "\"><texts></texts></localizationDictionary>";
            }
            
            xmlDocument.LoadXml(xmlString);

            XmlNodeList localizationDictionaryNode = xmlDocument.SelectNodes("/localizationDictionary");
            if (localizationDictionaryNode == null || localizationDictionaryNode.Count <= 0)
            {
                throw new KontecgException("A Localization Xml must include localizationDictionary as root node.");
            }

            string cultureName = localizationDictionaryNode[0].GetAttributeValueOrNull("culture");
            if (string.IsNullOrEmpty(cultureName))
            {
                throw new KontecgException("culture is not defined in language XML file!");
            }

            GenericDictionary dictionary = new GenericDictionary(CultureInfo.GetCultureInfo(cultureName));

            List<string> dublicateNames = new List<string>();

            XmlNodeList textNodes = xmlDocument.SelectNodes("/localizationDictionary/texts/text");
            if (textNodes != null)
            {
                var tagTextsNavigator = xmlDocument.CreateNavigator();
                while (tagTextsNavigator.MoveToLastChild() && !tagTextsNavigator.Matches("/localizationDictionary/texts"))
                {
                }

                foreach (XmlNode node in textNodes)
                {
                    string xmlName = node.GetAttributeValueOrNull("name");
                    if (string.IsNullOrEmpty(xmlName))
                    {
                        throw new KontecgException("name attribute of a text is empty in given xml string.");
                    }

                    if (dictionary.Contains(xmlName))
                    {
                        dublicateNames.Add(xmlName);
                    }

                    dictionary[xmlName] = (node.GetAttributeValueOrNull("value") ?? node.InnerText).NormalizeLineEndings();
                }

                if (!dictionary.Contains(name))
                {
                    XmlElement newChild = xmlDocument.CreateElement("text");
                    newChild.SetAttribute("name", name);
                    newChild.InnerText = $"{name}";
                    tagTextsNavigator.AppendChild(newChild.CreateNavigator());
                }
            }

            if (dublicateNames.Count > 0)
            {
                throw new KontecgException(
                    "A dictionary can not contain same key twice. There are some duplicated names: " +
                    dublicateNames.JoinAsString(", "));
            }
            
            var file = File.CreateText(filePath);
            xmlDocument.Save(file);
            file.Flush();
            file.Close();
        }

        private static void WriteToXml(string sourceName, List<string> names, CultureInfo culture)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string xmlString = "";
            string filePath = culture.Name != "en" ? $"{sourceName}-{culture.Name}.xml" : $"{sourceName}.xml";

            try
            {
                xmlString = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                xmlString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><localizationDictionary culture=\"" + culture.Name + "\"><texts></texts></localizationDictionary>";
            }

            xmlDocument.LoadXml(xmlString);

            XmlNodeList localizationDictionaryNode = xmlDocument.SelectNodes("/localizationDictionary");
            if (localizationDictionaryNode == null || localizationDictionaryNode.Count <= 0)
            {
                throw new KontecgException("A Localization Xml must include localizationDictionary as root node.");
            }

            string cultureName = localizationDictionaryNode[0].GetAttributeValueOrNull("culture");
            if (string.IsNullOrEmpty(cultureName))
            {
                throw new KontecgException("culture is not defined in language XML file!");
            }

            GenericDictionary dictionary = new GenericDictionary(CultureInfo.GetCultureInfo(cultureName));

            List<string> dublicateNames = new List<string>();

            XmlNodeList textNodes = xmlDocument.SelectNodes("/localizationDictionary/texts/text");
            if (textNodes != null)
            {
                var tagTextsNavigator = xmlDocument.CreateNavigator();
                while (tagTextsNavigator.MoveToLastChild() && !tagTextsNavigator.Matches("/localizationDictionary/texts"))
                {
                }

                foreach (XmlNode node in textNodes)
                {
                    string xmlName = node.GetAttributeValueOrNull("name");
                    if (string.IsNullOrEmpty(xmlName))
                    {
                        throw new KontecgException("name attribute of a text is empty in given xml string.");
                    }

                    if (dictionary.Contains(xmlName))
                    {
                        dublicateNames.Add(xmlName);
                    }

                    dictionary[xmlName] = (node.GetAttributeValueOrNull("value") ?? node.InnerText).NormalizeLineEndings();
                }

                foreach(var name in names)
                {
                    if (!dictionary.Contains(name))
                    {
                        XmlElement newChild = xmlDocument.CreateElement("text");
                        newChild.SetAttribute("name", name);
                        newChild.InnerText = $"{name}";
                        tagTextsNavigator.AppendChild(newChild.CreateNavigator());
                    }
                }
            }

            if (dublicateNames.Count > 0)
            {
                throw new KontecgException(
                    "A dictionary can not contain same key twice. There are some duplicated names: " +
                    dublicateNames.JoinAsString(", "));
            }

            var file = File.CreateText(filePath);
            xmlDocument.Save(file);
            file.Flush();
            file.Close();
        }
    }

    internal class GenericDictionary : LocalizationDictionary
    {
        public GenericDictionary(CultureInfo cultureInfo)
            : base(cultureInfo)
        {
        }

        public new bool Contains(string name)
        {
            return base.Contains(name);
        }
    }
}
#endif