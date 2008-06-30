using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Obi
{
    /// <summary>
    /// Persistent application settings.
    /// </summary>
    /// <remarks>It also seems that making a change in the class resets the existing settings.</remarks>
    [Serializable()]
    public class Settings
    {
        public bool AllowOverwrite;         // allow/disallow overwriting audio when recording
        public int AudioChannels;           // number of channels for recording
        public int BitDepth;                // sample bit depth
        public ColorSettings ColorSettings;  // current color settings
        public bool CreateTitleSection;     // defaulf for "create title section" in new project
        public string DefaultExportPath;    // default path for DAISY export
        public string DefaultPath;          // default location
        public bool EnableTooltips;         // enable or disable tooltips
        public float FontSize;              // global font size (all font sizes must be relative to this one)
        public string LastInputDevice;      // the name of the last input device selected by the user
        public string LastOpenProject;      // path to the last open project
        public string LastOutputDevice;     // the name of the last output device selected by the user
        public Size NewProjectDialogSize;   // size of the new project dialog
        public double NudgeTimeMs;          // nudge time in milliseconds
        public Size ObiFormSize;            // size of the form (for future sessions)
        public bool OpenLastProject;        // open the last open project at startup
        public bool PlayIfNoSelection;      // play all or nothing if no selection
        public bool PlayOnNavigate;         // start playback when navigating, or just change the selection
        public int PreviewDuration;         // playback preview duration in milliseconds
        public ArrayList RecentProjects;    // paths to projects recently opened
        public int SampleRate;              // sample rate in Hertz
        public bool SynchronizeViews;       // keep views synchronized
        public UserProfile UserProfile;     // the user profile
        public bool WrapStrips;             // wrapping in content view

        private static readonly string SETTINGS_FILE_NAME = "obi_settings.xml";


        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static Settings GetSettings()
        {
            Settings settings = new Settings();
            settings.AudioChannels = 1;
            settings.AllowOverwrite = false;
            settings.BitDepth = 16;
            settings.ColorSettings = ColorSettings.DefaultColorSettings();
            settings.DefaultExportPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings.DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings.EnableTooltips = true;
            settings.FontSize = 10.0f;
            settings.LastInputDevice = "";
            settings.LastOpenProject = "";
            settings.LastOutputDevice = "";
            settings.NewProjectDialogSize = new Size(0, 0);
            settings.NudgeTimeMs = 100.0;
            settings.ObiFormSize = new Size(0, 0);
            settings.OpenLastProject = false;
            settings.PreviewDuration = 1500;
            settings.PlayIfNoSelection = true;
            settings.PlayOnNavigate = true;
            settings.RecentProjects = new ArrayList();
            settings.SampleRate = 44100;
            settings.SynchronizeViews = true;
            settings.UserProfile = new UserProfile();
            settings.WrapStrips = false;
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Open, FileAccess.Read, file);
                SoapFormatter soap = new SoapFormatter();
                settings = (Settings)soap.Deserialize(stream);
                stream.Close();
            }
            catch (Exception) { }
            return settings;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }
    }
}
