using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WordTeacher.Commands;
using WordTeacher.Extensions;
using WordTeacher.Models;

namespace WordTeacher.ViewModels
{
    public class InputTestViewModel: INotifyPropertyChanged, ICloseable
    {
        private const int HighlightingDelayInSec = 2;

        private int _currentTestIndex;
        private int _rightAnswers;
        private string _correctAnswer;
        private string _answerInput;
        private Brush _answerColor;
        private bool _isNextEnabled;
        private List<InputTest> _tests = new List<InputTest>();

        private ICommand _nextCommand;

        public string EndButtonName
        {
            get { return _currentTestIndex == _tests.Count - 1 ? "Finish" : "Next"; }
        }

        public string Question
        {
            get { return Tests.Count > _currentTestIndex ? (_currentTestIndex + 1) + "/" + Tests.Count + ": " + Tests[_currentTestIndex].Question : string.Empty; }
        }

        public List<InputTest> Tests
        {
            get { return _tests; }
            set
            {
                _tests = value;

                OnPropertyChanged("Question");
                OnPropertyChanged("Options");
            }
        }

        public string CorrectAnswer
        {
            get { return _correctAnswer; }
            set
            {
                _correctAnswer = value;
                OnPropertyChanged("CorrectAnswer");
            }
        }

        public string AnswerInput
        {
            get { return _answerInput; }
            set
            {
                _answerInput = value;
                OnPropertyChanged("AnswerInput");
            }
        }

        public Brush AnswerColor
        {
            get { return _answerColor; }
            set
            {
                _answerColor = value;
                OnPropertyChanged("AnswerColor");
            }
        }

        public bool IsNextEnabled
        {
            get { return _isNextEnabled; }
            set
            {
                _isNextEnabled = value;
                OnPropertyChanged("IsNextEnabled");
            }
        }

        public ICommand NextCommand
        {
            get { return _nextCommand ?? (_nextCommand = new CommandHandler(Next, true)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> RequestClose;

        public void Opened()
        {
            _currentTestIndex = 0;
            _rightAnswers = 0;
            ClearTestView();
            UpdateAllProperties();
            IsNextEnabled = true;
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
        
        private void Next()
        {
            if (IsRightAnswer())
            {
                AnswerColor = Brushes.LimeGreen;
                _rightAnswers++;
            }
            else
            {
                AnswerColor = Brushes.Red;
                CorrectAnswer = Tests[_currentTestIndex].Answer;
            }
            OnPropertyChanged("AnswerColor");

            if (_currentTestIndex == _tests.Count - 1)
            {
                MessageBox.Show("Your score: " + _rightAnswers + "/" + Tests.Count + " (" + (_rightAnswers * 100 / Tests.Count) + "%)");

                var handler = RequestClose;
                if (handler != null)
                    handler.Invoke(this, new EventArgs());
            }
            else
            {
                IsNextEnabled = false;
                _currentTestIndex++;
                SynchronizationContext.Current.Post(TimeSpan.FromSeconds(HighlightingDelayInSec), () =>
                    {
                        ClearTestView();
                        UpdateAllProperties();
                        IsNextEnabled = true;
                    }
                );
            }
        }

        private void ClearTestView()
        {
            CorrectAnswer = string.Empty;
            AnswerInput = string.Empty;
            AnswerColor = Brushes.Transparent;
        }

        private bool IsRightAnswer()
        {
            return AnswerInput != null && AnswerInput.Trim().ToLower().Equals(Tests[_currentTestIndex].Answer.Trim().ToLower());
        }

        private void UpdateAllProperties()
        {
            OnPropertyChanged("Question");
            OnPropertyChanged("Options");
            OnPropertyChanged("EndButtonName");
        }
    }
}
