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
    public sealed partial class DerDieDasPage : Page
    {
        private List<Noun> selectedNouns = new List<Noun>();
        private Dictionary<int, Tuple<int, int>> nounStats = new Dictionary<int, Tuple<int, int>>();
        private int noTries = 0;
        private int noSuccess = 0;
        private Noun ActiveNoun { get; set; }
        private readonly Random random = new Random();

        public DerDieDasPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                listViewLessons.ItemsSource = db.lessons.OrderBy(x => x.Title).ToList();
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

            object derdiedasObj = null;
            if (false == container.Values.TryGetValue("derdiedas", out derdiedasObj))
                return;

            var parts = derdiedasObj.ToString().Split(';');

            var regex = new Regex(@"(?<a>\d+)=\[(?<b>\d+),(?<c>\d+)\]");
            foreach(var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                var match = regex.Match(part);
                if (match.Success)
                {
                    var gg = match.Groups;
                    if (gg.Count != 4)
                        continue;

                    nounStats[int.Parse(gg[1].Value)] = new Tuple<int, int>(int.Parse(gg[2].Value),
                                                                            int.Parse(gg[3].Value));
                }
            }

            return;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var container = localSettings.Containers["DeutschPractice"];

            var sb = new StringBuilder();
            foreach( var key in nounStats.Keys)
            {
                var val = nounStats[key];
                var succ = val.Item1;
                var total = val.Item2;

                sb.Append($"{key}=[{succ},{total}];");
            }

            container.Values["derdiedas"] = sb.ToString();
        }


        private void listViewLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lessons = listViewLessons.SelectedItems.Select(i => i as Lesson).ToList();

            using (var db = new ApplicationDbContext())
            {
                selectedNouns = db.nouns.Where(x => lessons.Any(y => y.ID == x.LessonID)).ToList();
            }

            lessonsCount.Text = listViewLessons.SelectedItems.Count.ToString();
            nounsCount.Text = selectedNouns.Count.ToString();

            UpdateSelectedNoun();
        }

        private void UpdateSelectedNoun()
        {
            if (selectedNouns.Count == 0)
            {
                currentNoun.Text = string.Empty;

                return;
            }

            int idx = random.Next(selectedNouns.Count);
            ActiveNoun = selectedNouns[idx];
            currentNoun.Text = ActiveNoun.Singular;
        }

        private void btnDer_Click(object sender, RoutedEventArgs e)
        {
            DerDieDasOption(Noun.Gender.Der);
        }

        private void btnDie_Click(object sender, RoutedEventArgs e)
        {
            DerDieDasOption(Noun.Gender.Die);
        }

        private void btnDas_Click(object sender, RoutedEventArgs e)
        {
            DerDieDasOption(Noun.Gender.Das);
        }

        private void DerDieDasOption(Noun.Gender gender)
        {
            noTries++;
            int trySucc = 0,
                tryTot = 1;

            if (gender == ActiveNoun.Gen)
            {
                noSuccess++;
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
            textSessionStats.Text = $"{noSuccess} correct from {noTries} tries";

            UpdateSelectedNoun();
        }
    }

}
