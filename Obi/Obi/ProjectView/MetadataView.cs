using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.command;

namespace Obi.ProjectView
{
    public partial class MetadataView : UserControl, IControlWithSelection
    {
        private ProjectView mView;             // parent project view
        private MetadataSelection mSelection;  // current selection
        private float mBaseFontSize;           // base font size
        private bool m_IsImportingMetadata; // flag to indicate if importing of metadata is going on
        
        public MetadataView()
        {
            InitializeComponent();
            mView = null;
            mSelection = null;
            mBaseFontSize = Font.SizeInPoints;
            m_IsImportingMetadata = false;
        }


        /// <summary>
        /// A new entry of the given kind can be added if this is not readonly, and if it is repeatable
        /// or there is not yet any occurrence of it.
        /// </summary>
        public bool CanAdd(MetadataEntryDescription d)
        {
            return
                d == null ||
                (!d.ReadOnly && (d.Repeatable || mView.Presentation.getListOfMetadata(d.Name).Count == 0));
        }

        /// <summary>
        /// A particular entry can be removed if it is not readonly and not the only occurrence in case of a required entry. 
        /// </summary>
        public bool CanRemove(MetadataEntryDescription d)
        {
        // do not allow delete metadata node if focus is in text boxes
        if (mContentTextbox.ContainsFocus || mNameTextbox.ContainsFocus) return false; 

            return
                d == null ||
                (!d.ReadOnly && (d.Occurrence != MetadataOccurrence.Required || mView.Presentation.getListOfMetadata(d.Name).Count > 1));
        }

        /// <summary>
        /// A metadata entry can be removed if it is selected.
        /// </summary>
        public bool CanRemoveMetadata
        {
            get
            {
                return mSelection != null && CanRemove(mSelection.Item.Description) &&
                    mView.CanDeleteMetadataEntry(mSelection.Item.Entry);
            }
        }

        /// <summary>
        /// The project view is showing a new presentation;
        /// reset the list of metadata panels.
        /// </summary>
        public void NewPresentation()
        {
            if (mView.Presentation != null)
            {
                ImportMetadata();
                mView.Presentation.MetadataEntryAdded += new MetadataEventHandler(Presentation_MetadataEntryAdded);
                mView.Presentation.MetadataEntryDeleted += new MetadataEventHandler(Presentation_MetadataEntryDeleted);
                mView.Presentation.MetadataEntryContentChanged += new MetadataEventHandler(Presentation_MetadataEntryContentChanged);
                mView.Presentation.MetadataEntryNameChanged += new MetadataEventHandler(Presentation_MetadataEntryNameChanged);
            }
        }

        public float ZoomFactor
        {
            set
            {
                float size = mBaseFontSize * value;
                int labelOffset = mNameTextbox.Location.Y - mNameLabel.Location.Y;
                mMetadataListView.Font = new Font(mMetadataListView.Font.FontFamily, size);
                mNameLabel.Font = new Font(mNameLabel.Font.FontFamily, size);
                mNameTextbox.Font = new Font(mNameTextbox.Font.FontFamily, size);
                mContentLabel.Font = new Font(mContentLabel.Font.FontFamily, size);
                mContentTextbox.Font = new Font(mContentTextbox.Font.FontFamily, size);
                mUpdateButton.Font = new Font(mUpdateButton.Font.FontFamily, size);
                int labelEdge = mNameLabel.Width > mContentLabel.Width ?
                    mNameLabel.Location.X + mNameLabel.Width + mNameLabel.Margin.Right :
                    mContentLabel.Location.X + mContentLabel.Width + mContentLabel.Margin.Right;
                mContentTextbox.Location = new Point(labelEdge + mContentTextbox.Margin.Left,
                    mUpdateButton.Location.Y - mUpdateButton.Margin.Top - mContentTextbox.Margin.Bottom - mContentTextbox.Height);
                mContentTextbox.Width = Width - mContentTextbox.Location.X - mContentTextbox.Margin.Right;
                mContentLabel.Location = new Point(labelEdge - mContentLabel.Margin.Right - mContentLabel.Width,
                    mContentTextbox.Location.Y + labelOffset);
                mNameTextbox.Location = new Point(labelEdge + mNameTextbox.Margin.Left,
                    mContentTextbox.Location.Y - mContentTextbox.Margin.Top - mNameTextbox.Margin.Bottom - mNameTextbox.Height);
                mNameTextbox.Width = mContentTextbox.Width;
                mNameLabel.Location = new Point(labelEdge - mNameLabel.Margin.Right - mNameLabel.Width,
                    mNameTextbox.Location.Y + labelOffset);
                mMetadataListView.Height = mNameTextbox.Location.Y - mNameTextbox.Margin.Top - mMetadataListView.Margin.Vertical;
            }
        }


