using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    public partial class TOCView : UserControl, IControlWithSelection
    {
        private ProjectView mView;  // the parent project view

        /// <summary>
        /// Create a new TOC view as part of a project view
        /// </summary>
        /// <param name="view"></param>
        public TOCView(ProjectView view) : this() { mView = view; }
        public TOCView() { InitializeComponent(); }


        /// <summary>
        /// Set a new project for this view.
        /// </summary>
        public void NewProject()
        {
            mView.Project.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(TOCView_treeNodeAdded);
            mView.Project.getPresentation().treeNodeRemoved += new TreeNodeRemovedEventHandler(TOCView_treeNodeRemoved);
            mView.Project.RenamedSectionNode += new Obi.Events.RenameSectionNodeHandler(Project_RenamedSectionNode);
        }

        /// <summary>
        /// Select a node in the TOC view and start its renaming.
        /// </summary>
        public void SelectAndRenameNode(SectionNode section)
        {
            DoToNewNode(section, delegate()
            {
                TreeNode n = FindTreeNode(section);
                n.BeginEdit();
                mView.Selection = new NodeSelection(section, this);
            });
        }

        /// <summary>
        /// Get or set the current selection. Make sure that the node is indeed in the tree.
        /// </summary>
        public ObiNode Selection
        {
            get
            {
                TreeNode selected = mTOCTree.SelectedNode;
                return selected == null ? null : (ObiNode)selected.Tag;
            }
            set
            {
                if ((mTOCTree.SelectedNode == null && value != null) ||
                    (mTOCTree.SelectedNode != null && value != mTOCTree.SelectedNode.Tag))
                {
                    mTOCTree.SelectedNode = value == null ? null : FindTreeNode((SectionNode)value);
                }
            }
        }

        /// <summary>
        /// Get the selected section, or null if nothing is selected.
        /// </summary>
        public SectionNode SelectedSection
        {
            get { return Selection as SectionNode; }
        }

        /// <summary>
        /// Select a node in the TOC view.
        /// </summary>
        public void SelectNode(SectionNode section)
        {
            DoToNewNode(section, delegate() { mView.Selection = new NodeSelection(section, this); });
        }


        // When a node was renamed, show the new name in the tree.
        private void Project_RenamedSectionNode(object sender, Obi.Events.Node.RenameSectionNodeEventArgs e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Label;
        }

        // Rename the section after the text of the tree node has changed.
        // Cancel if the text is empty.
        private void mTOCTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null && e.Label != "")
            {
                mView.RenameSectionNode(e.Node.Tag as SectionNode, e.Label);
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        // Add new section nodes to the tree
        private void TOCView_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null)
            {
                TreeNode n = AddSingleSectionNode(section);
                n.ExpandAll();
                n.EnsureVisible();
            }
        }

        // Remove deleted section nodes from the tree
        void TOCView_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null) mTOCTree.Nodes.Remove(FindTreeNode(section));
        }


        // Convenience method to add a new tree node for a section. Return the added tree node.
        private TreeNode AddSingleSectionNode(SectionNode section)
        {
            TreeNode n;
            if (section.ParentSection != null)
            {
                TreeNode p = FindTreeNode(section.ParentSection);
                n = p.Nodes.Insert(section.Index, section.GetHashCode().ToString(), section.Label);
            }
            else
            {
                n = mTOCTree.Nodes.Insert(section.Index, section.GetHashCode().ToString(), section.Label);
            }
            n.Tag = section;
            return n;
        }


        /// <summary>
        /// Find the tree node for a section node. The labels must also match.
        /// </summary>
        private TreeNode FindTreeNode(SectionNode section)
        {
            TreeNode n = FindTreeNodeWithoutLabel(section);
            if (n.Text != section.Label)
            {
                throw new TreeNodeFoundWithWrongLabelException(
                    String.Format("Found tree node matching section node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\").",
                    section.GetHashCode(), section.Label, n.Text));
            }
            return n;
        }

        /// <summary>
        /// Find a tree node for a section node, regardless of its label.
        /// The node that we are looking for must be present, but its label
        /// may not be the same.
        /// </summary>
        private TreeNode FindTreeNodeWithoutLabel(SectionNode section)
        {
            TreeNode[] nodes = mTOCTree.Nodes.Find(section.GetHashCode().ToString(), true);
            foreach (TreeNode n in nodes)
            {
                if (n.Tag == section) return n;
            }
            throw new TreeNodeNotFoundException(
                String.Format("Could not find tree node matching section node #{0} with label \"{1}\".",
                    section.GetHashCode(), Project.GetTextMedia(section).getText()));
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the tree.
        private void DoToNewNode(SectionNode section, DoToNewNodeDelegate f)
        {
            try
            {
                f();
            }
            catch (TreeNodeNotFoundException _e)
            {
                TreeNodeAddedEventHandler h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e) { };
                h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == section)
                    {
                        f();
                        mView.Project.getPresentation().treeNodeAdded -= h;
                    }
                };
                mView.Project.getPresentation().treeNodeAdded += h;
            }
        }
    }

    /// <summary>
    /// Raised when a tree node could not be found.
    /// </summary>
    public class TreeNodeNotFoundException : Exception
    {
        public TreeNodeNotFoundException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Raised when a tree node is found but with the wrong label.
    /// </summary>
    public class TreeNodeFoundWithWrongLabelException: Exception
    {
        public TreeNodeFoundWithWrongLabelException(string msg) : base(msg) { }
    }
}
