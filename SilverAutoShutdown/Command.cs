using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAutoShutdown
{
    public class Command : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ShutdownTimer";

        public string Help => "Shuts down the server with a timer specified in the config";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "shutdowntimer" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            SilverAutoShutdown.Instance.StartShutdown();
        }
    }
}
