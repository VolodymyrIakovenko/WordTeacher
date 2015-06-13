namespace WordTeacher.Models
{
    public class TranslationItem
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

        public override string ToString()
        {
            return Word + ": " + Translation;
        }
    }
}