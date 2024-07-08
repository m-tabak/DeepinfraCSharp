namespace DeepinfraCSharp
{
    /// <summary>
    /// A class that represnts a human-AI interaction.
    /// </summary>
    public class Interaction
    {
        /// <summary>
        /// The question or input from a human.
        /// </summary>
        public string Input { get; set; } = "";

        /// <summary>
        /// The answer or response from AI.
        /// </summary>
        public string Output { get; set; } = "";

        internal string FormatInteraction(Model model)
        {
            string result ="";
            switch (model.PromptStyle)
            {
                case PromptFormat.Airoboros:
                    result = $"USER: {Input}\nASSISTANT: {Output}\n";
                    break;
                case PromptFormat.Llama2:
                    result = $"[INST] {Input} [/INST] {Output}\n";
                    break;
                case PromptFormat.Alpaca:
                    result = $"### Instruction:\n\n{Input}\n\n### Response: {Output}\n";
                    break;
                case PromptFormat.Llama3:
                    result = $"<|start_header_id|>user<|end_header_id|>\n{Input}<|eot_id|><|start_header_id|>assistant<|end_header_id|>\n{Output}<|eot_id|>";
                    break;
            }
            return result;
        }
    }
}
