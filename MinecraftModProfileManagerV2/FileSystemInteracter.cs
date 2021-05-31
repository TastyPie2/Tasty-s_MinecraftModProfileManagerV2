using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace MinecraftModProfileManagerV2
{
    public class FileSystemInteracter
    {

        static public readonly string ProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\Profiles";
        static public readonly string ModsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\Mods";
        static public readonly string MinecraftInstallPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft";

        static public async Task<List<Profile>> GetProfilesAsync()
        {

            await CheckForMinecraftInstallation();
            await CheckNeededFolders();

            List<Profile> profiles = new List<Profile>();
            List<string> profilePaths = Directory.GetDirectories(ProfilePath).ToList();

            List<Task> FileTrasfers = new List<Task>();

            FileTrasfers.Add(Task.Run(() => profilePaths.ForEach(i => { Profile y = new Profile(); y.Path = i; y.Name = i.Split(@"\").Last(); profiles.Add(y); })));

            FileTrasfers.ForEach(i => i.Wait());

            return profiles;

        }
        
        static public async Task AddProfileAsync(string name, MainWindow mainWindowInstance)
        {
            await CheckForMinecraftInstallation();
            await CheckNeededFolders();

            string newProfilePath = ProfilePath + @"\" + name;

            /*if (Directory.Exists(newProfilePath))
                throw new IOException();*/

            Directory.CreateDirectory(newProfilePath);

            Profile newProfile = new Profile() { Name = name, Path = newProfilePath};

            mainWindowInstance.AddProfile(newProfile);

        }

        static public async Task UseChoosenProfile(Profile profile)
        {
            await CheckForMinecraftInstallation();
            await CheckNeededFolders();

            List<string> profileMods = Directory.GetFiles(profile.Path).ToList();

            await ClearModsFolder();

            List<Task> FileTrasfers = new List<Task>();

            FileTrasfers.Add(Task.Run(() => profileMods.ForEach(i =>
            {
                string destPath = ModsFolder + @"\" + i.Split(@"\").Last();
                File.Copy(i, destPath);
            })));

            FileTrasfers.ForEach(i => i.Wait());
        }

        static public async Task ClearModsFolder()
        {
            await CheckForMinecraftInstallation();
            await CheckNeededFolders();

            List<string> modsFolderContents = Directory.GetFiles(ModsFolder).ToList();
            List<Task> FileTrasfers = new List<Task>();

            FileTrasfers.Add(Task.Run(() => modsFolderContents.ForEach(i => File.Delete(i))));
            FileTrasfers.ForEach(i => i.Wait());

        }

        public static async Task DeleteProfile(Profile profile)
        {
            await CheckForMinecraftInstallation();
            await CheckNeededFolders();

            List<string> folderContents = Directory.GetFiles(profile.Path).ToList();
            List<Task> FileTrasfers = new List<Task>();

            FileTrasfers.Add(Task.Run(() => folderContents.ForEach(i => File.Delete(i))));
            FileTrasfers.ForEach(i => i.Wait());

            Directory.Delete(profile.Path);

        }

        static public async Task AddFileToProfile(Profile profile, List<string> modPaths)
        {
            await CheckForMinecraftInstallation();
            await CheckNeededFolders();

            List<Task> FileTrasfers = new List<Task>();

            modPaths.ForEach(i =>
            {
                FileTrasfers.Add(Task.Run(() => 
                {
                    string destPath = profile.Path + @"\" + i.Split(@"\").Last();
                    File.Copy(i, destPath); 
                }));
            });

            FileTrasfers.ForEach(i => i.Wait());

        }

        static public async Task CheckNeededFolders()
        {
            if (!Directory.Exists(ProfilePath))
            {
                Directory.CreateDirectory(ProfilePath);
            }
            if (!Directory.Exists(ModsFolder))
            {
                Directory.CreateDirectory(ModsFolder);
            }
        }

        static public async Task CheckForMinecraftInstallation()
        {
            if(!Directory.Exists(MinecraftInstallPath))
            {
                MessageBox.Show("No minecraft installation was found","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(-1);
            }
        }

    }
}
