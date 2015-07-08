using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordTeacher.Models
{
    public class OptionsTest
    {
        public OptionsTest(string question, List<string> options, string answer)
            : this(question, options.Select(o => new Option(o)).ToList(), answer)
        {
        }

        public OptionsTest(string question, List<Option> options, string answer)
        {
            Question = question;
            Options = options;
            Answer = answer;
        }

        public string Question { get; set; }

        public List<Option> Options { get; set; }

        public string Answer { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Question);
            sb.Append(": ");
            foreach (var option in Options)
            {
                sb.Append("[");
                sb.Append(option);
                sb.Append("]; ");
            }

            return sb.ToString();
        }
    }
}
