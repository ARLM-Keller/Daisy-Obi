using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog for creating a new project.
    /// The user can choos a title for the project and a file to save it to.
    /// </summary>
    public partial class NewProject : Form
    {
        /// <summary>
        /// The chosen title for the project.
        /// </summary>
        public string Title
        {
            get
            {
                return titleBox.Text;
            }
        }

        /// <summary>
        /// The chosen path for the XUK project file.
        /// </summary>
        public string Path
        {
            get
            {
                return fileBox.Text;
            }
        }

        /// <summary>
        /// Create a new dialog with default information (dummy name and default path.)
        /// </summary>
        /// <param name="path">The initial directory where to create the project.</param>
        public NewProject(string path)
        {
            InitializeComponent();
            titleBox.Text = Localizer.Message("new_project");
            fileBox.Text = path;
            GenerateFileName();
        }

        /// <summary>
        /// Update the path text box with the selected path from the file chooser.
        /// </summary>
        private void selectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(fileBox.Text);
            dialog.Filter = "XUK project file (*.xuk)|*.xuk";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileBox.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// Generate a new XUK file name when a new title is chosen.
        /// </summary>
        private void titleBox_Leave(object sender, EventArgs e)
        {
            GenerateFileName();
        }

        /// <summary>
        /// Generate a full path from the initial directory and the title of the project.
        /// </summary>
        private void GenerateFileName()
        {
            fileBox.Text = String.Format(@"{0}\{1}.xuk", System.IO.Path.GetDirectoryName(fileBox.Text),
                Project.ShortName(titleBox.Text));
        }
    }
}