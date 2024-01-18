using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepinfraCSharp
{
    /// <summary>
    /// A class that represnts an LLM prompt
    /// </summary>
    public class PromptSettings
    {
        /// <summary>
        /// Examples to give in the prompt.
        /// </summary>
        public List<Interaction> InteractionExamples { get; set; } = new();

        /// <summary>
        /// A special messages to steer the behavior of the large langauge model.
        /// </summary>
        public string SystemPrompt { get; set; } = string.Empty;
    }
}
