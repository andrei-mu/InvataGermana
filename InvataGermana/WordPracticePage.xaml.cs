using InvataGermana.Data;
using InvataGermana.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InvataGermana
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WordsPracticePage : Page
    {
        private List<Word> selectedNouns = new List<Word>();
        private List<Word> selectedWords = new List<Word>();
        private List<Word> autoSuggWords = new List<Word>();

        private Dictionary<int, Tuple<int, int>> nounStats = new Dictionary<int, Tuple<int, int>>();
        private Dictionary<int, Tuple<int, int>> translateStats = new Dictionary<int, Tuple<int, int>>();
        private int nounTries = 0;
        private int nounCorrect = 0;

        private Word ActiveNoun { get; set; }
        private Word ActiveTranslation { get; set; }
        private readonly Random random = new Random();

        public WordsPracticePage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                listViewLessons.ItemsSource = db.lessons.OrderBy(x => x.Title).ToList();

                autoSuggWords = db.words.ToList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Windows.Storage.ApplicationDataContainer container = null;

            // Create a setting in a container
            if (localSettings.Containers.ContainsKey("DeutschPractice"))
            {
                container = localSettings.Containers["DeutschPractice"];
            }
            else
            {
                container = localSettings.CreateContainer("DeutschPractice", Windows.Storage.ApplicationDataCreateDisposition.Always);
            }

            nounStats = LoadStats(container, "derdiedas");

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.Containers["DeutschPractice"];

            SaveStats(container, "derdiedas", nounStats);
        }

        private Dictionary<int, Tuple<int, int>> LoadStats(Windows.Storage.ApplicationDataContainer container, string containerValue)
        {
            object derdiedasObj = null;
            var retDict = new Dictionary<int, Tuple<int, int>>();

            if (false == container.Values.TryGetValue("derdiedas", out derdiedasObj))
                return retDict;

            var parts = derdiedasObj.ToString().Split(';');

            var regex = new Regex(@"(?<a>\d+)=\[(?<b>\d+),(?<c>\d+)\]");
            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                var match = regex.Match(part);
                if (match.Success)
                {
                    var gg = match.Groups;
                    if (gg.Count != 4)
                        continue;

                    retDict[int.Parse(gg[1].Value)] = new Tuple<int, int>(int.Parse(gg[2].Value),
                                                                            int.Parse(gg[3].Value));
                }
            }

            return retDict;

        }

        private void SaveStats(Windows.Storage.ApplicationDataContainer container, string containerValue, Dictionary<int, Tuple<int, int>> stats)
        {
            var sb = new StringBuilder();
            foreach (var key in stats.Keys)
            {
                var val = stats[key];
                var succ = val.Item1;
                var total = val.Item2;

                sb.Append($"{key}=[{succ},{total}];");
            }

            container.Values[containerValue] = sb.ToString();

        }

        private void listViewLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lessons = listViewLessons.SelectedItems.Select(i => i as Lesson).ToList();

            using (var db = new ApplicationDbContext())
            {
                selectedWords = db.words.Where(x => lessons.Any(y => y.ID == x.Lesson.ID)).ToList();
                selectedNouns = selectedWords.Where(x => x.IsNoun).ToList();
            }

            lessonsCount.Text = listViewLessons.SelectedItems.Count.ToString();
            wordsCount.Text = selectedNouns.Count.ToString();

            UpdateSelection();
        }

        private void UpdateSelection()
        {
            UpdateSelectedNoun();
            UpdateSelectedWord();
        }

        private void UpdateSelectedNoun()
        {
            if (selectedNouns.Count == 0)
            {
                currentNoun.Text = string.Empty;
                ActiveNoun = null;
            }
            else
            {
                int idx = random.Next(selectedNouns.Count);
                ActiveNoun = selectedNouns[idx];
                currentNoun.Text = ActiveNoun.German;

                textError.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateSelectedWord()
        {
            if (selectedWords.Count == 0)
            {
                currentWord.Text = string.Empty;
                ActiveTranslation = null;
            }
            else
            {
                int idx = random.Next(selectedWords.Count);
                ActiveTranslation = selectedWords[idx];
                currentWord.Text = ActiveTranslation.German;

                textError.Visibility = Visibility.Collapsed;
            }

            userTranslation.Text = string.Empty;
        }

        private void btnDer_Click(object sender, RoutedEventArgs e)
        {
            DerDieDasOption(Word.Gender.Der);
        }

        private void btnDie_Click(object sender, RoutedEventArgs e)
        {
            DerDieDasOption(Word.Gender.Die);
        }

        private void btnDas_Click(object sender, RoutedEventArgs e)
        {
            DerDieDasOption(Word.Gender.Das);
        }

        private void DerDieDasOption(Word.Gender gender)
        {
            if (ActiveNoun == null)
            {
                textError.Visibility = Visibility.Visible;
                return;
            }

            nounTries++;
            int trySucc = 0,
                tryTot = 1;

            if (gender == ActiveNoun.Gen)
            {
                nounCorrect++;
                textSuccess.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                textSuccess.Text = $"Correct: {ActiveNoun.SingularCaption}";

                trySucc++;

                Tuple<int, int> tries;
                if (nounStats.TryGetValue(ActiveNoun.ID, out tries))
                {
                    trySucc += tries.Item1;
                    tryTot += tries.Item2;
                }
            }
            else
            {
                textSuccess.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                textSuccess.Text = $"Error: {ActiveNoun.SingularCaption}";

                Tuple<int, int> tries;
                if (nounStats.TryGetValue(ActiveNoun.ID, out tries))
                {
                    trySucc += tries.Item1;
                    tryTot += tries.Item2;
                }
            }

            nounStats[ActiveNoun.ID] = new Tuple<int, int>(trySucc, tryTot);
            textNounStats.Text = $"{ActiveNoun.SingularCaption}: {trySucc} correct from {tryTot} tries";
            textSessionStats.Text = $"{nounCorrect} correct from {nounTries} tries";

            UpdateSelectedNoun();
        }

        private void btnClearResults_Click(object sender, RoutedEventArgs e)
        {
            nounTries = 0;
            nounCorrect = 0;
            nounStats.Clear();
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveTranslation == null || string.IsNullOrEmpty(userTranslation.Text))
            {
                UpdateSelectedWord();
                return;
            }

            CheckUserTranslation();
        }

        private void CheckUserTranslation()
        {
            int trySucc = 0,
                tryTot = 1;

            if (userTranslation.Text.ToLower() == ActiveTranslation.Translation.ToLower())
            {
                nounCorrect++;
                textWordSuccess.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                textWordSuccess.Text = $"Correct: {ActiveTranslation.German} = [{ActiveTranslation.Translation}]";

                trySucc++;
            }
            else
            {
                textWordSuccess.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                textWordSuccess.Text = $"Error: {ActiveTranslation.German} = [{ActiveTranslation.Translation}]";
            }

            Tuple<int, int> tries;
            if (translateStats.TryGetValue(ActiveTranslation.ID, out tries))
            {
                trySucc += tries.Item1;
                tryTot += tries.Item2;
            }

            nounStats[ActiveNoun.ID] = new Tuple<int, int>(trySucc, tryTot);
            textTranslateStats.Text = $"{ActiveTranslation.German}: {trySucc} correct from {tryTot} tries";

            UpdateSelectedWord();
        }


        private void userTranslation_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                return;

            if (userTranslation.Text.Length < 2)
                return;

            var text = userTranslation.Text;

            var suggestion = autoSuggWords.Where(x => x.NormalizedTranslation.Contains(text)).Take(15).OrderBy(x => x.Translation).Select(x => x.Translation).ToArray();
            userTranslation.ItemsSource = suggestion;
        }

        private void userTranslation_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            CheckUserTranslation();
        }
    }

}
