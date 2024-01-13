using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepinfraCSharp
{
    internal enum PromptFormat
    {
        Llama,
        Airoboros,
        Alpaca,
    }

    /// <summary>
    /// A class that contains static properties to represnt models/endpoints.
    /// </summary>
    public class Model
    {
        private string UriPath = "";
        internal PromptFormat PromptStyle;
        
        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        public static implicit operator string(Model model) => model.UriPath;

        /// <summary>
        /// LLaMa 2 is a collections of LLMs trained by Meta. This is the 70B chat optimized version.
        /// </summary>
        public static Model Llama2_70b => new Model() { UriPath = "meta-llama/Llama-2-70b-chat-hf", PromptStyle = PromptFormat.Llama };

        /// <summary>
        /// A merge of MythoMix, my MythoLogic-L2 and Huginn. This model is proficient at both roleplaying and storywriting.
        /// </summary>
        public static Model MythoMax_13b => new Model() { UriPath = "Gryphe/MythoMax-L2-13b", PromptStyle = PromptFormat.Alpaca};

        /// <summary>
        /// A fine-tunned version of llama-2-70b using the Airoboros dataset.
        /// </summary>
        public static Model Airoboros_70b => new Model() { UriPath = "deepinfra/airoboros-70b", PromptStyle = PromptFormat.Airoboros};
    }
}
