using System;
using System.Collections.Generic;

using urakawa.command;
using urakawa.media.data.audio;
using urakawa.media.data;
using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class SplitAudio: Command
    {
        private PhraseNode mNode;       // node to split
        private PhraseNode mNodeAfter;  // node after split point
        private Time mSplitTime;        // split point (begin/cursor)


        // Create a split command to split the selected node at the given position.
        // Use only through GetSplitCommand.
        private SplitAudio(ProjectView.ProjectView view, PhraseNode node, double time): base(view)
        {
            mNode = node;
            mNodeAfter = view.Presentation.CreatePhraseNode();
            mSplitTime = new Time(time);
            Label = Localizer.Message("split_phrase");
        }

        public PhraseNode Node { get { return mNode; } }
        public PhraseNode NodeAfter { get { return mNodeAfter; } }

        /// <summary>
        /// Create a command to crop the current selection.
        /// </summary>
        public static ICommand GetCropCommand(Obi.ProjectView.ProjectView view)
        {
            PhraseNode phrase = view.SelectedNodeAs<PhraseNode>();
            if (phrase != null)
            {
                double begin = view.TransportBar.SplitBeginTime;
                double end = view.TransportBar.SplitEndTime;
                if (end >= phrase.Duration) end = 0.0;
                CompositeCommand command = view.Presentation.CreateCompositeCommand(Localizer.Message("crop_audio"));
                if (begin > 0.0 || end > 0.0)
                {
                    SplitAudio split;         // actual split command
                    PhraseNode after = null;  // node to select after splitting
                    if (end > 0.0)
                    {
                        split = AppendSplitCommandWithProperties(view, command, phrase, end, false);
                        after = split.Node;
                        command.append(new Commands.Node.DeleteWithOffset(view, phrase, 1));
                    }
                    if (begin > 0.0)
                    {
                        split = AppendSplitCommandWithProperties(view, command, phrase, begin,
                            view.Selection is AudioSelection && !((AudioSelection)view.Selection).AudioRange.HasCursor);
                        after = split.NodeAfter;
                        command.append(new Commands.Node.Delete(view, phrase));
                    }
                    command.append(new Commands.UpdateSelection(view, new NodeSelection(after, view.Selection.Control)));
                    return command;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first node after the split created by a split command.
        /// </summary>
        public static PhraseNode GetSplitNode(ICommand command)
        {
            CompositeCommand c = command as CompositeCommand;
            SplitAudio split = command as SplitAudio;
            if (c != null)
            {
                System.Collections.Generic.List<ICommand> commands = c.getListOfCommands();
                int i = commands.Count - 1;
                for (; i >= 0 && !(commands[i] is SplitAudio); --i) { }
                if (i >= 0) split = commands[i] as SplitAudio;
            }
            return split == null ? null : split.NodeAfter;
        }

        /// <summary>
        /// Get the node after the split node created by a crop command.
        /// </summary>
        public static PhraseNode GetCropNode(ICommand command, PhraseNode splitNode)
        {
            CompositeCommand c = command as CompositeCommand;
            SplitAudio split = null;
            if (c != null)
            {
                System.Collections.Generic.List<ICommand> commands = c.getListOfCommands();
                int i = 0;
                for (; i < commands.Count && !((commands[i] is SplitAudio) && ((SplitAudio)commands[i]).NodeAfter != splitNode);
                    ++i) { }
                if (i < commands.Count) split = commands[i] as SplitAudio;
            }
            return split == null ? null : split.NodeAfter;
        }

        /// <summary>
        /// Create a split command for the current selection.
        /// </summary>
        public static CompositeCommand GetSplitCommand(ProjectView.ProjectView view)
        {
            PhraseNode phrase = view.TransportBar.PlaybackPhrase;
            if (phrase == null) phrase = view.SelectedNodeAs<PhraseNode>();
            if (phrase != null)
            {
                double begin = view.TransportBar.SplitBeginTime;
                double end = view.TransportBar.SplitEndTime;
                if (end >= phrase.Duration) end = 0.0;
                if (begin > 0.0 || end > 0.0)
                {
                    CompositeCommand command =
                        view.Presentation.CreateCompositeCommand(Localizer.Message("split_phrase"));
                    if (end > 0.0) AppendSplitCommandWithProperties(view, command, phrase, end, false);
                    if (begin > 0.0) AppendSplitCommandWithProperties(view, command, phrase, begin,
                        view.Selection is AudioSelection && !((AudioSelection)view.Selection).AudioRange.HasCursor);
                    if (command.getCount() > 0) return command;
                }
            }
            return null;
        }

        /// <summary>
        /// Create the phrase detection command.
        /// </summary>
        public static CompositeCommand GetPhraseDetectionCommand(ProjectView.ProjectView view, PhraseNode phrase,
            long threshold, double gap, double before)
        {
            CompositeCommand command = view.Presentation.CreateCompositeCommand(Localizer.Message("phrase_detection"));
            ObiNode parent = phrase.ParentAs<ObiNode>();
            int index = phrase.Index;
            command.append(new Commands.Node.Delete(view, phrase, false));
            System.Collections.Generic.List<PhraseNode> phrases = view.Presentation.CreatePhraseNodesFromAudioAssetList(
                Obi.Audio.PhraseDetection.Apply(phrase.Audio.copy(), threshold, gap, before));
            for (int i = 0; i < phrases.Count; ++i)
            {
                // Copy page/heading role for the first phrase only
                if (i == 0 || (phrase.Role_ != EmptyNode.Role.Page && phrase.Role_ != EmptyNode.Role.Heading))
                {
                    phrases[i].CopyAttributes(phrase);
                }
                phrases[i].Used = phrase.Used;
                phrases[i].TODO = phrase.TODO;
                if (phrases[i].Role_ == EmptyNode.Role.Heading && i > 0) phrases[i].Role_ = EmptyNode.Role.Plain;

// in following add node constructor, update selection is made false, to improve performance (19 may, 2010)
                command.append(new Commands.Node.AddNode(view, phrases[i], parent, index + i, false));
            }
            return command;
        }

        // Create a split command preserving used/TODO status, and optionally transferring the role to the next node
        private static SplitAudio AppendSplitCommandWithProperties(ProjectView.ProjectView view, CompositeCommand command,
            PhraseNode phrase, double time, bool transferRole)
        {
            SplitAudio split = new SplitAudio(view, phrase, time);
            if (split.Node.TODO) command.append(new Commands.Node.ToggleNodeTODO(view, split.NodeAfter));
            if (!split.Node.Used) command.append(new Commands.Node.ToggleNodeUsed(view, split.NodeAfter));
            if (split.Node.Role_ == EmptyNode.Role.Silence)
            {
                Commands.Node.AssignRole silence =
                    new Commands.Node.AssignRole(view, split.NodeAfter, EmptyNode.Role.Silence);
                silence.UpdateSelection = false;
                command.append(silence);
            }
            command.append(split);
            if (transferRole && phrase.Role_ != EmptyNode.Role.Plain)
            {
                command.append(new Commands.Node.AssignRole(view, phrase, EmptyNode.Role.Plain));
                if (phrase.Role_ == EmptyNode.Role.Page)
                {
                    command.append(new Commands.Node.SetPageNumber(view, split.NodeAfter, phrase.PageNumber.Clone()));
                }
                else
                {
                    command.append(new Commands.Node.AssignRole(view, split.NodeAfter, phrase.Role_, phrase.CustomRole));
                }
            }
            return split;
        }

        // Perform a split given a time and a node with no audio, optionally updating the selection in the view afterward.
        public static void Split(ProjectView.ProjectView view, PhraseNode node, PhraseNode nodeAfter, Time splitTime,
            bool updateSelection)
        {
            nodeAfter.Audio = node.SplitAudio(splitTime);
            node.InsertAfterSelf(nodeAfter);
            if (updateSelection) view.SelectedBlockNode = nodeAfter;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
        }

        public override List<MediaData> getListOfUsedMediaData ()
            {
            List<MediaData> mediaList = new List<MediaData> ();

            if (mNodeAfter != null && mNodeAfter is PhraseNode && mNodeAfter.Audio != null)
                mediaList.Add ( mNodeAfter.Audio.getMediaData () );

            return mediaList;
            }


        public override void execute()
        {
            Split(View, mNode, mNodeAfter, mSplitTime, UpdateSelection);
        }

        public override void unExecute()
        {
            MergeAudio.Merge(View, mNode, mNodeAfter, UpdateSelection);
            base.unExecute();
        }
    }
}
