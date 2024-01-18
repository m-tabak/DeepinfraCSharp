using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepinfraCSharp
{
    /// <summary>
    /// A class that reprsents chat mode.
    /// </summary>
    public class ChatInformation
    {
        internal List<Interaction> conversation = new ();

        /// <summary>
        /// Whether to enable chat mode. Defult value is 'false'.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// A <see cref="List{T}"/> T is <see cref="Interaction"/>. The interactions in the current conversation.
        /// </summary>
        public IEnumerable<Interaction> Conversation => new List<Interaction>(conversation);

        /// <summary>
        /// Removes the last interacton from <see cref="Conversation"/>.
        /// </summary>
        public void RemoveLastInteration()
        {
            if (conversation.Count > 0)
                conversation.RemoveAt(conversation.Count - 1);
        }

    }
}
