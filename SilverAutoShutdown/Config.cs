using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAutoShutdown
{
    public class Config : IRocketPluginConfiguration
    {
        public bool EnableUi;
        public ushort UiId;
        public string UiString1;
        public string UiString2;
        public string ChatMessage;
        public int RebootTimeHourUTC;
        public int RebootTimeMinuteUTC;
        public string KickMessage;
        public int AlertDelay;
        public int NumAlerts;
        public int DontRebootIfStartedWithinHours;
        public string MessageColor;
        public void LoadDefaults()
        {
            EnableUi = true;
            UiId = 28002;
            UiString1 = "Server Rebooting Soon!";
            UiString2 = "The server will reboot in {0} seconds";
            ChatMessage = "The server will reboot in {0} seconds";
            RebootTimeHourUTC = 8;
            RebootTimeMinuteUTC = 0;
            KickMessage = "Server is rebooting. Be back soon!";
            AlertDelay = 15;
            NumAlerts = 4;
            DontRebootIfStartedWithinHours = 12;
            MessageColor = "ffff00";
        }
    }
}
