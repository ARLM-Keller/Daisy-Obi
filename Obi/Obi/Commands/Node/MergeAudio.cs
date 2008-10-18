using urakawa.media.timing;
using urakawa.command;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Basic merge commands to merge the two nodes given as argument.
    /// Does not preserve node attributes (used, TODO, etc.) so use as
    /// part of a composite command.
    /// </summary>
    class MergeAudio: Command
    {
        private PhraseNode mNode;          // the selected phrase
        private PhraseNode mNextNode;      // the following phrase to merge with
        private Time mSplitTime;           // the split time of the new merged node


        private MergeAudio(ProjectView.ProjectView view, PhraseNode node, PhraseNode next)
            : base(view)
        {
            mNode = node;
            mNextNode = next;
            mSplitTime = new urakawa.media.timing.Time(mNode.Audio.getDuration().getTimeDeltaAsMillisecondFloat());
        }


        public static CompositeCommand GetMergeCommand(ProjectView.ProjectView view)
        {
            EmptyNode node = view.SelectedNodeAs<EmptyNode>();
            return GetMergeCommand(view, node, node.getNextSibling() as EmptyNode);
        }

        public static CompositeCommand GetMergeCommand(ProjectView.ProjectView view, EmptyNode node, EmptyNode next)
        {
            if (node != null && next != null)
            {
                CompositeCommand command =
                    view.Presentation.CreateCompositeCommand(Localizer.Message("merge_phrase_with_next"));
                if (node is PhraseNode)
                {
                    AppendCopyNodeAttributes(command, view, next, node);
                    if (next is PhraseNode)
                    {
                        command.append(new Commands.Node.MergeAudio(view, (PhraseNode)node, (PhraseNode)next));
                    }
                    else
                    {
                        command.append(new Commands.Node.Delete(view, next));
                    }
                }
                else
                {
                    AppendCopyNodeAttributes(command, view, node, next);
                    command.append(new Commands.Node.Delete(view, node));
                }
                return command;
            }
            return null;
        }
       
        /// <summary>
        /// Append commands to transfer the attributes of a node to another (used, TODO, role, page number)
        /// </summary>
        public static void AppendCopyNodeAttributes(CompositeCommand command, ProjectView.ProjectView view,
            EmptyNode from, EmptyNode to)
        {
            if (from.TODO && !to.TODO) command.append(new Commands.Node.ToggleNodeTODO(view, to));
            if (!from.Used && to.Used) command.append(new Commands.Node.ToggleNodeUsed(view, to));
            if (from.Role_ == EmptyNode.Role.Page)
            {
                command.append(new Commands.Node.SetPageNumber(view, to, from.PageNumber.Clone()));
            }
            else if (from.Role_ != EmptyNode.Role.Plain &&
                (from.Role_ != EmptyNode.Role.Silence || to is PhraseNode))
            {
                command.append(new Commands.Node.AssignRole(view, to, from.Role_, from.CustomRole));
            }
        }

        /// <summary>
        /// Merge the selected phrase with the following phrase.
        /// </summary>
        //public MergeAudio(ProjectView.ProjectView view):
        //    this(view, (PhraseNode)view.Selection.Node.ParentAs<ObiNode>().PhraseChild(view.Selection.Node.Index + 1)) {}

        /// <summary>
        /// Merge two nodes; the "next" one is removed after merging.
        /// </summary>
        public static void Merge(ProjectView.ProjectView view, PhraseNode node, PhraseNode next, bool updateSelection)
        {
            next.Detach();
            node.MergeAudioWith(next.Audio);
            if (updateSelection) view.SelectedBlockNode = node;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
        }

        public override void execute()
        {
            Merge(View, mNode, mNextNode, UpdateSelection);
        }

        public override void unExecute()
        {
            SplitAudio.Split(View, mNode, mNextNode, mSplitTime, UpdateSelection);
            base.unExecute();
        }
    }
}
