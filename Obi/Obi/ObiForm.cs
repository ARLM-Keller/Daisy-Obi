using Obi.Commands;
using Obi.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// The main for of the application.
    /// The form consists mostly of a menu bar and a project panel.
    /// We also keep an undo stack (the command manager) and settings.
    /// </summary>
    public partial class ObiForm : Form, IMessageFilter
    {
        private Project mProject;                // the project currently being authored
        private Settings mSettings;              // application settings
        private CommandManager mCommandManager;  // the undo stack for this project
        private Audio.VuMeterForm mVuMeterForm;  // keep track of a single VU meter form
        private Audio.VuMeter m_Vumeter ; // VuMeterForm is to be initialised again and again so this instance is required as member

        public bool AllowDelete
        {
            set { mDeleteToolStripMenuItem.Enabled = value; }
        }

        /// <summary>
        /// Application settings.
        /// </summary>
        public Settings Settings
        {
            get { return mSettings; }
        }

        /// <summary>
        /// The VU meter form owned by the main form can be shown and hidden from a menu.
        /// </summary>
        public Audio.VuMeterForm VuMeterForm
        {
            get { return mVuMeterForm; }
        }

        /// <summary>
        /// Initialize a new form.
        /// </summary>
        public ObiForm()
        {
            InitializeObi();
            if (mSettings.OpenLastProject && mSettings.LastOpenProject != "")
            {
                // open the last open project
                DoOpenProject(mSettings.LastOpenProject);
            }
            else
            {
                // no project opened, same as if we closed a project.
                StatusUpdateClosedProject();
            }
        }

        /// <summary>
        /// Initialize a new form with a project given as parameter.
        /// </summary>
        /// <param name="path">The project to open on startup.</param>
        public ObiForm(string path)
        {
            InitializeObi();
            DoOpenProject(path);
        }

        private void InitializeObi()
        {
            try
            {
                InitializeComponent();
                mProject = null;
                mSettings = null;
                mCommandManager = new CommandManager();
                InitializeVuMeter();
                InitializeSettings();
                InitialiseHighContrastSettings();
                mProjectPanel.TransportBar.StateChanged +=
                    new Obi.Events.Audio.Player.StateChangedHandler(TransportBar_StateChanged);
                mProjectPanel.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
                StatusUpdateClosedProject();
            }
            catch (Exception eAnyStartupException)
            {
                System.IO.StreamWriter tmpErrorLogStream = System.IO.File.CreateText(Application.StartupPath + Path.DirectorySeparatorChar + "ObiStartupError.txt");
                tmpErrorLogStream.WriteLine(eAnyStartupException.ToString());
                tmpErrorLogStream.Close();
                System.Windows.Forms.MessageBox.Show("An error occured while initializing Obi.\nPlease Submit a bug report, including the contents of " + Application.StartupPath + Path.DirectorySeparatorChar + "ObiStartupError.txt\nError text:\n" + eAnyStartupException.ToString(), "Obi initialization error");
            }
        }

        
        /// <summary>
        /// Set up the VU meter form.
        /// </summary>
        private void InitializeVuMeter()
        {
            m_Vumeter  = new Obi.Audio.VuMeter(mProjectPanel.TransportBar.AudioPlayer ,  mProjectPanel.TransportBar.Recorder );
            mProjectPanel.TransportBar.AudioPlayer.VuMeter = m_Vumeter;
            mProjectPanel.TransportBar.Recorder.VuMeterObject = m_Vumeter ;
            m_Vumeter.SetEventHandlers();
        }

        // setup a VuMeter form and show it
        private void ShowVuMeterForm ()
        {
            mVuMeterForm = new Audio.VuMeterForm(m_Vumeter );
            mVuMeterForm.MagnificationFactor = 1.5;
            // Kludgy
            mVuMeterForm.Show();
            //mVuMeterForm.Visible = false;
        }

        /// <summary>
        /// Show the state of the transport bar in the status bar.
        /// </summary>
        void TransportBar_StateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            Status(Localizer.Message(mProjectPanel.TransportBar._CurrentPlaylist.State.ToString()));
        }

        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status(String.Format(Localizer.Message("playback_rate"), mProjectPanel.TransportBar._CurrentPlaylist.PlaybackRate));
        }


        #region File menu event handlers

        private void mFileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForFileMenu();
        }

        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e) { NewProject(); }


        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// Try to close a possibly open project first.
        /// </summary>
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DidCloseProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = Localizer.Message("xuk_filter");
                dialog.InitialDirectory = mSettings.DefaultPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TryOpenProject(dialog.FileName);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Clear the list of recently opened files (prompt the user first.)
        /// </summary>
        private void mClearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Stop();
            if (MessageBox.Show(Localizer.Message("clear_recent_text"),
                    Localizer.Message("clear_recent_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                ClearRecentList();
            }
            Ready();
        }

        /// <summary>
        /// Save the current project under its current name, or ask for one if none is defined yet.
        /// </summary>
        /// <remarks>In the future, do not clear the command manager (only after cleanup.)</remarks>
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                mProjectPanel.TransportBar.Stop();
                mProject.Save();
                mCommandManager.Clear();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        private void mSaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Stop();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = Localizer.Message("xuk_filter");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mProject.SaveAs(dialog.FileName);
                AddRecentProject(dialog.FileName);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Revert the project to its last saved state.
        /// </summary>
        private void mDiscardChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                mProjectPanel.TransportBar.Stop();
                // Ask for confirmation (yes/no question)
                if (MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    DoOpenProject(mProject.XUKPath);
                }
            }
        }

        /// <summary>
        /// Close and clean up the current project.
        /// </summary>
        private void mCloseProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DidCloseProject())
            {
                mProjectPanel.CurrentSelection = null;
                mProject = null;
                mCommandManager.Clear();
            }
        }
        /// <summary>
        /// Clean the assets of a project
        /// </summary>
        private void mCleanProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
            
                try
                {
                    mProject.CleanProjectAssets();
                }
                catch (Exception x)
                {
                    //report an error and exit the function
                    MessageBox.Show(String.Format(Localizer.Message("didnt_clean_project_text"), x.Message),
                            Localizer.Message("didnt_clean_project_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.Cursor = Cursors.Default;
                    mProjectPanel.TransportBar.Enabled = true;
                    return;
                }

               //report success
               MessageBox.Show(Localizer.Message("cleaned_project_text"), Localizer.Message("cleaned_project_caption"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Cursor = Cursors.Default;
                mProjectPanel.TransportBar.Enabled = true;
            }
        }
        /// <summary>
        /// Export the project to DAISY 3.
        /// </summary>
        private void mExportAsDAISYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                if (mProject.Unsaved)
                {
                    DialogResult result = MessageBox.Show(Localizer.Message("export_unsaved_text"),
                        Localizer.Message("export_unsaved_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel) return;
                }
                this.Cursor = Cursors.WaitCursor;
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = Localizer.Message("export_choose_folder");
                dialog.SelectedPath = mSettings.DefaultExportPath;
                if (dialog.ShowDialog() == DialogResult.OK && IsExportDirectoryReady(dialog.SelectedPath))
                {
                    try
                    {
                        mProject.ExportToZed(dialog.SelectedPath);
                        MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"), dialog.SelectedPath),
                            Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("didnt_save_as_daisy_text"), dialog.SelectedPath, x.Message),
                            Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Ready();
                }
                this.Cursor = Cursors.Default;
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// The export directory is ready if it doesn't exist and can be created, or exists
        /// and is empty or can be emptied (or the user decided not to empty it.)
        /// </summary>
        private bool IsExportDirectoryReady(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                if (files.Length > 0)
                {
                    DialogResult result = MessageBox.Show(String.Format(Localizer.Message("empty_directory_text"), path),
                        Localizer.Message("empty_directory_caption"), MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception e)
                            {
                                DialogResult dialog = MessageBox.Show(String.Format(Localizer.Message("cannot_delete_text"),
                                    file, e.Message),
                                    Localizer.Message("cannot_delete_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format(Localizer.Message("cannot_create_directory_text"), path, e.Message),
                        Localizer.Message("cannot_create_directory_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private void mExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
                                    Close();
        }

        #endregion


        #region Edit menu event handlers

        private void mEditToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForEditMenu();
        }

        /// <summary>
        /// Handle the undo menu item.
        /// If there is something to undo, undo it and update the labels of undo and redo
        /// to synchronize them with the command manager.
        /// </summary>
        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                if (!mCommandManager.HasUndo) mProject.Reverted();
            }
        }

        /// <summary>
        /// Handle the redo menu item.
        /// If there is something to undo, undo it and update the labels of undo and redo
        /// to synchronize them with the command manager.
        /// </summary>
        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCommandManager.HasRedo) mCommandManager.Redo();
        }

        /// <summary>
        /// Cut depends on what is selected.
        /// </summary>
        private void mCutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null) mProjectPanel.Cut();
        }

        /// <summary>
        /// Copy depends on what is selected.
        /// </summary>
        private void mCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null) mProjectPanel.Copy();
        }

        /// <summary>
        /// Paste what's in the clipboard in/before what is selected.
        /// </summary>
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null) mProjectPanel.Paste();
        }

        /// <summary>
        /// Delete depens on what is selected.
        /// </summary>
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null) mProjectPanel.Delete();
        }

        /// <summary>
        /// Edit the metadata for the project.
        /// </summary>
        private void mMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject);
                // TODO replace this: if (mProject != null && dialog.ShowDialog() == DialogResult.OK) mProject.Modified();
                Ready();
                mProjectPanel.TransportBar.Enabled = true;
            }
        }


        /// <summary>
        /// Touch the project so that it seems that it was modified.
        /// Also refresh the display.
        /// </summary>
        private void mTouchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                if (!mCommandManager.HasUndo) mProject.Touch();
                mProjectPanel.SynchronizeWithCoreTree();
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        #endregion


        #region TOC menu event handlers

        private void mTocToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForTOCMenu();
        }

        private void mShowhideTableOfContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TOCPanelVisible)
            {
                mProjectPanel.HideTOCPanel();
            }
            else
            {
                mProjectPanel.ShowTOCPanel();
            }
        }

        private void mInsertSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.InsertSection();
        }

        private void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.AddSubSection();
        }

        private void mRenameSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.StartRenamingSelectedSection();
        }

        private void mMoveOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.MoveSectionNodeOut(mProjectPanel.CurrentSelectedSection);
        }

        private void mMoveInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.MoveSectionNodeIn(mProjectPanel.CurrentSelectedSection);
        }

        private void mShowInStripviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.ShowSelectedSectionInStripView();
        }

        #endregion


        #region Strips menu event handlers

        private void mStripsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForStripsMenu();
        }

        private void mInsertStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.InsertStrip();
        }

        private void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.StartRenamingSelectedStrip();
        }

        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ImportPhrases();
        }

        private void mSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.SplitBlock();
        }

        private void mQuickSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.QuickSplitBlock();
        }

        private void mApplyPhraseDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ApplyPhraseDetection();
        }

        private void mMergeWithPreviousAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MergeBlocks();
        }

        private void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MoveBlock(PhraseNode.Direction.Forward);
        }

        private void mMoveAudioBlockBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MoveBlock(PhraseNode.Direction.Backward);
        }

        private void mMarkAudioBlockAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ToggleSelectedAudioBlockUsed();
        }

        private void mMarkAudioBlockAsSectionHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MarkSelectedAudioBlockAsHeading();
        }

        private void mEditAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.EditAnnotationForSelectedAudioBlock();
        }

        private void mRemoveAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.RemoveAnnotationForAudioBlock();
        }

        private void mSetPageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.SetPageNumber();
        }

        private void mRemovePageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.RemovePageNumber();
        }

        private void mFocusOnAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.FocusOnAnnotation();
        }

        private void mGoToPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.GoToPage();
        }

        #endregion





        /// <summary>
        /// Handle errors when closing a project.
        /// </summary>
        /// <param name="message">The error message.</param>
        private void ReportDeleteError(string path, string message)
        {
            MessageBox.Show(String.Format(Localizer.Message("report_delete_error"), path, message));
        }

        /// <summary>
        /// Edit the user profile through the user profile dialog.
        /// </summary>
        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.UserProfile dialog = new Dialogs.UserProfile(mSettings.UserProfile);
            dialog.ShowDialog();
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Project tab. (JQ)
        /// </summary>
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings, mProject, mProjectPanel.TransportBar);
            dialog.SelectProjectTab();
            ShowPreferencesDialog(dialog);
        }

        private void ShowPreferencesDialog(Dialogs.Preferences dialog)
        {
            if (dialog.ShowDialog() == DialogResult.OK) UpdateSettings(dialog);
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Audio tab. (JQ)
        /// </summary>
        private void mAudioPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings, mProject, mProjectPanel.TransportBar);
            dialog.SelectAudioTab();
            ShowPreferencesDialog(dialog);
        }

        /// <summary>
        /// Update the settings after the user has made some changes in the preferrences dialog. (JQ)
        /// </summary>
        private void UpdateSettings(Dialogs.Preferences dialog)
        {
            if (dialog.IdTemplate.Contains("#")) mSettings.IdTemplate = dialog.IdTemplate;
            if (Directory.Exists(dialog.DefaultXUKDirectory)) mSettings.DefaultPath = dialog.DefaultXUKDirectory;
            if (Directory.Exists(dialog.DefaultDAISYDirectory)) mSettings.DefaultExportPath = dialog.DefaultDAISYDirectory;
            mSettings.OpenLastProject = dialog.OpenLastProject;
            mSettings.LastOutputDevice = dialog.OutputDevice.Name;
            mProjectPanel.TransportBar.AudioPlayer.SetDevice(this, dialog.OutputDevice);
            mSettings.LastInputDevice = dialog.InputDevice.Name;
            mProjectPanel.TransportBar.Recorder.InputDevice = dialog.InputDevice;
            mSettings.AudioChannels = dialog.AudioChannels;
            mSettings.SampleRate = dialog.SampleRate;
            mSettings.BitDepth = dialog.BitDepth;
            // tooltips
            mSettings.EnableTooltips = dialog.EnableTooltips;
            mProjectPanel.EnableTooltips = dialog.EnableTooltips;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        /// <remarks>Warn when closing while playing?</remarks>
        private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DidCloseProject())
            {
                try
                {
                    mSettings.SaveSettings();
                }
                catch (Exception x)
                {
                    MessageBox.Show(String.Format(Localizer.Message("save_settings_error_text"), x.Message),
                        Localizer.Message("save_settings_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                mProjectPanel.TransportBar.Stop();
                Application.Exit();

                // unhook User preferences system events 
                Microsoft.Win32.SystemEvents.UserPreferenceChanged
                -= new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            }
            else
            {
                e.Cancel = true;
                Ready();
            }
        }

        /// <summary>
        /// Add a new command to the command manager and update the status display and menu items.
        /// </summary>
        private void mProject_CommandCreated(object sender, Events.Project.CommandCreatedEventArgs e)
        {
            mCommandManager.Add(e.Command);
            UpdateEnabledItemsForUndoRedo();
        }

        /// <summary>
        /// Update the TOC menu when a tree node is (de)selected.
        /// </summary>
        private void TOCPanel_SelectedTreeNode(object sender, Events.Node.SelectedEventArgs e)
        {
            mAddSubSectionToolStripMenuItem.Enabled = e.Selected;
            mRenameSectionToolStripMenuItem.Enabled = e.Selected;
            mShowInStripviewToolStripMenuItem.Enabled = e.Selected;

        }

        /// <summary>
        /// Show the HTML help page.
        /// </summary>
        private void mHelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dialogs.Help help = new Dialogs.Help();
            help.WebBrowser.Url = new Uri(Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("help_file_name")));
            help.ShowDialog();
        }

        /// <summary>
        /// Show the help dialog.
        /// </summary>
        private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Dialogs.About()).ShowDialog();
        }




        /// <summary>
        /// Initialize the application settings: get the settings from the saved user settings or the system
        /// and add the list of recent projects (at least those that actually exist) to the recent project menu.
        /// </summary>
        private void InitializeSettings()
        {
            mSettings = Settings.GetSettings();
            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                if (!AddRecentProjectsItem((string)mSettings.RecentProjects[i])) mSettings.RecentProjects.RemoveAt(i);
            }
            try
            {
                mProjectPanel.TransportBar.AudioPlayer.SetDevice(this, mSettings.LastOutputDevice);
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("no_output_device_text"), Localizer.Message("no_output_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            try
            {
                mProjectPanel.TransportBar.Recorder.SetDevice(this, mSettings.LastInputDevice);
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("no_input_device_text"), Localizer.Message("no_input_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            // tooltips
            mProjectPanel.EnableTooltips = mSettings.EnableTooltips;
            if (mSettings.ObiFormSize.Width == 0 || mSettings.ObiFormSize.Height == 0)
            {
                mSettings.ObiFormSize = Size;
            }
            else
            {
                Size = mSettings.ObiFormSize;
            }
        }

        /// <summary>
        /// Update the title and status bar when the project is closed.
        /// </summary>
        private void StatusUpdateClosedProject()
        {
            this.Text = Localizer.Message("obi");
            if (mProject == null)
            {
                Ready();
            }
            else
            {
                mToolStripStatusLabel.Text = String.Format(Localizer.Message("closed_project"), mProject.Title);
                mProjectPanel.Project = null;
                EnableItemsProjectClosed();
            }
        }

        private void EnableItemsProjectClosed()
        {
            mShowSourceDEBUGToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Update the form (title and status bar) when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title);
            Status(String.Format(Localizer.Message("opened_project"), mProject.XUKPath));
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title);
            Status(String.Format(Localizer.Message("saved_project"), mProject.LastPath));
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title + "*");
            Ready();
        }




        









        /// <summary>
        /// Update the visibility and actual label of transport items.
        /// </summary>
        private void mTransportToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForTransportMenu();
        }

        private void UpdateEnabledItemsForTransportMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isNodeSelected = isProjectOpen && mProjectPanel.CurrentSelection != null;

            mShowHideVUMeterToolStripMenuItem.Text = Localizer.Message(( mVuMeterForm != null && mVuMeterForm.Visible) ? "hide_vu_meter" : "show_vu_meter");

            if (mProjectPanel.TransportBar.IsInlineRecording)
            {
                mPlayAllToolStripMenuItem.Enabled = true;
                mPlaySelectionToolStripMenuItem.Enabled = true;
                mStopToolStripMenuItem.Enabled = true;
            }
            else
            {

                if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    mPlayAllToolStripMenuItem.Enabled = isProjectOpen;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                    mPlaySelectionToolStripMenuItem.Enabled = isNodeSelected;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                    mStopToolStripMenuItem.Enabled = isNodeSelected;
                }
                else if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.NotReady)
                {
                    mPlayAllToolStripMenuItem.Enabled = false;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                    mPlaySelectionToolStripMenuItem.Enabled = false;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                    mStopToolStripMenuItem.Enabled = false;
                }
                else if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused)
                {
                    // Avn: changed to allowdirect change of Playback mode
                    mPlayAllToolStripMenuItem.Enabled = true; // mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                    // Avn: changed to allowdirect change of Playback mode
                    mPlaySelectionToolStripMenuItem.Enabled = (mProjectPanel.CurrentSelection != null);  // !mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                    mStopToolStripMenuItem.Enabled = true;
                }
                else // playing
                {
                    // Avn: changed to allowdirect change of Playback mode
                    mPlayAllToolStripMenuItem.Enabled = true; //mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("pause_all");
                    // Avn: changed to allowdirect change of Playback mode
                    mPlaySelectionToolStripMenuItem.Enabled = (mProjectPanel.CurrentSelection != null);  // !mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("pause");
                    mStopToolStripMenuItem.Enabled = true;
                }
                mRecordToolStripMenuItem.Enabled = mProjectPanel.TransportBar.CanRecord;
            }
        }

        internal void UndoLast()
        {
            if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                UpdateEnabledItemsForUndoRedo();
            }
        }

        // Transport bar stuff

        #region transport bar

        /// <summary>
        /// Show the VU meter form (creating it if necessary) or hide it.
        /// </summary>
        private void mShowHideVUMeterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mVuMeterForm != null && mVuMeterForm.Visible)
            {

                //mVuMeterForm.Hide();
                mVuMeterForm.Close();
            }
            else
            {
                ShowVuMeterForm();
            }
        }

        /// <summary>
        /// Play the whole book from the selected node, or from the beginning.
        /// If already playing, pause.
        /// </summary>
        private void mPlayAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Play();
        }

        /// <summary>
        /// Play the current selection (phrase or section.)
        /// </summary>
        private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Play(mProjectPanel.CurrentSelectionNode);
        }

        /// <summary>
        /// Play a single phrase node using the transport bar.
        /// </summary>
        private void Play(ObiNode node)
        {
            mProjectPanel.TransportBar.Play(node);
        }

        private void mPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Pause();
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        private void mStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_Stop();
        }

        private void TransportBar_Stop ()
        {
            mProjectPanel.TransportBar.Stop();
        }

        /// <summary>
        /// Record new assets.
        /// </summary>
        private void mRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Record();
        }

        #endregion

        private void mRewindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Rewind();
        }

        private void mFastForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.FastForward();
        }

        private void mPreviousSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.PrevSection();
        }

        private void mPreviousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_PreviousPhrase();
        }

        private void TransportBar_PreviousPhrase()
        {
            if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectPanel.TransportBar.PrevPhrase();
            }
        }

        private void mNextPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_NextPhrase();
        }

        private void TransportBar_NextPhrase()
        {
            if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectPanel.TransportBar.NextPhrase();
            }
        }

        private void mNextSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.NextSection();
        }

        /// <summary>
        /// Toggle section used/unsed.
        /// </summary>
        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.ToggleNodeUsedWithCommand(mProjectPanel.CurrentSelectedSection, true);
        }

        /// <summary>
        /// Toggle strip used/unused.
        /// </summary>
        private void mMarkStripAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ToggleSelectedStripUsed();
        }


        // Various utility functions

        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
        /// </summary>
        /// <param name="path">The path of the project to add.</param>
        private void AddRecentProject(string path)
        {
            if (mSettings.RecentProjects.Contains(path))
            {
                // the item was in the list so bump it up
                int i = mSettings.RecentProjects.IndexOf(path);
                mSettings.RecentProjects.RemoveAt(i);
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            if (AddRecentProjectsItem(path)) mSettings.RecentProjects.Insert(0, path);
        }

        /// <summary>
        /// Add an item in the recent projects list, if the file actually exists.
        /// The path relative to the project directory is shown.
        /// </summary>
        /// <param name="path">The path of the item to add.</param>
        /// <returns>True if the file was added.</returns>
        /// <remarks>The file was in the preferences but may have disappeared since.</remarks>
        private bool AddRecentProjectsItem(string path)
        {
            if (File.Exists(path))
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetDirectoryName(path) == mSettings.DefaultPath ? Path.GetFileName(path) : path;
                item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { TryOpenProject(path); });
                mOpenRecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clear the list of recent projects.
        /// </summary>
        private void ClearRecentList()
        {
            while (mOpenRecentProjectToolStripMenuItem.DropDownItems.Count > 2)
            {
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(0);
            }
            mSettings.RecentProjects.Clear();
        }

        /// <summary>
        /// Open a project without asking anything (using for reverting, for instance.)
        /// </summary>
        /// <param name="path">The path of the project to open.</param>
        /// <remarks>TODO: have a progress bar, and hide the panel while opening.</remarks>
        private void DoOpenProject(string path)
        {
            try
            {
                mProject = new Project(path);
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                mProject.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
                this.Cursor = Cursors.WaitCursor;
                mProject.Open(path);
                AddRecentProject(path);
                mSettings.LastOpenProject = path;
            }
            catch (Exception e)
            {
                // if opening failed, no project is open and we don't try to open it again next time.
                MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                mProject = null;
                mSettings.LastOpenProject = "";
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Update the status bar to say "Ready."
        /// </summary>
        private void Ready()
        {
            Status(Localizer.Message("ready"));
        }

        /// <summary>
        /// Display a message on the status bar.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void Status(string message)
        {
            mToolStripStatusLabel.Text = message;
        }

        /// <summary>
        /// Try to open a project from a XUK file.
        /// Actually open it only if a possible current project could be closed properly.
        /// </summary>
        /// <param name="path">The path of the XUK file to open.</param>
        private void TryOpenProject(string path)
        {
            if (DidCloseProject())
            {
                DoOpenProject(path);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Update the enabled items for all menus.
        /// </summary>
        /// <remarks>This is necessary to make sure that keyboard shortcuts work correctly.</remarks>
        private void UpdateEnabledItems()
        {
            UpdateEnabledItemsForFileMenu();
            UpdateEnabledItemsForEditMenu();
            UpdateEnabledItemsForTOCMenu();
            UpdateEnabledItemsForStripsMenu();
            UpdateEnabledItemsForTransportMenu();
        }

        /// <summary>
        /// Update the enabled items of the File menu.
        /// </summary>
        private void UpdateEnabledItemsForFileMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isProjectModified = isProjectOpen && mProject.Unsaved;
            bool isPlayingOrRecording = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectPanel.TransportBar.IsInlineRecording;

            // mNewProjectToolStripMenuItem.Enabled = !isPlayingOrRecording;
            mOpenProjectToolStripMenuItem.Enabled = !isPlayingOrRecording;
            mOpenRecentProjectToolStripMenuItem.Enabled = !isPlayingOrRecording && mSettings.RecentProjects.Count > 0;
            mClearListToolStripMenuItem.Enabled = !isPlayingOrRecording;
            mSaveProjectToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectModified;
            mSaveProjectAsToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectOpen;
            mDiscardChangesToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectModified;
            mCloseProjectToolStripMenuItem.Enabled = isProjectOpen && !isPlayingOrRecording;
            mExportAsDAISYToolStripMenuItem.Enabled = isProjectOpen && !isPlayingOrRecording;
            mCleanProjectToolStripMenuItem.Enabled = isProjectOpen && !isPlayingOrRecording;
        }

        /// <summary>
        /// Update the enabled items of the Edit menu.
        /// </summary>
        private void UpdateEnabledItemsForEditMenu()
        {
            UpdateEnabledItemsForUndoRedo();

            bool isPlayingOrRecording = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectPanel.TransportBar.IsInlineRecording;
            bool canCutCopyDelete = !isPlayingOrRecording && mProjectPanel.CurrentSelectionNode != null && !mProjectPanel.TransportBar.IsInlineRecording;
            string itemLabel = mProjectPanel.SelectedLabel;
            if (itemLabel != "") itemLabel = " " + itemLabel;
            ObiNode clipboardData = mProject == null ? null : mProject.Clipboard.Data as ObiNode;
            string pasteLabel = mProjectPanel.PasteLabel(clipboardData);
            if (pasteLabel != "") pasteLabel = " " + pasteLabel;

            mCutToolStripMenuItem.Enabled = canCutCopyDelete;
            mCutToolStripMenuItem.Text = String.Format(Localizer.Message("cut_menu_label"), itemLabel);
            mCopyToolStripMenuItem.Enabled = canCutCopyDelete;
            mCopyToolStripMenuItem.Text = String.Format(Localizer.Message("copy_menu_label"), itemLabel);
            mPasteToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectPanel.CanPaste(clipboardData);
            mPasteToolStripMenuItem.Text = String.Format(Localizer.Message("paste_menu_label"), pasteLabel);
            mDeleteToolStripMenuItem.Enabled = canCutCopyDelete;
            mDeleteToolStripMenuItem.Text = String.Format(Localizer.Message("delete_menu_label"), itemLabel);

            bool isProjectOpen = mProject != null;
            bool canTouch = !isPlayingOrRecording && isProjectOpen && !mProjectPanel.TransportBar.IsInlineRecording;
            mMetadataToolStripMenuItem.Enabled = canTouch;
            mFullMetadataToolStripMenuItem.Enabled = canTouch;
            mTouchProjectToolStripMenuItem.Enabled = canTouch;
        }

        /// <summary>
        /// Update the label for undo and redo (and their availability) depending on what is in the command manager.
        /// </summary>
        private void UpdateEnabledItemsForUndoRedo()
        {
            bool isPlayingOrRecording = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectPanel.TransportBar.IsInlineRecording;
            if (mCommandManager.HasUndo)
            {
                mUndoToolStripMenuItem.Enabled = !isPlayingOrRecording;
                mUndoToolStripMenuItem.Text = String.Format(Localizer.Message("undo_label"), Localizer.Message("undo"),
                    mCommandManager.UndoLabel);
            }
            else
            {
                mUndoToolStripMenuItem.Enabled = false;
                mUndoToolStripMenuItem.Text = Localizer.Message("undo");
            }
            if (mCommandManager.HasRedo)
            {
                mRedoToolStripMenuItem.Enabled = !isPlayingOrRecording;
                mRedoToolStripMenuItem.Text = String.Format(Localizer.Message("redo_label"), Localizer.Message("redo"),
                    mCommandManager.RedoLabel);
            }
            else
            {
                mRedoToolStripMenuItem.Enabled = false;
                mRedoToolStripMenuItem.Text = Localizer.Message("redo");
            }
            System.Diagnostics.Debug.Print("~~~ can{0} undo ~~~", mUndoToolStripMenuItem.Enabled ? "" : "NOT");
        }

        /// <summary>
        /// Update the enabled items of the Edit menu.
        /// </summary>
        private void UpdateEnabledItemsForTOCMenu()
        {
            mShowhideTableOfContentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            mShowhideTableOfContentsToolStripMenuItem.Enabled = mProject != null;

            bool isPlayingOrRecording = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing ||mProjectPanel.TransportBar.IsInlineRecording;
            bool isProjectOpen = mProject != null;
            bool noNodeSelected = isProjectOpen && mProjectPanel.CurrentSelection == null;
            bool isSectionNodeSelected = isProjectOpen && mProjectPanel.CurrentSelectedSection != null;
            bool isSectionNodeUsed = isSectionNodeSelected && mProjectPanel.CurrentSelectedSection.Used;
            bool isParentUsed = isSectionNodeSelected ?
                mProjectPanel.CurrentSelectedSection.ParentSection == null ||
                mProjectPanel.CurrentSelectedSection.ParentSection.Used : false;

            mAddSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && (noNodeSelected || isSectionNodeUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed;
            mMoveOutToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed &&
                mProjectPanel.Project.CanMoveSectionNodeOut(mProjectPanel.CurrentSelectionNode as SectionNode);
            mMoveInToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed &&
                mProjectPanel.Project.CanMoveSectionNodeIn(mProjectPanel.CurrentSelectionNode as SectionNode);
            
            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeSelected && isParentUsed;
            mMarkSectionAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("section"),
                Localizer.Message(!isSectionNodeSelected || isSectionNodeUsed ? "unused" : "used"));
            mShowInStripviewToolStripMenuItem.Enabled = isSectionNodeSelected;
        }

        private void UpdateEnabledItemsForStripsMenu()
        {
            bool isPlayingOrRecording = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectPanel.TransportBar.IsInlineRecording;
            bool isPaused = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused;
            bool isProjectOpen = mProject != null;
            bool isStripSelected = isProjectOpen && mProjectPanel.StripManager.SelectedSectionNode != null;
            bool isAudioBlockSelected = isProjectOpen && mProjectPanel.StripManager.SelectedPhraseNode != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.Index ==
                mProjectPanel.StripManager.SelectedPhraseNode.ParentSection.PhraseChildCount - 1;
            bool isAudioBlockFirst = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.Index == 0;
            bool isBlockClipBoardSet = isProjectOpen && mProject.Clipboard.Phrase != null;
            bool canMerge = isProjectOpen && mProjectPanel.StripManager.CanMerge;

            bool canInsertPhrase = !isPlayingOrRecording && isProjectOpen && mProjectPanel.StripManager.CanInsertPhraseNode;
            mImportAudioFileToolStripMenuItem.Enabled = canInsertPhrase;

            mInsertStripToolStripMenuItem.Enabled = isProjectOpen;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;

            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mQuickSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (isPlayingOrRecording || isPaused);
            mApplyPhraseDetectionToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMergeWithPreviousAudioBlockToolStripMenuItem.Enabled = !isPlayingOrRecording && canMerge;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockFirst;
            mMoveAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (!isAudioBlockFirst || !isAudioBlockLast);

            bool canRemoveAnnotation = !isPlayingOrRecording && isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.HasAnnotation;
            mEditAnnotationToolStripMenuItem.Enabled = !isPlayingOrRecording && isAudioBlockSelected;
            mRemoveAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;
            mFocusOnAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;

            mSetPageNumberToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectPanel.StripManager.CanSetPage;
            mRemovePageNumberToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectPanel.StripManager.CanRemovePage;
            mGoToPageToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectOpen && mProject.Pages > 0; 

            mShowInTOCViewToolStripMenuItem.Enabled = isStripSelected;

            mMarkAudioBlockAsUnusedToolStripMenuItem.Enabled = mProjectPanel.CanToggleAudioBlock;
            mMarkAudioBlockAsUnusedToolStripMenuItem.Text = mProjectPanel.ToggleAudioBlockString;
            
            mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled = isAudioBlockSelected &&
                !mProjectPanel.CurrentSelectedAudioBlock.IsHeading && mProjectPanel.CurrentSelectedAudioBlock.Used &&
                mProjectPanel.CurrentSelectedAudioBlock.Audio.getDuration().getTimeDeltaAsMillisecondFloat() > 0.0;
            mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled = isAudioBlockSelected &&
                mProjectPanel.CurrentSelectedAudioBlock.IsHeading;
            mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Visible = mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled;
            mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Visible = !mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled;
        }

        private void mViewHelpInExternalBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uri url = new Uri(Path.GetDirectoryName(GetType().Assembly.Location) + "\\help_en.html");
            System.Diagnostics.Process.Start(url.ToString());
        }

        private void mReportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uri url = new Uri("http://sourceforge.net/tracker/?func=add&group_id=149942&atid=776242");
            System.Diagnostics.Process.Start(url.ToString());
        }

        private void mShowInTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // should be a project panel method?
            if (mProjectPanel.CurrentSelectedStrip != null)
            {
                mProjectPanel.CurrentSelection = new NodeSelection(mProjectPanel.CurrentSelection.Node, mProjectPanel.TOCPanel);
                //since the tree can be hidden:
                mProjectPanel.ShowTOCPanel();
                mProjectPanel.TOCPanel.Focus();
            }
        }

        #region IMessageFilter Members

        private const UInt32 WM_KEYDOWN = 0x0100;
        private const UInt32 WM_SYSKEYDOWN = 0x0104;

        private void mProjectPanel_Load(object sender, EventArgs e)
        {
            Application.AddMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
            {
                System.Diagnostics.Debug.Print("*** Got WM_{0}KEYDOWN message ***", m.Msg == WM_SYSKEYDOWN ? "SYS" : "");
                UpdateEnabledItems();
            }
            return false;
        }

        #endregion


        private void InitialiseHighContrastSettings()
        {
            // Associate  user preference system events
            Microsoft.Win32.SystemEvents.UserPreferenceChanged
                += new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            //UserControls.Colors.SetHighContrastColors(SystemInformation.HighContrast);
            //mProjectPanel.TransportBar.SetHighContrastColors(SystemInformation.HighContrast);
            //BackColor = UserControls.Colors.ObiBackGround;
            
        }

        private void UserPreferenceChanged( object sender , EventArgs e )
        {
            UserControls.Colors.SetHighContrastColors( SystemInformation.HighContrast );
            //mProjectPanel.TransportBar.SetHighContrastColors(SystemInformation.HighContrast);
            BackColor = UserControls.Colors.ObiBackGround;
            mProject.Touch();
        }

/// <summary>
///  move keyboard focus amung TOC view, Strip view, Transport Bar
/// <see cref=""/>
/// </summary>
/// <param name="Clockwise">
///  true for clockwise movement
/// </param>
        private void MoveToNextPanel( bool Clockwise )
        {
                        mProjectPanel.TransportBar.PlayOnFocusEnabled = false;
                        if (mProjectPanel.CurrentSelection != null)
                        {
                            if (mProjectPanel.TOCPanel.ContainsFocus)
                            {
                                if (Clockwise)
                                {
                                    NodeSelection TempnodeSelection = mProjectPanel.CurrentSelection;
                                    mProjectPanel.StripManager.Focus();
                                    mProjectPanel.CurrentSelection = new NodeSelection(TempnodeSelection.Node, mProjectPanel.StripManager);
                                }
                                else
                                    mProjectPanel.TransportBar.Focus();
                            }
                            else if (mProjectPanel.StripManager.ContainsFocus)
                            {
                                if (Clockwise)
                                    mProjectPanel.TransportBar.Focus();
                                else
                                    FocusTOCPanel();
                            }
                            else if (mProjectPanel.TransportBar.ContainsFocus)
                            {
                                if (Clockwise)
                                    FocusTOCPanel();
                                else
                                {
                                    NodeSelection TempnodeSelection = mProjectPanel.CurrentSelection;
                                    mProjectPanel .StripManager.Focus();
                                    mProjectPanel.CurrentSelection = new NodeSelection(TempnodeSelection.Node , mProjectPanel.StripManager);
                                                                    }
                            }
                        }
                        else
                            mProjectPanel.TOCPanel.Focus();
            mProjectPanel.TransportBar.PlayOnFocusEnabled = true;
        }


        /// <summary>
        ///  convenience function to be used in MoveToNextPanel ()
        /// <see cref=""/>
        /// </summary>
        private void FocusTOCPanel()
        {
            if (mProjectPanel.CurrentSelectionNode.GetType().Name == "PhraseNode")
            {
                PhraseNode TempPhraseNode = mProjectPanel.CurrentSelectionNode as PhraseNode;
                mProjectPanel.CurrentSelection = new NodeSelection(TempPhraseNode.ParentSection, mProjectPanel.TOCPanel);
                mProjectPanel.TOCPanel.Focus();
            }
            else
                mProjectPanel.StripManager.ShowInTOCPanel();

        }

        //added by med june 4 2007
        /// <summary>
        /// Import an XHTML file and build the project structure from it.
        /// The requirements for the file to import are:
        /// 1. it is well-formed
        /// 2. the headings are ordered properly (i.e. h2 comes between h1 and h3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mNewProjectFromImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Enabled = false;
            if (!DidCloseProject())
            {
                mProjectPanel.TransportBar.Enabled = true;
                Ready();
                return;
            }
            
            //select a file for import
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Choose a file for import";
            openFile.Filter = "HTML | *.html";

            if (openFile.ShowDialog() != DialogResult.OK) return;

            Dialogs.NewProject dialog = new Dialogs.NewProject(
                mSettings.DefaultPath,
                Localizer.Message("default_project_filename"),
                Localizer.Message("obi_project_extension"),
                ImportStructure.grabTitle(new Uri(openFile.FileName)));
            dialog.MakeAutoTitleCheckboxInvisible();
            dialog.Text = "Create a new project starting from XHTML import";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            
            // let's see if we can actually write the file that the user chose (bug #1679175)
            try
            {
                FileStream file = File.Create(dialog.Path);
                file.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show(String.Format(Localizer.Message("cannot_create_file_text"), dialog.Path, x.Message),
                    Localizer.Message("cannot_create_file_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
               
           
            CreateNewProject(dialog.Path, dialog.Title, false);
            try
            {
                ImportStructure importer = new ImportStructure();
                importer.ImportFromXHTML(openFile.FileName, mProject);
            }
            catch (Exception ex)
            {
                //report failure and undo the creation of a new project
                MessageBox.Show("Import failed: " + ex.Message);
                mProject.Close();
                File.Delete(dialog.Path);
                mProjectPanel.TransportBar.Enabled = false;
                RemoveRecentProject(dialog.Path);
                return;
            }
        
            Ready();
            mProjectPanel.TransportBar.Enabled = true;
        }

        
        /// <summary>
        /// Remove a project from the recent projects list
        /// This is required when import fails halfway through
        /// </summary>
        /// <param name="p"></param>
        //added by med june 4 2007
        private void RemoveRecentProject(String path)
        {
            if (mSettings.RecentProjects.Contains(path))
            {
                int i = mSettings.RecentProjects.IndexOf(path);
                mSettings.RecentProjects.RemoveAt(i);
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
        }

        private void mShowSourceDEBUGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null) new Dialogs.ShowSource(mProject).Show();
        }

        // TODO: merge full and simple metadata editing into a single dialog with two tabs
        private void mFullMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullMetadata dialog = new FullMetadata(mProject);
            List<urakawa.metadata.Metadata> affected = new List<urakawa.metadata.Metadata>();
            foreach (object o in mProject.getPresentation ().getMetadataList())
            {
                urakawa.metadata.Metadata meta = (urakawa.metadata.Metadata)o;
                if (MetadataEntryDescription.GetDAISYEntries().Find(delegate(MetadataEntryDescription entry)
                    { return entry.Name == meta.getName(); }) != null)
                {
                    affected.Add(meta);
                    dialog.AddPanel(meta.getName(), meta.getContent());
                }
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (urakawa.metadata.Metadata m in affected) mProject.getPresentation ().deleteMetadata(m.getName());
                foreach (UserControls.MetadataPanel p in dialog.MetadataPanels)
                {
                    if (p.CanSetName)
                    {
                        urakawa.metadata.Metadata m = (urakawa.metadata.Metadata)mProject.getPresentation ().getMetadataFactory().createMetadata();
                        m.setName(p.EntryName);
                        m.setContent(p.EntryContent);
                        mProject.getPresentation().appendMetadata(m);
                    }
                    else
                    {
                        MessageBox.Show(String.Format(Localizer.Message("error_metadata_name_message"), p.EntryName),
                            Localizer.Message("error_metadata_name_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                mProject.Touch();
            }
        }

        private void ObiForm_ResizeEnd(object sender, EventArgs e)
        {
            mSettings.ObiFormSize = Size;
        }







        /// <summary>
        /// Check if a string representation of a directory 
        /// exists as a directory on the filesystem,
        /// if not, try to create it, asking the user first.
        /// </summary>
        /// <param name="path">String representation of the directory to be checked/created</param>
        /// <param name="checkEmpty">Check for empty directories.</param>
        /// <returns>True if the is suitable, false otherwise.</returns>        
        public static bool CanUseDirectory(string path, bool checkEmpty)
        {
            return File.Exists(path) ? false :
                Directory.Exists(path) ? CheckEmpty(path, checkEmpty) : DidCreateDirectory(path);
        }

        /// <summary>
        /// Check if a directory is empty or not; ask the user to confirm
        /// that they mean this directory even though it is not empty.
        /// </summary>
        /// <param name="path">The directory to check.</param>
        /// <param name="checkEmpty">Actually check.</param>
        private static bool CheckEmpty(string path, bool checkEmpty)
        {
            if (checkEmpty && Directory.GetFiles(path).Length > 0)
            {
                DialogResult result = MessageBox.Show(
                    String.Format(Localizer.Message("really_use_directory_text"), path),
                    Localizer.Message("really_use_directory_caption"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                return result == DialogResult.Yes;
            }
            else
            {
                return true;  // the directory was empty or we didn't need to check
            }
        }

        /// <summary>
        /// Create a new project. The application listens to the project's state change and
        /// command created events.
        /// </summary>
        /// <param name="path">Path of the XUK file to the project.</param>
        /// <param name="title">Title of the project.</param>
        /// <param name="createTitleSection">If true, a title section is automatically created.</param>
        private void CreateNewProject(string path, string title, bool createTitleSection)
        {
            mProject = new Project(path);
            mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
            mProject.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
            mProject.Initialize(title, mSettings.GeneratedID, mSettings.UserProfile, createTitleSection);
            AddRecentProject(mProject.XUKPath);
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about what to do.
        /// Close the project if that is what the user wants to do or if it was unmodified.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool DidCloseProject()
        {
            mProjectPanel.TransportBar.Stop();
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"),
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        mProject.Save();
                        mProject.Close();
                        return true;
                    case DialogResult.No:
                        mProject.Close();
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                if (mProject != null) mProject.Close();
                return true;
            }
        }

        /// <summary>
        /// Ask the user whether she wants to create a directory,
        /// and try to create it if she does.
        /// </summary>
        /// <param name="path">Path to the non-existing directory.</param>
        /// <returns>True if the directory was created.</returns>
        private static bool DidCreateDirectory(string path)
        {
            if (MessageBox.Show(
                String.Format(Localizer.Message("create_directory_query"), path),
                Localizer.Message("create_directory_caption"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    return true;  // did create the directory
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        String.Format(Localizer.Message("create_directory_failure"), path, e.Message),
                        Localizer.Message("error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;  // couldn't create the directory
                }
            }
            else
            {
                return false;  // didn't want to create the directory
            }
        }

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void NewProject()
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(
                mSettings.DefaultPath,
                Localizer.Message("default_project_filename"),
                Localizer.Message("obi_project_extension"),
                Localizer.Message("default_project_title"));
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // let's see if we can actually write the file that the user chose (bug #1679175)
                    FileStream file = File.Create(dialog.Path);
                    file.Close();
                    mSettings.CreateTitleSection = dialog.CreateTitleSection;
                    if (DidCloseProject())
                    {
                        CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection);
                    }
                    else
                    {
                        Ready();
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(String.Format(Localizer.Message("cannot_create_file_text"), dialog.Path, x.Message),
                        Localizer.Message("cannot_create_file_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Handle state change events from the project (closed, modified, opened, saved.)
        /// </summary>
        private void mProject_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            switch (e.Change)
            {
                case Obi.Events.Project.StateChange.Closed:
                    StatusUpdateClosedProject();
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    FormUpdateModifiedProject();
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    mProjectPanel.Project = mProject;
                    FormUpdateOpenedProject();
                    mCommandManager.Clear();
                    mProjectPanel.SynchronizeWithCoreTree();
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    FormUpdateSavedProject();
                    break;
            }
        }


        /// <summary>
        /// Format a time value. If less than a minute, display seconds and milliseconds.
        /// If less than an hour, display minutes and seconds. Otherwise show hh:mm:ss.
        /// </summary>
        /// <param name="time">The time to display (in milliseconds.)</param>
        /// <returns>The formatted string.</returns>
        public static string FormatTime(double time)
        {
            return time < 60000.0 ? FormatTime_ss_ms(time) :
                // time < 3600000.0 ? FormatTime_mm_ss(time) :
                FormatTime_hh_mm_ss(time);
        }

        /// <summary>
        /// Convenient function to format a milliseconds time into hh:mm:ss format.
        /// </summary>
        /// <param name="time">The time in milliseconds.</param>
        /// <returns>The time in hh:mm:ss format (fractions of seconds are discarded.)</returns>
        public static string FormatTime_hh_mm_ss(double time)
        {
            int s = Convert.ToInt32(time / 1000.0);
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(s / 60);
            str = (m % 60).ToString("00") + ":" + str;
            int h = m / 60;
            return h.ToString("00") + ":" + str;
        }

        private static string FormatTime_mm_ss(double time)
        {
            int s = Convert.ToInt32(Math.Floor(time / 1000.0));
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(Math.Floor(s / 60.0));
            return m.ToString("00") + ":" + str;
        }

        private static string FormatTime_ss_ms(double time)
        {
            time /= 1000.0;
            return time.ToString("0.00") + "s";
        }
    }
}
