using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinecraftModProfileManagerV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Profile> Profiles = new List<Profile>();

        public MainWindow()
        {
            InitializeComponent();
        }

        public async void LoadProfiles()
        {
            ProfilesSelector.Items.Clear();

            List<Profile> ProfilesFromDir = FileSystemInteracter.GetProfilesAsync().GetAwaiter().GetResult();

            ProfilesFromDir.ForEach(i => AddProfile(i));
        }

        public void AddProfile(Profile profile)
        {
            ProfilesSelector.Items.Add(string.Format("{0}", profile.Name));

            profile.ListIndex = Profiles.Count;
            Profiles.Add(profile);

        }

        private void PrimaryWindow__Init__(object sender, RoutedEventArgs e)
        {
            LoadProfiles();
            ProfilesSelector.SelectedIndex = 0;

            DeleteSelectedProfileButton.Margin = new Thickness(35,68,0,0);
        }

        private async void AddNewProfileButton_Click(object sender, RoutedEventArgs e)
        {
            await FileSystemInteracter.AddProfileAsync(ProfileNameTextBox.Text, this);
            ProfileNameTextBox.Text = "";
        }

        private void UseSelectedProfileButton_Click(object sender, RoutedEventArgs e)
        {
            FileSystemInteracter.UseChoosenProfile(Profiles[ProfilesSelector.SelectedIndex]);
        }

        private void OpenInExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = Profiles[ProfilesSelector.SelectedIndex];

            Process exporer = new Process();
            exporer.StartInfo.FileName = "explorer.exe";
            exporer.StartInfo.Arguments = profile.Path;

            exporer.Start();
        }

        private async void DeleteSelectedProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmationBoxResult = MessageBox.Show("This action cannot be undone, continue?", "Confirmation needed", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

            if(confirmationBoxResult == MessageBoxResult.OK)
            {
                await FileSystemInteracter.DeleteProfile(Profiles[ProfilesSelector.SelectedIndex]);
                LoadProfiles();
                ProfilesSelector.SelectedIndex = 0;
            }


        }
    }
}
