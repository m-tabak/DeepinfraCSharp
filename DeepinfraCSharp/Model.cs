using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepinfraCSharp
{
    internal enum PromptFormat
    {
        Llama2,
        Airoboros,
        Alpaca,
        Llama3
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
        /// A merge of MythoMix, my MythoLogic-L2 and Huginn. This model is proficient at both roleplaying and storywriting.
        /// </summary>
        public static Model MythoMax_13b => new Model() { UriPath = "Gryphe/MythoMax-L2-13b", PromptStyle = PromptFormat.Alpaca};

        /// <summary>
        /// A multi-model merge of several LLaMA2 70B finetunes for roleplaying and creative work. 
        /// </summary>
        public static Model Lzlv_70b => new Model() { UriPath = "lizpreciatior/lzlv_70b_fp16_hf", PromptStyle = PromptFormat.Airoboros};

        /// <summary>
        /// Meta Llama 3 instruction tuned generative text model in 70B size.
        /// </summary>
        public static Model Llama3_70B => new Model() { UriPath = "meta-llama/Meta-Llama-3-70B-Instruct", PromptStyle = PromptFormat.Llama3 };

    }
}
