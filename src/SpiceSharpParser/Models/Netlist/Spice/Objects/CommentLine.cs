﻿namespace SpiceSharpParser.Models.Netlist.Spice.Objects
{
    /// <summary>
    /// A comment line.
    /// </summary>
    public class CommentLine : Statement
    {
        public CommentLine(string text, SpiceLineInfo lineInfo) : base(lineInfo)
        {
            Text = text;
        }

        /// <summary>
        /// Gets or sets the comment line's text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <returns>A clone of the object.</returns>
        public override SpiceObject Clone()
        {
            return new CommentLine(Text, LineInfo);
        }
    }
}