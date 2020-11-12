﻿using InvataGermana.Data;
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
    public sealed partial class LessonsPage : Page
    {
        public LessonsPage()
        {
            this.InitializeComponent();

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
    }
}