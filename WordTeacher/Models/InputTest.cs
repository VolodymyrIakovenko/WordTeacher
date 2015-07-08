using System.Collections.Generic;

namespace WordTeacher.Models
{
    public class InputTest
    {
        public InputTest(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }

        public string Question { get; set; }

        public string Answer { get; set; }

        public override string ToString()
        {
            return Question + ": " + Answer;
        }
    }
}
