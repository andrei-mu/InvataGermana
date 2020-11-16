using InvataGermana.Data;
using InvataGermana.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class EditLessonPage : Page
    {
        public EditLessonPage()
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

        private void btnDer_Click(object sender, RoutedEventArgs e)
        {
            AddNoun(Noun.Gender.Der);
        }

        private void btnDie_Click(object sender, RoutedEventArgs e)
        {
            AddNoun(Noun.Gender.Die);
        }

        private void btnDas_Click(object sender, RoutedEventArgs e)
        {
            AddNoun(Noun.Gender.Das);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                var noun = GetCurrentNoun(db);

                if (noun != null)
                {
                    db.nouns.Remove(noun);
                    db.SaveChanges();
                    UpdateCurrentLesson(db);
                }
            }
        }

        private void AddNoun(Noun.Gender gender)
        {
            if (string.IsNullOrEmpty(tbNouns.Text))
                return;

            var translation = tbNouns.Text.Split('=');
            if (translation.Length != 2)
            {
                AddErrorMessage("Please provide a translation");
                return;
            }

            var germanForms = translation[0].Trim();
            var translatedNoun = translation[1].Trim();

            var parts = germanForms.Split(';');
            if (parts.Length != 2)
            {
                AddErrorMessage("Need to provide singular and plural");
                return;
            }

            var s = parts[0];
            var p = parts[1];

            var ss = s.ToUpper()[0] + s.Substring(1).ToLower();
            var pp = p.ToUpper()[0] + p.Substring(1).ToLower();

            using (var db = new ApplicationDbContext())
            {
                var lesson = GetCurrentLesson(db);
                if (lesson != null)
                {
                    Noun noun = new Noun
                    {
                        Gen = gender,
                        Singular = ss.Trim(),
                        Plural = pp.Trim(),
                        Translation = translatedNoun,
                        LessonId = lesson.ID
                    };

                    var entry = db.nouns.Add(noun);

                    db.SaveChanges();

                    UpdateCurrentLesson(db);
                }

            }

            tbNouns.Text = string.Empty;
        }

        private void listViewLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                UpdateCurrentLesson(db);
            }
        }

        private void UpdateCurrentLesson(ApplicationDbContext dbContext)
        {
            var lesson = GetCurrentLesson(dbContext);

            if (lesson == null)
            {
                return;
            }

            var allnouns = dbContext.nouns.ToList();
            var nouns = dbContext.nouns.Where(x => x.Lesson.ID == lesson.ID).OrderBy(x => x.Singular).ToList();
            listLessonNouns.ItemsSource = nouns;
            tbNounsCount.Text = $"Lesson [{lesson.Title}] has {nouns.Count()} nouns.";
        }
        private Lesson GetCurrentLesson(ApplicationDbContext dbContext)
        {
            return listViewLessons.SelectedItem as Lesson;
        }

        private Noun GetCurrentNoun(ApplicationDbContext dbContext)
        {
            return listLessonNouns.SelectedItem as Noun;
        }

        private void AddErrorMessage(string error)
        {
        }
    }
}
