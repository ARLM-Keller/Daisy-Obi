using System.Collections.Generic;

namespace Obi.Commands
{
    class ListCommand : Command
    {
        private string mLabel;            // the label that will be shown in the end
        private List<Command> mCommands;  // list of actual commands

        public override string Label { get { return mLabel; } }

        /// <summary>
        /// Create a list of commands from, well, a list of commands.
        /// </summary>
        public ListCommand(string label, List<Command> commands)
        //    : base(Command.Visible)
        {
            mLabel = label + "*";  // just for debugging
            mCommands = commands;
        }

        /// <summary>
        /// Do all commands in order.
        /// </summary>
        public override void Do()
        {
            foreach (Command c in mCommands) c.Do();
        }

        /// <summary>
        /// Undo all commands starting from the last.
        /// </summary>
        public override void Undo()
        {
            mCommands.Reverse();
            foreach (Command c in mCommands) c.Undo();
            mCommands.Reverse();
        }
    }
}