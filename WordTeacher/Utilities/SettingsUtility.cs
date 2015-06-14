using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using WordTeacher.Models;

namespace WordTeacher.Utilities
{
    public static class SettingsUtility
    {
        public static string CurrentFile = "sv-en.xml";

        public static string SettingsFolderPath
        {
            get
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(folder, "WordTeacher");
            }
        }

        public static void CheckSettingsFolder()
        {
            if (!Directory.Exists(SettingsFolderPath))
                Directory.CreateDirectory(SettingsFolderPath);
        }

        public static void Save(List<TranslationItem> translationItems)
        {
            var serializer = new XmlSerializer(typeof(List<TranslationItem>));
            var writer = new StreamWriter(Path.Combine(SettingsFolderPath, CurrentFile));
            serializer.Serialize(writer, translationItems);
            writer.Close();
        }

        public static IList<TranslationItem> Load()
        {
            var serializer = new XmlSerializer(typeof(List<TranslationItem>));
            var path = Path.Combine(SettingsFolderPath, CurrentFile);
            if (!File.Exists(path))
                return new List<TranslationItem>();

            var fileStream = new FileStream(path, FileMode.Open);
            return (List<TranslationItem>)serializer.Deserialize(fileStream);
        }
    }
}
