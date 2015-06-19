using System;

namespace WordTeacher.Models
{
    public class TranslationItem : ICloneable, IEquatable<TranslationItem>
    {
        /// <summary>
        /// Without empty constructor datagrid can't be editable. 
        /// </summary>
        public TranslationItem() { }

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
            return this.Word == null ? 0 : this.Word.GetHashCode();
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