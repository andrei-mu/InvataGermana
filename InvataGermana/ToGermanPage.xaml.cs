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
    public sealed partial class ToGermanPage : Page
    {
        private List<Word> SelectedWords { get; set; }
        private readonly Random random = new Random();

        public ToGermanPage()
        {
            this.InitializeComponent();
    }

        private void listViewLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lessons = listViewLessons.SelectedItems.Select(i => i as Lesson).ToList();

            using (var db = new ApplicationDbContext())
            {
                SelectedWords = db.words.Where(x => lessons.Any(y => y.ID == x.Lesson.ID)).ToList();
            }

            lessonsCount.Text = listViewLessons.SelectedItems.Count.ToString();
            wordsCount.Text = SelectedWords.Count.ToString();
        }

        private void UpdateSelection()
        {
            //int count = 
        }

        private void btnClearResults_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
