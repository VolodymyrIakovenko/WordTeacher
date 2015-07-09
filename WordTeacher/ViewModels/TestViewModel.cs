using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using WordTeacher.Commands;
using WordTeacher.Extensions;
using WordTeacher.Models;
using WordTeacher.Utilities;
using WordTeacher.Views;

namespace WordTeacher.ViewModels
{
    public class TestViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<string> _availableCategories = new ObservableCollection<string>();
        private ObservableCollection<string> _chosenCategories = new ObservableCollection<string>();

        private ICommand _moveRightCommand;
        private ICommand _moveLeftCommand;
        private ICommand _moveAllRightCommand;
        private ICommand _moveAllLeftCommand;
        private ICommand _startCommand;

        public ObservableCollection<string> AvailableCategories
        {
            get { return _availableCategories; }
            set
            {
                _availableCategories = value;
                OnPropertyChanged("AvailableCategories");
            }
        }

        public ObservableCollection<string> ChosenCategories
        {
            get { return _chosenCategories; }
            set
            {
                _chosenCategories = value;

                OnPropertyChanged("ChosenCategories");
            }
        }

        public string SelectedAvailable { get; set; }

        public string SelectedChosen { get; set; }

        public bool IsOptionsSelected { get; set; }

        public bool IsInputSelected { get; set; }

        public bool IsWordSelected { get; set; }

        public bool IsTranslationSelected { get; set; }

        public bool IsStartEnabled
        {
            get { return ChosenCategories.Count > 0; }
        }

        public ICommand MoveRightCommand
        {
            get { return _moveRightCommand ?? (_moveRightCommand = new CommandHandler(MoveRight, true)); }
        }

        public ICommand MoveLeftCommand
        {
            get { return _moveLeftCommand ?? (_moveLeftCommand = new CommandHandler(MoveLeft, true)); }
        }

        public ICommand MoveAllRightCommand
        {
            get { return _moveAllRightCommand ?? (_moveAllRightCommand = new CommandHandler(MoveAllRight, true)); }
        }

        public ICommand MoveAllLeftCommand
        {
            get { return _moveAllLeftCommand ?? (_moveAllLeftCommand = new CommandHandler(MoveAllLeft, true)); }
        }

        public ICommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new CommandHandler(Start, true)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Opened()
        {
            AvailableCategories = new ObservableCollection<string>(SettingFilesUtility.GetAllXmlFiles());
            ChosenCategories.Clear();

            IsOptionsSelected = true;
            OnPropertyChanged("IsOptionsSelected");

            IsWordSelected = true;
            OnPropertyChanged("IsWordSelected");
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        private void MoveRight()
        {
            if (SelectedAvailable == null)
                return;

            var categoryToMove = SelectedAvailable;
            AvailableCategories.Remove(categoryToMove);
            ChosenCategories.Add(categoryToMove);
            OnPropertyChanged("IsStartEnabled");
        }

        private void MoveLeft()
        {
            if (SelectedChosen == null)
                return;

            var categoryToMove = SelectedChosen;
            ChosenCategories.Remove(categoryToMove);
            AvailableCategories.Add(categoryToMove);
            OnPropertyChanged("IsStartEnabled");
        }

        private void MoveAllRight()
        {
            var categoriesToMove = AvailableCategories.Clone();
            AvailableCategories.Clear();
            foreach (var category in categoriesToMove)
            {
                ChosenCategories.Add(category);
            }
            OnPropertyChanged("IsStartEnabled");
        }

        private void MoveAllLeft()
        {
            var categoriesToMove = ChosenCategories.Clone();
            ChosenCategories.Clear();
            foreach (var category in categoriesToMove)
            {
                AvailableCategories.Add(category);
            }
            OnPropertyChanged("IsStartEnabled");
        }

        private void Start()
        {
            // Load all chosen categories.
            var categories = ChosenCategories.Select(SettingFilesUtility.Load).ToList();
            if (!categories.Any(c => c.TranslationItems.Count > 0))
            {
                var message = categories.Count == 1 ? "The chosen category is empty!" : "The chose categories are empty!";
                MessageBox.Show(message);
                return;
            }

            // If options test was chosen.
            if (IsOptionsSelected)
            {
                const int optionsCount = 4;
                var optionTests = new List<OptionsTest>();
                var rand = new Random();
                var randAnswer = new Random();

                // Walk through all chosen categories.
                foreach (var category in categories)
                {
                    var allOptions = new List<string>();
                    if (IsWordSelected)
                        allOptions = category.TranslationItems.Select(t => t.Translation).ToList();
                    else if (IsTranslationSelected)
                        allOptions = category.TranslationItems.Select(t => t.Word).ToList();

                    foreach (var translationItem in category.TranslationItems)
                    {
                        // Generate answer options.
                        var options = new List<string>();

                        var question = string.Empty;
                        var answer = string.Empty;
                        if (IsWordSelected)
                        {
                            question = translationItem.Word;
                            answer = translationItem.Translation;
                        }
                        else if (IsTranslationSelected)
                        {
                            question = translationItem.Translation;
                            answer = translationItem.Word;
                        }

                        for (var i = 0; i < optionsCount - 1; i++)
                        {
                            options.Add(GetRandomOption(rand, allOptions,
                                (options.Concat(new List<string> {answer})).ToList()));
                        }
                        var answerIndex = randAnswer.Next(0, optionsCount);
                        options.Insert(answerIndex, answer);

                        // Create a test.
                        var optionTest = new OptionsTest(question, options, answer);
                        optionTests.Add(optionTest);
                    }
                }

                var optionsTestView = new OptionsTestView();
                var optionsTestViewModel = (OptionsTestViewModel)optionsTestView.DataContext;
                optionTests.Shuffle();
                optionsTestViewModel.Tests = optionTests;
                optionsTestView.ShowDialog();
            }

            // If input test was chosen.
            else if (IsInputSelected)
            {
                var inputTests = new List<InputTest>();

                // Walk through all chosen categories.
                foreach (var category in categories)
                {
                    foreach (var translationItem in category.TranslationItems)
                    {
                        var question = string.Empty;
                        var answer = string.Empty;
                        if (IsWordSelected)
                        {
                            question = translationItem.Word;
                            answer = translationItem.Translation;
                        }
                        else if (IsTranslationSelected)
                        {
                            question = translationItem.Translation;
                            answer = translationItem.Word;
                        }

                        // Create a test.
                        var inputTest = new InputTest(question, answer);
                        inputTests.Add(inputTest);
                    }
                }

                var inputTestView = new InputTestView();
                var inputTestViewModel = (InputTestViewModel)inputTestView.DataContext;
                inputTests.Shuffle();
                inputTestViewModel.Tests = inputTests;
                inputTestView.ShowDialog();
            }
        }

        private string GetRandomOption(Random rand, List<string> list, List<string> exclude)
        {
            var newList = list.Where(c => !exclude.Contains(c)).ToList();
            var index = rand.Next(0, newList.Count - exclude.Count);
            return newList.ElementAt(index);
        }
    }
}
