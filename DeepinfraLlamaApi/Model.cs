using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepinfraCSharp
{
    public class Model
    {
        private string UriPath = "";
        internal bool IsAiroboros = false;

        public Model() { }

        public static implicit operator string(Model model) => model.UriPath;

        public static Model Llama2_70b => new Model() { UriPath = "meta-llama/Llama-2-70b-chat-hf" };

        public static Model MythoMax_13b => new Model() { UriPath = "Gryphe/MythoMax-L2-13b" };

        public static Model Airoboros_70b => new Model() { UriPath = "deepinfra/airoboros-70b", IsAiroboros = true };
    }
}
