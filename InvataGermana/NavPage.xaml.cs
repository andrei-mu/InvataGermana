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
    public sealed partial class NavPage : Page
    {
        public NavPage()
        {
            this.InitializeComponent();
            nvSample.SelectedItem = nvSample.MenuItems[0];
        }

        private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            switch(args.SelectedItemContainer.Tag as string)
            {
                case "LessonsPage":
                    contentFrame.Navigate(typeof(LessonsPage));
                    break;
                case "EditLessonPage":
                    contentFrame.Navigate(typeof(EditLessonPage));
                    break;
                case "DerDieDasPage":
                    contentFrame.Navigate(typeof(DerDieDasPage));
                    break;


            }
        }

        public string PrepareSuspension()
        {
            return contentFrame.GetNavigationState();
        }

        public void PrepareResum(string state)
        {
            contentFrame.SetNavigationState(state);
        }
    }
}
