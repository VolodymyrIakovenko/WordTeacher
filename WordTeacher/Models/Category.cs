using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordTeacher.Extensions;

namespace WordTeacher.Models
{
    public class Category : ICloneable, IEquatable<Category>
    {
        // Always creates uniqe hash code.
        private readonly DateTime _timestamp = DateTime.UtcNow;

        /// <summary>
        /// Without empty constructor datagrid can't be editable. 
        /// </summary>
        public Category()
        {
            Name = string.Empty;
            TranslationItems = new List<TranslationItem>();
        }

        public Category(string name, List<TranslationItem> translationItems)
        {
            Name = name;
            TranslationItems = new List<TranslationItem>(translationItems.Clone());
        }

        public string Name { get; set; }

        public List<TranslationItem> TranslationItems { get; set; }

        public bool Equals(Category other)
        {
            return Name.Equals(other.Name) && TranslationItems.SequenceEqual(other.TranslationItems);
        }

        public override int GetHashCode()
        {
            return _timestamp.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(": ");
            foreach (var translationItem in TranslationItems)
            {
                sb.Append("[");
                sb.Append(translationItem);
                sb.Append("]; ");
            }

            return sb.ToString();
        }

        public object Clone()
        {
            return new Category(Name, TranslationItems);
        }
    }
}