        // A new metadata entry was added.
        private void Presentation_MetadataEntryAdded(object sender, MetadataEventArgs e)
        {
            // Let's not try to be clever now.
            ImportMetadata();
            //ClearTextBoxesFromCommandEvents ();
                    }

        // An entry was deleted.
        void Presentation_MetadataEntryDeleted ( object sender, MetadataEventArgs e )
            {
            // Let's not try to be clever now.
            ImportMetadata ();
            ClearTextBoxesFromCommandEvents ();
            mMetadataListView.Focus();
                        }

        // Metadata content has changed.
        void Presentation_MetadataEntryContentChanged(object sender, MetadataEventArgs e)
        {
            // Let's not try to be clever now.
            ImportMetadata();
            ClearTextBoxesFromCommandEvents ();
        }

        // Metadata name has changed.
        void Presentation_MetadataEntryNameChanged(object sender, MetadataEventArgs e)
        {
            // Let's not try to be clever now.
            ImportMetadata();
        }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }

        /// <summary>
        /// Get or set the current selection in the view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != null) ClearSelection();
                mSelection = value as MetadataSelection;
                if (mSelection != null) SetSelection();
            }
        }


        // Clear the selection
        private void ClearSelection()
        {
            if (mSelection != null) mSelection.Item.Item.Selected = false;
            mSelection = null;
            mNameTextbox.Text = "";
            mContentTextbox.Text = "";
        }

        private delegate void ImportMetadataInvokation () ;

        // Import metadata entries from the presentation
        private void ImportMetadata()
        {
        if (InvokeRequired)
            {
            Invoke ( new  ImportMetadataInvokation (ImportMetadata ) );
            }
        else
            {
            mMetadataListView.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler ( mMetadataListView_ItemChecked );
            m_IsImportingMetadata = true;
            mMetadataListView.Items.Clear ();
            string[] nameContent = new string[2];
            List<urakawa.metadata.Metadata> items = mView.Presentation.getListOfMetadata ();
            items.Sort ( delegate ( urakawa.metadata.Metadata a, urakawa.metadata.Metadata b )
            {
                int names = a.getName ().CompareTo ( b.getName () );
                return names == 0 ? a.getContent ().CompareTo ( b.getContent () ) : names;
            } );

            // a list of metadata names which already exists
            // purpose is to compare addable items and prevent their addition to list if it is already there
            List<string> ExistingItemsList = new List<string> ();

            foreach (urakawa.metadata.Metadata m in items)
                {
                nameContent[0] = m.getName ();
                nameContent[1] = m.getContent ();
                ListViewItem item = new ListViewItem ( nameContent );
                mMetadataListView.Items.Add ( item );
                item.Checked = true;
                item.Tag = m;

                ExistingItemsList.Add ( nameContent[0] );
                }
            List<string> addables = mView.AddableMetadataNames;
            addables.Sort ();
            addables.Add ( Localizer.Message ( "metadata_custom" ) );
            foreach (string name in addables)
                {
                if (!ExistingItemsList.Contains ( name ))
                    {
                    nameContent[0] = name;
                    nameContent[1] = "";
                    ListViewItem item = new ListViewItem ( nameContent );
                    mMetadataListView.Items.Add ( item );
                    item.Checked = false;
                    item.Tag = null;
                    }
                }
            m_IsImportingMetadata = false;
            mMetadataListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler ( mMetadataListView_ItemChecked );
            }
        }

        private void ClearTextBoxesFromCommandEvents ()
            {
            if (InvokeRequired)
                {
                Invoke ( new ImportMetadataInvokation ( ImportMetadata ) );
                }
            else
                {
                if (mMetadataListView.SelectedIndices.Count <= 0 )
                    {
                                                            mContentTextbox.Text = "";
                    mContentTextbox.AccessibleName = mContentLabel.Text.Replace ( "&", "" );
                    mNameTextbox.Text = "";
                                        }
                }
            }

        // Commit a change
        private void mCommitButton_Click(object sender, EventArgs e) { CommitValues(); }

                private void CommitValues()
        {
        if (mSelection != null)
            {
            if (GetTextFromContentTextbox () != "")
                {
                string contentBoxText = GetTextFromContentTextbox ();
                int itemIndex = 0;
                if (mMetadataListView.SelectedItems.Count > 0) itemIndex = mMetadataListView.SelectedItems[0].Index ;
                urakawa.metadata.Metadata entry = mSelection.Item.Entry;
                if (  entry == null && mNameTextbox.Text != "")
                    {
                    if (mMetadataListView.Items.Count > 0 && mMetadataListView.Items[itemIndex].Tag == null 
                        && CanAdd ( MetadataEntryDescription.GetDAISYEntry ( mNameTextbox.Text ) ))
                        {
                        entry = mView.AddMetadataEntry ( mNameTextbox.Text );
                                                }
                    else
                        {
                        return;
                        }
                    }
                CompositeCommand command =
                    mView.Presentation.CreateCompositeCommand ( Localizer.Message ( "modify_metadata_entry" ) );
                if (entry.getName () != mNameTextbox.Text)
                    {
                    if (CanModify ( mSelection.Item.Description, mNameTextbox.Text ))
                        {
                        command.append ( new Commands.Metadata.ModifyName ( mView, entry, mNameTextbox.Text ) );
                        }
                    else
                        {
                        MessageBox.Show ( String.Format ( Localizer.Message ( "metadata_name_error_text" ), mNameTextbox.Text ),
                            Localizer.Message ( "metadata_name_error_caption" ),
                            MessageBoxButtons.OK, MessageBoxIcon.Error );
                        mNameTextbox.Text = entry.getName ();
                        }
                    } 
                if (entry.getContent () != contentBoxText)
                                        {
                                        command.append ( new Commands.Metadata.ModifyContent ( mView, entry, contentBoxText) );
                    }
                if (command.getCount () > 0) mView.Presentation.getUndoRedoManager ().execute ( command );
                }
            else
                {
                if ( !IsDateMetadataSelected || string.IsNullOrEmpty( mContentTextbox.Text) ) MessageBox.Show ( Localizer.Message("MetadataView_CannotCommitEmptyContent"), Localizer.Message ( "Caption_Error" ) );

                if ( mSelection != null &&  mSelection.Item.Entry == null
                    && mMetadataListView.SelectedItems.Count > 0   && mMetadataListView.SelectedItems[0] != null && mMetadataListView.SelectedItems[0].Checked)
                    mMetadataListView.SelectedItems[0].Checked = false;
                } // contentbox check ends

            }
        }


        private string GetTextFromContentTextbox ()
            {
            if (mSelection != null  && mMetadataListView.SelectedItems.Count>0)
                {
                string entryName  = mMetadataListView.SelectedItems[0].Text;
                                if (IsDateMetadataSelected )
                {
                try
                    {
                    DateTime dateEntry = DateTime.Parse ( mContentTextbox.Text );
                    string strDate = dateEntry.Year.ToString () + "-" + dateEntry.Month.ToString () + "-" + dateEntry.Day.ToString ();
                                        return strDate;
                    }
                catch (System.FormatException ex)
                    {
                    MessageBox.Show ( Localizer.Message ( "Metadata_InvalidDateFormat" ) );
                    return "";
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    return "";
                    }
                }
            else
                {
                return mContentTextbox.Text;
                }
                }
                        return "";
            }

        private bool IsDateMetadataSelected             
            {
            get
                {
                if (mSelection != null && mMetadataListView.SelectedItems.Count > 0)
                    {
                    string entryName = mMetadataListView.SelectedItems[0].Text;
                                        return entryName != null &&
                        (entryName == "dtb:producedDate" || entryName == "dtb:revisionDate" || entryName == "dc:Date" || entryName == "dtb:sourceDate");
                    }
                return false;
                }
            }
            

        // Check an item to 
        private void mMetadataListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
        if (e.Item.Checked)
            {
                        //if (e.Item.Tag == null) mView.AddMetadataEntry(e.Item.Text); // Avn: checking disabled.
            }
        else
            {
            if (mNameTextbox.Enabled) mNameTextbox.Focus ();
            else if (mContentTextbox.Enabled) mContentTextbox.Focus ();
            }
                }

        // Verify that the name of an entry can be modified to a new name.
        private bool CanModify(MetadataEntryDescription description, string newName)
        {
            MetadataEntryDescription newDescription =
                MetadataEntryDescription.GetDAISYEntries().ContainsKey(newName) ?
                MetadataEntryDescription.GetDAISYEntries()[newName] : null;
            return CanRemove(description) &&
                (newDescription == null || CanAdd(newDescription));
        }

        private void mMetadataListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mMetadataListView.SelectedItems.Count == 0)
            {
                mView.Selection = null;
            }
            else
            {
                ListViewItem item = mMetadataListView.SelectedItems[0];
                mView.Selection = new MetadataSelection(mView.Presentation.RootNode, this,
                    new MetadataItemSelection(item, MetadataEntryDescription.GetDAISYEntry(item.Text)));
            }
        }

        private void SetSelection()
        {
            if (mSelection != null)
            {
                mSelection.Item.Item.Selected = true;
                mNameTextbox.Text = mSelection.Item.Item.Text;
                                mNameTextbox.AccessibleName = mNameLabel.Text.Replace("&" ,"")  ;
                //bool editableName = (mSelection.Item.Item.Checked || CanRemoveMetadata) && mSelection.Item.Item.Text == Localizer.Message("metadata_custom") ;
                bool editableName = mSelection.Item.Item.Text == Localizer.Message ( "metadata_custom" );

                mNameTextbox.TabStop = editableName;
                mNameTextbox.Enabled = editableName;
                mContentTextbox.Text = mSelection.Item.Item.SubItems[1].Text;
                bool editableContent = mSelection.Item.Description == null || !mSelection.Item.Description.ReadOnly ;
                                mContentTextbox.TabStop = editableContent;
                mContentTextbox.Enabled = editableContent;
                if (editableName)
                    mContentTextbox.AccessibleName = mContentLabel.Text.Replace ("&" ,"") ;
                else
                mContentTextbox.AccessibleName = mNameTextbox.Text;
            }
        }

        // Handle the tab key
        protected override bool ProcessDialogKey(Keys key)
        {
            if (key == Keys.Tab && ActiveControl != null)
            {
                Control c = ActiveControl;
                SelectNextControl(c, true, true, true, true);
                if (ActiveControl != null && c.TabIndex > ActiveControl.TabIndex) System.Media.SystemSounds.Beep.Play();
                return true;
            }
            else if (key == (Keys)(Keys.Shift | Keys.Tab) && ActiveControl != null)
            {
                Control c = ActiveControl;
                SelectNextControl(c, false, true, true, true);
                if (ActiveControl != null && c.TabIndex < ActiveControl.TabIndex) System.Media.SystemSounds.Beep.Play();
                return true;
            }
            else
            {
                return base.ProcessDialogKey(key);
            }
        }

        private void MetadataView_VisibleChanged(object sender, EventArgs e) { if (Visible) Focus(); }

        private void mContentTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) CommitValues();
        }

        private void mMetadataListView_ItemCheck ( object sender, ItemCheckEventArgs e )
            {
            if (mSelection == null && mMetadataListView.SelectedIndices.Count > 0 )
                e.NewValue = e.CurrentValue;
            else if (!m_IsImportingMetadata)
                {
                if (e.CurrentValue == CheckState.Checked && !CanRemoveMetadata
                    && mMetadataListView.Items[e.Index].Tag != null)
                    e.NewValue = CheckState.Checked;
                else if (mMetadataListView.Items[e.Index].Tag == null)
                    e.NewValue = CheckState.Unchecked;
                }
                        }

        private void mContentTextbox_Leave ( object sender, EventArgs e )
            {
            if (mContentTextbox.Text == "")
                {
                //MessageBox.Show ( Localizer.Message ( "MetadataView_CannotCommitEmptyContent" ), Localizer.Message ( "Caption_Error" ) );
                if ( mSelection != null && mSelection.Item.Entry == null
                    && mMetadataListView.SelectedItems.Count > 0 &&  mMetadataListView.SelectedItems[0] != null && mMetadataListView.SelectedItems[0].Checked)
                    mMetadataListView.SelectedItems[0].Checked = false;
                }
                        }

        private void mNameTextbox_Leave ( object sender, EventArgs e )
            {
            if (mNameTextbox.Text == "")
                {
                //MessageBox.Show ( Localizer.Message ( "MetadataView_CannotCommitEmptyContent" ), Localizer.Message ( "Caption_Error" ) );
                }
            }


    }

    public class MetadataItemSelection
    {
        private ListViewItem mItem;                     // item in the list view
        private MetadataEntryDescription mDescription;  // and corresponding description (may be null for free metadata)

        public MetadataItemSelection(ListViewItem item, MetadataEntryDescription description)
        {
            mItem = item;
            mDescription = description;
        }

        public urakawa.metadata.Metadata Entry { get { return (urakawa.metadata.Metadata)mItem.Tag; } }
        public MetadataEntryDescription Description { get { return mDescription; } }
        public ListViewItem Item { get { return mItem; } }
    }
}
