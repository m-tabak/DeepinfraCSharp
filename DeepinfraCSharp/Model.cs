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

    public class Model
    {
        private string UriPath = "";
        internal PromptFormat PromptStyle;

        public Model() { }

        public static implicit operator string(Model model) => model.UriPath;

        public static Model Llama2_70b => new Model() { UriPath = "meta-llama/Llama-2-70b-chat-hf", PromptStyle = PromptFormat.Llama };

        public static Model MythoMax_13b => new Model() { UriPath = "Gryphe/MythoMax-L2-13b", PromptStyle = PromptFormat.Alpaca};

        public static Model Airoboros_70b => new Model() { UriPath = "deepinfra/airoboros-70b", PromptStyle = PromptFormat.Airoboros};
    }
}
