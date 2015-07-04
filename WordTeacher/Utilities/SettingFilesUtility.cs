using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WordTeacher.Models;
using WordTeacher.Properties;

namespace WordTeacher.Utilities
{
    public static class SettingFilesUtility
    {
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

        public static void Save(Category category)
        {
            var serializer = new XmlSerializer(typeof(List<TranslationItem>));
            var writer = new StreamWriter(Path.Combine(SettingsFolderPath, category.Name));
            serializer.Serialize(writer, category.TranslationItems);
            writer.Close();
        }

        public static Category Load(string file)
        {
            var serializer = new XmlSerializer(typeof(List<TranslationItem>));
            var path = Path.Combine(SettingsFolderPath, file);
            if (!File.Exists(path))
                return new Category(file, new List<TranslationItem>());

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    return new Category(file, (List<TranslationItem>)serializer.Deserialize(fileStream));
                }
                catch (InvalidOperationException)
                {
                    return new Category(file, new List<TranslationItem>());
                }
            }
        }

        public static void Delete(string categoryName)
        {
            var path = Path.Combine(SettingsFolderPath, categoryName);
            if (File.Exists(path))
                File.Delete(path);
        }

        public static string GetCurrentFile()
        {
            if (!Settings.Default.CurrentCategory.Equals(string.Empty)) 
                return Settings.Default.CurrentCategory;

            var files = GetAllXmlFiles();
            string currentFile;
            if (files.Count == 0)
            {
                const string newFile = "new.xml";
                Save(new Category(newFile, new List<TranslationItem>()));
                currentFile = newFile;
            }
            else
            {
                currentFile = files[0];
            }

            Settings.Default.CurrentCategory = currentFile;
            Settings.Default.Save();

            return Settings.Default.CurrentCategory;
        }

        public static List<string> GetAllXmlFiles()
        {
            var directoryInfo = new DirectoryInfo(SettingsFolderPath);
            var fileInfos = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
            return fileInfos.Select(file => file.Name).ToList();
        }
    }
}
