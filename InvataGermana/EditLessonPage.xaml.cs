using InvataGermana.Data;
using InvataGermana.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

                listViewLessons.ItemsSource = db.lessons.OrderBy(x => x.Title).ToList();
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

                listViewLessons.ItemsSource = db.lessons.OrderBy(x => x.Title).ToList();
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

                    listViewLessons.ItemsSource = db.lessons.OrderBy(x => x.Title).ToList();
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

        private async Task<bool> DisplayDuplicateWordDialog(string lessonName, string translation)
        {
            ContentDialog duplicateWordDialog = new ContentDialog
            {
                Title = $"Duplicate word found in: [{lessonName}]",
                Content = $"Previous word meaning: \n\t[{translation}]\nStill add?",
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await duplicateWordDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }

            return false;
        }

        private async void AddGenericWord(string text, Word.SpeechPart part, Word.Gender gender)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var translation = text.Split('=');
            if (translation.Length != 2)
            {
                AddErrorMessage("Please provide a translation");
                return;
            }

            var germanForms = translation[0].Trim();
            var translatedForm = translation[1].Trim();

            var sep = ",;".ToCharArray();
            var parts = germanForms.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                AddErrorMessage("Need to provide singular and plural");
                return;
            }

            var s = parts[0].ToLower().Trim();
            var p = (parts.Length > 1)? parts[1].ToLower().Trim() : "";

            if (part == Word.SpeechPart.Noun)
            {
                s = s.ToUpper()[0] + s.Substring(1).ToLower();
                p = p.ToUpper()[0] + p.Substring(1).ToLower();
            }

            using (var db = new ApplicationDbContext())
            {
                var lesson = GetCurrentLesson(db);

                var slower = s.ToLower();

                var duplicateWord = db.words.Where(x => x.German.ToLower() == slower).FirstOrDefault();
                if (duplicateWord != null)
                {
                    var duplicateLesson = db.lessons.Where(x => x.ID == duplicateWord.LessonId).FirstOrDefault();
                    bool ret = await DisplayDuplicateWordDialog(duplicateLesson.Title,
                        duplicateWord.Translation);
                    if (!ret)
                        return;
                }

                if (lesson != null)
                {
                    Word noun = new Word
                    {
                        SpeechType = part,
                        Gen = gender,
                        German = s,
                        Plural = p,
                        Translation = translatedForm,
                        LessonId = lesson.ID
                    };

                    var entry = db.words.Add(noun);

                    db.SaveChanges();

                    UpdateCurrentLesson(db);
                }
            }
        }
        private void AddNoun(Word.Gender gender)
        {
            if (string.IsNullOrEmpty(tbNouns.Text))
                return;

            AddGenericWord(tbNouns.Text, Word.SpeechPart.Noun, gender);

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


            tbLessonName.Text = lesson.Title;

            List<Word> allWords;
            if (toggleOrder.IsOn)
            {
                allWords = dbContext.words.Where(x => x.Lesson.ID == lesson.ID).OrderBy(x => x.SpeechType).ThenBy(x => x.German.ToLower()).ToList();
            }
            else
            {
                allWords = dbContext.words.Where(x => x.Lesson.ID == lesson.ID).OrderBy(x => x.SpeechType).ThenBy(x => x.Gen).ThenBy(x => x.German.ToLower()).ToList();
            }
            
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

            AddGenericWord(tbWords.Text, Word.SpeechPart.Other, Word.Gender.None);

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

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                UpdateCurrentLesson(db);
            }
        }

        private void btnAddVerb_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbWords.Text))
                return;

            AddGenericWord(tbWords.Text, Word.SpeechPart.Verb, Word.Gender.None);

            tbWords.Text = string.Empty;

        }
    }
}
