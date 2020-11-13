using InvataGermana.Data;
using InvataGermana.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class DerDieDasPage : Page
    {
        private List<Noun> activeNouns = new List<Noun>();
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

        private void listViewLessons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lessons = listViewLessons.SelectedItems.Select(i => i as Lesson).ToList();

            using (var db = new ApplicationDbContext())
            {
                activeNouns = db.nouns.Where(x => lessons.Any(y => y.ID == x.LessonID)).ToList();
            }

            lessonsCount.Text = listViewLessons.SelectedItems.Count.ToString();
            nounsCount.Text = activeNouns.Count.ToString();
            
            if (activeNouns.Count == 0)
            {
                currentNoun.Text = string.Empty;

                return;
            }

            int idx = random.Next(activeNouns.Count);
            ActiveNoun = activeNouns[idx];
            currentNoun.Text = ActiveNoun.Singular;
        }

        private void btnDer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDie_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDas_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
