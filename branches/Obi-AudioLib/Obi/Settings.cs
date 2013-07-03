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
        public bool AllowOverwrite;            // allow/disallow overwriting audio when recording
        public int AudioChannels;              // number of channels for recording
        public bool AudioClues;                // use audio clues (or not.)
        public float AudioScale;               // scale of audio in waveform views
        public int BitDepth;                   // sample bit depth
        public ColorSettings ColorSettings;    // current color settings
        public ColorSettings ColorSettingsHC;  // current color settings for high contrast
        public bool CreateTitleSection;        // defaulf for "create title section" in new project
        public string DefaultPath;             // default location
        public bool EnableTooltips;            // enable or disable tooltips
        public float FontSize;                 // global font size (all font sizes must be relative to this one)
        public string LastInputDevice;         // the name of the last input device selected by the user
        public string LastOpenProject;         // path to the last open project
        public string LastOutputDevice;        // the name of the last output device selected by the user
        public uint MaxPhraseDurationMinutes;  // maximum phrase duration in minutes for autosplitting during import
        public Size NewProjectDialogSize;      // size of the new project dialog
        public AudioLib.VuMeter.NoiseLevelSelection NoiseLevel;  // noise level for low amplitude detection
        public double NudgeTimeMs;             // nudge time in milliseconds
        public Size ObiFormSize;               // size of the form (for future sessions)
        public bool OpenLastProject;           // open the last open project at startup
        public string PipelineScriptsPath;     // path to the pipeline script for the DTB audio encoder
        public bool PlayIfNoSelection;         // play all or nothing if no selection
        public bool PlayOnNavigate;            // start playback when navigating, or just change the selection
        public int PreviewDuration;            // playback preview duration in milliseconds
        public ArrayList RecentProjects;       // paths to projects recently opened
        public int SampleRate;                 // sample rate in Hertz
        public bool SplitPhrasesOnImport;      // split phrases on import
        public bool SynchronizeViews;          // keep views synchronized
        public UserProfile UserProfile;        // the user profile
        public bool WrapStripContents;         // wrap strip contents in the content view
        public float ZoomFactor;               // global zoom factor
        public bool AutoSave_RecordingEnd ; // flag to audo save whenever recording ends
        public bool AutoSaveTimeIntervalEnabled;// enables / disables auto save after time interval contained in AutoSaveTimeInterval
        public int AutoSaveTimeInterval; // time interval after which project will be auto saved 

        private static readonly string SETTINGS_FILE_NAME = "obi_settings.xml";


        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static Settings GetSettings()
        {
            Settings settings = new Settings();
            settings.AudioChannels = 1;
            settings.AudioClues = false;
            settings.AudioScale = 0.01f;
            settings.AllowOverwrite = false;
            settings.BitDepth = 16;
            settings.ColorSettings = ColorSettings.DefaultColorSettings();
            settings.ColorSettingsHC = ColorSettings.DefaultColorSettingsHC();
            settings.DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings.EnableTooltips = true;
            settings.FontSize = 10.0f;
            settings.LastInputDevice = "";
            settings.LastOpenProject = "";
            settings.LastOutputDevice = "";
            settings.MaxPhraseDurationMinutes = 10;
            settings.NewProjectDialogSize = new Size(0, 0);
            settings.NoiseLevel = AudioLib.VuMeter.NoiseLevelSelection.Medium;
            settings.NudgeTimeMs = 100.0;
            settings.ObiFormSize = new Size(0, 0);
            settings.OpenLastProject = false;
            settings.PreviewDuration = 1500;
            settings.PipelineScriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                Path.Combine ( "Pipeline-lite", "scripts" ) );
            settings.PlayIfNoSelection = true;
            settings.PlayOnNavigate = false;
            settings.RecentProjects = new ArrayList();
            settings.SampleRate = 44100;
            settings.SplitPhrasesOnImport = true;
            settings.SynchronizeViews = true;
            settings.UserProfile = new UserProfile();
            settings.WrapStripContents = true;
            settings.ZoomFactor = 1.0f;
            settings.AutoSave_RecordingEnd = false;
            settings.AutoSaveTimeIntervalEnabled= true;
            settings.AutoSaveTimeInterval = 1800000; // saving time interval in ms

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