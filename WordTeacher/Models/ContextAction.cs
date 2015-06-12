using System.Windows.Input;

namespace WordTeacher.Models
{
    public class ContextAction
    {
        public ContextAction(string name, ICommand action)
        {
            Name = name;
            Action = action;
        }

        public string Name { get; set; }

        public ICommand Action { get; set; }
    }
}
