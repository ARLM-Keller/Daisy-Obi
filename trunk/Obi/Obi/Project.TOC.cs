using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi
{
    public partial class Project
    {
        public event Events.SectionNodeHandler AddedSectionNode;      // a section node was added to the TOC
        public event Events.RenameSectionNodeHandler RenamedSectionNode;                // a node was renamed in the presentation
        public event Events.MovedSectionNodeHandler MovedSectionNode;                    // a node was moved in the presentation
        public event Events.SectionNodeHandler DecreasedSectionNodeLevel;  // a node's level was decreased in the presentation
        public event Events.MovedSectionNodeHandler UndidMoveSectionNode;                // a node was restored to its previous location
        public event Events.SectionNodeHandler DeletedSectionNode;                // a node was deleted from the presentation
        public event Events.SectionNodeHandler CutSectionNode;
        public event Events.SectionNodeHandler PastedSectionNode;
        public event Events.SectionNodeHandler UndidPasteSectionNode;
        public event Events.SectionNodeHandler ToggledSectionUsedState;
      
        // Here are the event handlers for request sent by the GUI when editing the TOC.
        // Every request is passed to a method that uses mostly the same arguments,
        // which can also be called directly by a command for undo/redo purposes.
        // When we are done, a synchronization event is sent back.
        // (As well as a state change event.)

        /// <summary>
        /// Create a sibling section for a given section.
        /// The context node may be null if this is the first node that is added, in which case
        /// we add a new child to the root (and not a sibling.)
        /// </summary>
        /// <returns>The new section node.</returns>
        public SectionNode CreateSiblingSectionNode(object origin, SectionNode contextNode)
        {
            CoreNode parent = (CoreNode)(contextNode == null ? getPresentation().getRootNode() : contextNode.getParent());
            SectionNode sibling = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
            if (contextNode == null)
            {
                // first node ever
                parent.appendChild(sibling);
            }
            else
            {
                AddChildSectionAfter(sibling, contextNode, parent);
            }
            AddedSectionNode(origin, new Events.Node.SectionNodeEventArgs(origin, sibling));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(sibling);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            return sibling;
        }

        public void CreateSiblingSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            CreateSiblingSectionNode(sender, e.Node);
        }

        /// <summary>
        /// Create a new child section for a given section. If the context node is null, add to the root of the tree.
        /// </summary>
        public SectionNode CreateChildSectionNode(object origin, CoreNode parent)
        {
            SectionNode child = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
            
            if (parent == null)
            {
                getPresentation().getRootNode().appendChild(child);
            }
            else
            {
                AppendChildSection(child, parent);
            }
            AddedSectionNode(origin, new Events.Node.SectionNodeEventArgs(origin, child));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(child);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));

            return child;
        }

        public void CreateChildSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            CreateChildSectionNode(sender, e.Node);
        }

        /// <summary>
        /// Add a section that had previously been added.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="originalLabel"></param>
        public void AddExistingSectionNode(SectionNode node, CoreNode parent, int index, string originalLabel)
        {
            if (node.getParent() == null) AddChildSection(node, parent, index);

            if (originalLabel != null) Project.GetTextMedia(node).setText(originalLabel);
 
            AddedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Readd a section node that was previously delete and restore all its contents.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        public void UndeleteSectionNode(SectionNode node, CoreNode parent, int index)
        {
            Visitors.UndeleteSubtree visitor = new Visitors.UndeleteSubtree(this, parent, index);
            node.acceptDepthFirst(visitor);
        }

        /// <summary>
        /// Remove a node from the core tree. It is detached from the tree and the
        /// </summary>
        //md
        //the command value is returned so it can be used in UndoShallowDelete's undo list
        public Commands.Command RemoveSectionNode(object origin, SectionNode node)
        {
            Commands.TOC.DeleteSectionNode command = null;
            if (node != null)
            {
                //md: need this particular command to be created even if origin = this
                //because its undo fn is required by UndoShallowDelete
                //if (origin != this)
               // {
                    CoreNode parent = (CoreNode)node.getParent();
                    command = new Commands.TOC.DeleteSectionNode(node);
              //  }
                node.DetachFromParent();
                DeletedSectionNode(this, new Events.Node.SectionNodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

                //md: added condition "origin != this" to accomodate the change made above
                if (command != null && origin != this) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }

            return command;
        }

        public void RemoveSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            RemoveSectionNode(sender, e.Node);
        }

        /// <summary>
        /// reposition the node at the index under its given parent
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void UndoMoveSectionNode(SectionNode node, CoreNode parent, int index)
        {
            if (node.getParent() != null) node.DetachFromParent();
            AddChildSection(node, parent, index);
           
            UndidMoveSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, parent));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Undo increase level
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="node"></param>
        //added by marisa 01 aug 06
        public void UndoIncreaseSectionNodeLevel(SectionNode node, CoreNode parent, int index)
        {
            UndoMoveSectionNode(node, parent, index);
        }

        public void IncreaseSectionNodeLevel(object origin, SectionNode node)
        {
            Commands.TOC.IncreaseSectionNodeLevel command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.IncreaseSectionNodeLevel(node, parent);
            }

            bool succeeded = ExecuteIncreaseSectionNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();
                MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, newParent));

                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  // JQ
            }

        }

        /// <summary>
        /// Move the node "in", i.e. increase its level.
        /// </summary>
        /// <param name="node">The node to move in.</param>
        /// <returns>True on success</returns>
        private bool ExecuteIncreaseSectionNodeLevel(SectionNode node)
        {
            if (node.Index == 0)
            {
                //can't increase section level if the node has no "older" siblings
                return false;
            }
            else
            {
                SectionNode newParent;
                SectionNode movedNode;
                if (node.ParentSection == null)
                {
                    //assumption: the root only has section node children, so we can do this
                    newParent = (SectionNode)((CoreNode)node.getParent()).getChild(node.Index - 1);
                    movedNode = (SectionNode)node.DetachFromParent();
                }
                else
                {
                    //else the node's parent is an ordinary section node
                    newParent = node.ParentSection.SectionChild(node.Index - 1);
                    movedNode = (SectionNode)node.DetachFromParent();
                }
                AppendChildSection(movedNode, newParent);                
                return true;
            }
        }
       
        public void IncreaseSectionNodeLevelRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            IncreaseSectionNodeLevel(sender, e.Node);
        }

        //md
        //the command value is returned so it can be used in UndoShallowDelete's undo list
        public Commands.Command DecreaseSectionNodeLevel(object origin, SectionNode node)
        {
            Commands.TOC.DecreaseSectionNodeLevel command = null;

            //md: need this particular command to be created even if origin = this
            //because its undo fn is required by UndoShallowDelete
            //if (origin != this)
            //{
                CoreNode parent = (CoreNode)node.getParent();
              
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.DecreaseSectionNodeLevel(node, parent);
                    
            //}

            bool succeeded = ExecuteDecreaseSectionNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();
                DecreasedSectionNodeLevel(this, new Events.Node.SectionNodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

                //md: added condition "origin != this" to accomodate the change made above
                if (command != null && origin != this) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }

            return command;
        }

        /// <summary>
        /// move the node "out"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool ExecuteDecreaseSectionNodeLevel(SectionNode node)
        {
            //the only reason we can't decrease the level is if the node is already 
            //at the outermost level
            if (node.getParent() == null ||
                node.getParent().Equals(node.getPresentation().getRootNode()))
            {
                return false;
            }

            List<SectionNode> futureChildren = new List<SectionNode>();
            int nodeIndex = node.Index;

            int numSectionChildren = ((SectionNode)node.getParent()).SectionChildCount;

            //make copies of our future children, and remove them from the tree
            for (int i = numSectionChildren - 1; i > nodeIndex; i--)
            {
                SectionNode sectionChild;
                sectionChild = (SectionNode)((SectionNode)node.getParent()).SectionChild(i).DetachFromParent();
                futureChildren.Add(sectionChild);
            }
            //since the list was built in backwards order, rearrange it
            futureChildren.Reverse();

            CoreNode newParent = (CoreNode)node.getParent().getParent();
            
            //the index is relative to sections
            int newIndex = ((SectionNode)node.getParent()).Index + 1;

            SectionNode clone = (SectionNode)node.DetachFromParent();

            AddChildSection(clone, newParent, newIndex);

            foreach (SectionNode childnode in futureChildren)
            {
                AppendChildSection(childnode, clone);
            }

            return true;

        }

        public void DecreaseSectionNodeLevelRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            DecreaseSectionNodeLevel(sender, e.Node);
        }

        /// <summary>
        /// undo decrease node level is a bit tricky because we might have to move some of the node's
        /// children out a level, but only if they were newly adopted after the decrease level action happened. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="childCount">number of children this node used to have before the decrease level action happened</param>
        public void UndoDecreaseSectionNodeLevel(SectionNode node, CoreNode parent, int originalChildCount)
        {
            //error-checking
            if (node.getChildCount() < originalChildCount)
            {
                //this would be a pretty strange thing to have happen.
                //todo: throw an exception?  
                return;
            }

            //detach the non-original children (child nodes originalChildCount...n-1)
            List<SectionNode> nonOriginalChildren = new List<SectionNode>();
            int totalNumChildren = node.getChildCount();

            for (int i = totalNumChildren - 1; i >= originalChildCount; i--)
            {
                if (node.getChild(i).GetType() == Type.GetType("Obi.SectionNode"))
                {
                    SectionNode child = (SectionNode)node.getChild(i);
                    if (child != null)
                    {
                        nonOriginalChildren.Add(child);
                        child.DetachFromParent();
                    }
                }
            }

            //this array was built backwards, so reverse it
            nonOriginalChildren.Reverse();

            //insert the node back in its old location
            node.DetachFromParent();
           
            AppendChildSection(node, parent);

            MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, parent));

      
            //reattach the children
            for (int i = 0; i < nonOriginalChildren.Count; i++)
            {
                AppendChildSection(nonOriginalChildren[i], parent);
                MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs
                    (this, nonOriginalChildren[i], parent));
            }
            
        }

        /// <summary>
        /// Change the text label of a node.
        /// </summary>
        public void RenameSectionNode(object origin, SectionNode node, string label)
        {
         
            TextMedia text = GetTextMedia(node);
            Commands.TOC.Rename command = origin == this ? null : new Commands.TOC.Rename(this, node, text.getText(), label);
            GetTextMedia(node).setText(label);
            RenamedSectionNode(this, new Events.Node.RenameSectionNodeEventArgs(origin, node, label));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void RenameSectionNodeRequested(object sender, Events.Node.RenameSectionNodeEventArgs e)
        {
            RenameSectionNode(sender, e.Node, e.Label);
        }

        //this is almost the same as ShallowDeleteSectionNode
        public void DoShallowCutSectionNode(object origin, SectionNode node)
        {
            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            mClipboard.Section = node.copy(false);
            Commands.TOC.ShallowCutSectionNode command = origin == this ?
                null : new Commands.TOC.ShallowCutSectionNode(node);
            int numChildren = node.SectionChildCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                Commands.Command cmdDecrease = DecreaseSectionNodeLevel(this, node.SectionChild(i));
                if (command != null) command.AddCommand(cmdDecrease);
            }
            numChildren = node.PhraseChildCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                Commands.Command cmdDeletePhrase = RemovePhraseNodeAndAsset(node.PhraseChild(i));
                if (command != null) command.AddCommand(cmdDeletePhrase);
            }
            Commands.Command cmdRemove = RemoveSectionNode(this, node);
            if (command != null) command.AddCommand(cmdRemove);
            Modified();
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void _CutSectionNode(SectionNode node, bool issueCommand)
        {
            if (node != null)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Commands.TOC.CutSectionNode command = new Commands.TOC.CutSectionNode(node);
                mClipboard.Section = node;
                node.DetachFromParent();
                CutSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
                Modified();
                if (issueCommand) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        //md 20061220
        public void UndoShallowCutSectionNode()
        {
            mClipboard.Section = null;
            //the rest of the undo operations for this command are found in 
            //Commands.TOC.ShallowCutSectionNode
        }

        //md 20060810
        public void UndoCutSectionNode(SectionNode node, CoreNode parent, int index)
        {
            UndeleteSectionNode(node, parent, index);
            mClipboard.Section = null;
        }

        //md 20061220
        internal void UndoShallowCopySectionNode(SectionNode node)
        {
            mClipboard.Section = null;

            //no event is raised (i.e. UndidShallowCopySectionNode) because no one cares

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
     
        }

        /// <summary>
        /// Copy a section node as a whole (i.e. the entire subtree.)
        /// The project is unmodified, only the clipboard gets updated.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void CopySectionNode(SectionNode node, bool issueCommand)
        {
            if (node != null)
            {
                object data = mClipboard.Data;
                mClipboard.Section = node.copy(true);
                if (issueCommand)
                {
                    CommandCreated(this,
                        new Events.Project.CommandCreatedEventArgs(new Commands.TOC.CopySectionNode(data, mClipboard.Section)));
                }
            }
        }

        //md 20060810
        // modified by JQ 20060818: can paste under the root node if we have deleted the first and only heading.
        public void PasteSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            if (e.Node != null)
            {
                PasteSectionNode(sender, e.Node);
            }
            else
            {
                PasteSectionNode(sender, getPresentation().getRootNode());
            }
        }

        public void PasteSectionNode(object origin, CoreNode parent)
        {
            PasteSectionNode(parent, origin != this);
        }

        /// <summary>
        /// Paste the clipboard node under the parent (or the root if no parent is given.)
        /// </summary>
        /// <param name="parent">Parent to paste under (root if null.)</param>
        /// <param name="issueCommand">If true, a command is issued.</param>
        public void PasteSectionNode(CoreNode parent, bool issueCommand)
        {
            if (mClipboard.Section != null)
            {
                if (parent == null) parent = RootNode;
                SectionNode pastedSection = mClipboard.Section;
                mClipboard.Section = mClipboard.Section.copy(true);
                AppendChildSection(pastedSection, parent);
                Obi.Visitors.CopyPhraseAssets assVisitor = new Obi.Visitors.CopyPhraseAssets(mAssManager, this);
                pastedSection.acceptDepthFirst(assVisitor);
                PastedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, pastedSection));
                Modified();
                if (issueCommand)
                {
                    CommandCreated(this,
                        new Events.Project.CommandCreatedEventArgs(new Commands.TOC.PasteSectionNode(parent, pastedSection)));
                }
            }
        }

        //md 20060810
        public void UndoPasteSectionNode(SectionNode node)
        {
            node.DetachFromParent();
           
            UndidPasteSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

        }

        //md 20060812
        public void ShallowDeleteSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            ShallowDeleteSectionNode(sender, e.Node);
        }

        //md 20060812
        //shallow delete a section node
        //see Commands.TOC.ShallowDeleteSectionNode if you're wondering how the "undo" works
        public void ShallowDeleteSectionNode(object origin, SectionNode node)
        {
            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            //these lines only need to be executed if origin != this
       
            Commands.TOC.ShallowDeleteSectionNode command = null;
        
            if (origin != this)
            {  
                command = new Commands.TOC.ShallowDeleteSectionNode(node);
            }
               
            int numChildren = node.SectionChildCount;
            for (int i = numChildren - 1; i>=0; i--)
            {
                Commands.Command cmdDecrease = this.DecreaseSectionNodeLevel(this, node.SectionChild(i));
                command.AddCommand(cmdDecrease);
            }

            numChildren = node.PhraseChildCount;
            for (int i = numChildren - 1; i>=0; i--)
            {
                Commands.Command cmdDeletePhrase = RemovePhraseNodeAndAsset(node.PhraseChild(i));
                command.AddCommand(cmdDeletePhrase);
            }

            Commands.Command cmdRemove = this.RemoveSectionNode(this, node);
            command.AddCommand(cmdRemove);

          
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
             
        }

        //md 20060813
        internal bool CanMoveSectionNodeIn(SectionNode node)
        {
            //if it's not the first section
            if (node.Index > 0)
                return true;
            else
                return false;
        }

        //md 20060813
        internal bool CanMoveSectionNodeOut(SectionNode node)
        {
            //the only reason we can't decrease the level is if the node is already 
            //at the outermost level
            if (node.getParent() == null ||
                node.getParent().Equals(node.getPresentation().getRootNode()))
            {
                return false;
            }
           
            return true;
        }

        //helper function which tests for parent being root
        //md 20061204
        internal void AddChildSection(SectionNode node, CoreNode parent, int index)
        {
            if (parent.Equals(getPresentation().getRootNode()))
            {
                parent.insert(node, index);
            }
            else if (parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                ((SectionNode)parent).AddChildSection(node, index);
            }          
        }

        //helper function which tests for parent being root
        //md 20061204
        internal void AppendChildSection(SectionNode node, CoreNode parent)
        {
            if (parent.Equals(getPresentation().getRootNode()))
            {
                parent.appendChild(node);
            }
            else if (parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                ((SectionNode)parent).AppendChildSection(node);
            }         
        }

        //helper function which tests for parent being root
        //md 20061204
        internal void AddChildSectionAfter(SectionNode node, SectionNode contextNode, CoreNode parent)
        {
            if (parent.Equals(getPresentation().getRootNode()))
            {
                parent.insertAfter(node, contextNode);
            }
            else if (parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                ((SectionNode)parent).AddChildSectionAfter(node, contextNode);
            }         
        }

        /// <summary>
        /// Toggle the use state of a section node.
        /// </summary>
        internal void ToggleSectionUsedState(Events.Node.SectionNodeEventArgs e)
        {
            e.Node.Used = !e.Node.Used;
            ToggledSectionUsedState(this, e);
        }



        /// <summary>
        /// Shallow cut of a section node, i.e. only the strip.
        /// Sub-sections are moved back one level.
        /// </summary>
        /// <param name="node">The node to cut.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void ShallowCutSectionNode(SectionNode node, bool issueCommand)
        {
            if (node != null)
            {
                Commands.TOC.ShallowCutSectionNode command = new Commands.TOC.ShallowCutSectionNode(node);
                mClipboard.Section = node.copy(false);
                for (int i = node.SectionChildCount - 1; i >= 0; --i)
                {
                    command.AddCommand(DecreaseSectionNodeLevel(this, node.SectionChild(i)));
                }
                for (int i = node.PhraseChildCount - 1; i >= 0; --i)
                {
                    command.AddCommand(RemovePhraseNodeAndAsset(node.PhraseChild(i)));
                }
                command.AddCommand(RemoveSectionNode(this, node));
                Modified();
                if (issueCommand)
                {
                    CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
                }
            }
        }

        /// <summary>
        /// Copy a section node and its phrases, but not its subsections (i.e. copy one strip.)
        /// The project is unmodified, only the clipboard gets updated.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void ShallowCopySectionNode(SectionNode node, bool issueCommand)
        {
            if (node != null)
            {
                object data = mClipboard.Data;
                SectionNode copy = node.copy(false);
                for (int i = 0; i < node.PhraseChildCount; ++i)
                {
                    copy.AppendChildPhrase(node.PhraseChild(i).copy(true));
                }
                mClipboard.Section = copy;
                if (issueCommand)
                {
                    CommandCreated(this,
                        new Events.Project.CommandCreatedEventArgs(new Commands.TOC.CopySectionNode(data, copy)));
                }
            }
        }
    }
}
