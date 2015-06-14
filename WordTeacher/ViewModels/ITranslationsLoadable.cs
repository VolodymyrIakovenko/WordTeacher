using System.Collections.Generic;

using WordTeacher.Models;

namespace WordTeacher.ViewModels
{
    public interface ITranslationsLoadable
    {
        void ReloadSettings(IList<TranslationItem> translationItems);
    }
}
