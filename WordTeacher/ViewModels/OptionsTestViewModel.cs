using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WordTeacher.Commands;
using WordTeacher.Extensions;
using WordTeacher.Models;

namespace WordTeacher.ViewModels
{
    public class OptionsTestViewModel: INotifyPropertyChanged, ICloseable
    {
        private const int HighlightingDelayInSec = 2;

        private int _currentTestIndex;
        private int _rightAnswers;
        private List<OptionsTest> _tests = new List<OptionsTest>();

        private ICommand _nextCommand;

        public string EndButtonName
        {
            get { return _currentTestIndex == _tests.Count - 1 ? "Finish" : "Next"; }
        }

        public bool IsNextEnabled
        {
            get { return Tests.Count > _currentTestIndex && Tests[_currentTestIndex].Options.Any(o => o.IsChecked); }
        }

        public string Question
        {
            get { return Tests.Count > _currentTestIndex ? (_currentTestIndex + 1) + "/" + Tests.Count + ": " + Tests[_currentTestIndex].Question : string.Empty; }
        }

        public List<Option> Options
        {
            get { return Tests.Count > _currentTestIndex ? Tests[_currentTestIndex].Options : new List<Option>(); }
        }

        public List<OptionsTest> Tests
        {
            get { return _tests; }
            set
            {
                _tests = value;

                foreach (var optionsTest in _tests)
                {
                    foreach (var option in optionsTest.Options)
                    {
                        option.IsCheckedChanged += (sender, args) => OnPropertyChanged("IsNextEnabled");
                    }
                }

                OnPropertyChanged("Question");
                OnPropertyChanged("Options");
            }
        }

        public ICommand NextCommand
        {
            get { return _nextCommand ?? (_nextCommand = new CommandHandler(Next, true)); }
        }

        private Option ChosenOption
        {
            get { return Options.FirstOrDefault(o => o.IsChecked); }
        }

        private Option AnswerOption
        {
            get {
                return Tests.Count > _currentTestIndex 
                    ? Tests[_currentTestIndex].Options.FirstOrDefault(o => o.Name.Equals(Tests[_currentTestIndex].Answer))
                    : null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> RequestClose;

        public void Opened()
        {
            _currentTestIndex = 0;
            _rightAnswers = 0;
            UpdateAllProperties();
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        
        private void Next()
        {
            var chosenOption = ChosenOption;
            var answerOption = AnswerOption;

            if (IsRightAnswer())
            {
                _rightAnswers++;

                if (chosenOption != null)
                {
                    chosenOption.Color = Brushes.LimeGreen;
                }
            }
            else
            {
                if (chosenOption != null)
                {
                    chosenOption.Color = Brushes.Red;
                }

                if (answerOption != null)
                {
                    answerOption.Color = Brushes.LimeGreen;
                }
            }
            OnPropertyChanged("Options");

            if (_currentTestIndex == _tests.Count - 1)
            {
                MessageBox.Show("Your score: " + _rightAnswers + "/" + Tests.Count + " (" + (_rightAnswers * 100 / Tests.Count) + "%)");

                var handler = RequestClose;
                if (handler != null)
                    handler.Invoke(this, new EventArgs());
            }
            else
            {
                _currentTestIndex++;
                OnPropertyChanged("IsNextEnabled");
                SynchronizationContext.Current.Post(TimeSpan.FromSeconds(HighlightingDelayInSec), UpdateAllProperties);
            }
        }

        private bool IsRightAnswer()
        {
            var chosenOption = ChosenOption;
            if (chosenOption == null)
                return false;

            return chosenOption.Name.Equals(Tests[_currentTestIndex].Answer);
        }

        private void UpdateAllProperties()
        {
            OnPropertyChanged("Question");
            OnPropertyChanged("Options");
            OnPropertyChanged("IsNextEnabled");
            OnPropertyChanged("EndButtonName");
        }
    }
}
