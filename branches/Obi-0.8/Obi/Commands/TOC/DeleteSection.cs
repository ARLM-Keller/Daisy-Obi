using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class DeleteSectionNode : Command
    {
        protected SectionNode mNode;  // the deleted section node
        protected CoreNode mParent;   // parent of the deleted node (root or section node)
  
        public override string Label
        {
            get { return Localizer.Message("delete_section_command_label"); }
        }

        public DeleteSectionNode(SectionNode node)
        {
            mNode = node;
            mParent = (CoreNode)node.getParent();
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            mNode.Project.RemoveSectionNode(mNode.Project, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndeleteSectionNode(mNode, mParent);
        }
    }
}
