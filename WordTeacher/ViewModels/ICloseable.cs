using System;

namespace WordTeacher.ViewModels
{
    public interface ICloseable
    {
        event EventHandler<EventArgs> RequestClose;
    }
}
