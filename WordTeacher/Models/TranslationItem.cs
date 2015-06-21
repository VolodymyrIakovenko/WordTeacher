using System;

namespace WordTeacher.Models
{
    public class TranslationItem : ICloneable, IEquatable<TranslationItem>
    {
        // Always creates uniqe hash code.
        private readonly DateTime _timestamp = DateTime.UtcNow;

        /// <summary>
        /// Without empty constructor datagrid can't be editable. 
        /// </summary>
        public TranslationItem()
        {
            Word = string.Empty;
            Translation = string.Empty;
        }

        public TranslationItem(string word, string translation)
        {
            Word = word;
            Translation = translation;
        }

        public string Word { get; set; }

        public string Translation { get; set; }

        public bool Equals(TranslationItem other)
        {
            return other != null && this.Word.Equals(other.Word) && this.Translation.Equals(other.Translation);
        }

        public override int GetHashCode()
        {
            return _timestamp.GetHashCode();
        }

        public override string ToString()
        {
            return Word + ": " + Translation;
        }

        public object Clone()
        {
            return new TranslationItem(Word, Translation);
        }
    }
}