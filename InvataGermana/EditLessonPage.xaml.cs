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

        private void btnAddLesson_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                var lesson = new Lesson { Title = tbLessonName.Text };

                db.lessons.Add(lesson);
                db.SaveChanges();

                listViewLessons.ItemsSource = db.lessons.ToList();
            }

            tbLessonName.Text = string.Empty;

        }

        private void btnDeleteLesson_Click(object sender, RoutedEventArgs e)
        {
            var item = listViewLessons.SelectedItem as Lesson;
            if (item is null)
            {
                return;
            }

            using (var db = new ApplicationDbContext())
            {
                db.lessons.Remove(item);
                db.SaveChanges();

                listViewLessons.ItemsSource = db.lessons.ToList();
            }
        }

        private void btnRenameLesson_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbLessonName.Text))
                return;

            using (var db = new ApplicationDbContext())
            {
                var lesson = GetCurrentLesson(db);

                if (lesson != null)
                {
                    var dbLesson = db.lessons.SingleOrDefault(l => l.ID == lesson.ID);
                    dbLesson.Title = tbLessonName.Text;

                    db.SaveChanges();

                    listViewLessons.ItemsSource = db.lessons.ToList();
                }
            }

            tbLessonName.Text = string.Empty;
        }

        private void btnDer_Click(object sender, RoutedEventArgs e)
        {
            AddNoun(Word.Gender.Der);
        }

        private void btnDie_Click(object sender, RoutedEventArgs e)
        {
            AddNoun(Word.Gender.Die);
        }

        private void btnDas_Click(object sender, RoutedEventArgs e)
        {
            AddNoun(Word.Gender.Das);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                var noun = GetCurrentWord(db);

                if (noun != null)
                {
                    db.words.Remove(noun);
                    db.SaveChanges();
                    UpdateCurrentLesson(db);
                }
            }
        }

        private void AddNoun(Word.Gender gender)
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
                    Word noun = new Word
                    {
                        SpeechType = Word.SpeechPart.Noun,
                        Gen = gender,
                        German = ss.Trim(),
                        Plural = pp.Trim(),
                        Translation = translatedNoun,
                        LessonId = lesson.ID
                    };

                    var entry = db.words.Add(noun);

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

            var allWords = dbContext.words.Where(x => x.Lesson.ID == lesson.ID).OrderBy(x => x.SpeechType).ThenBy(x => x.German.ToLower()).ToList();
            listLessonNouns.ItemsSource = allWords;

            tbNounsCount.Text = $"Lesson [{lesson.Title}] has {allWords.Count()} words";
        }
        private Lesson GetCurrentLesson(ApplicationDbContext dbContext)
        {
            return listViewLessons.SelectedItem as Lesson;
        }

        private Word GetCurrentWord(ApplicationDbContext dbContext)
        {
            return listLessonNouns.SelectedItem as Word;
        }

        private void AddErrorMessage(string error)
        {
        }

        private void btnWord_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbWords.Text))
                return;

            var wordParts = tbWords.Text.Split('=');
            if (wordParts.Length != 2)
            {
                AddErrorMessage("Please provide a translation");
                return;
            }

            var germanForms = wordParts[0].Trim();
            var translation = wordParts[1].Trim();

            using (var db = new ApplicationDbContext())
            {
                var lesson = GetCurrentLesson(db);
                if (lesson != null)
                {
                    var word = new Word
                    {
                        SpeechType = Word.SpeechPart.Other,
                        German = germanForms,
                        Translation = translation,
                        LessonId = lesson.ID
                    };

                    var entry = db.words.Add(word);

                    db.SaveChanges();

                    UpdateCurrentLesson(db);
                }
            }

            tbWords.Text = string.Empty;
        }

        private void btnDeleteWord_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                var word = GetCurrentWord(db);

                if (word != null)
                {
                    db.words.Remove(word);
                    db.SaveChanges();
                    UpdateCurrentLesson(db);
                }
            }
        }
    }
}
