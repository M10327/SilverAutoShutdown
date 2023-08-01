using JetBrains.Annotations;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace SilverAutoShutdown
{
    public class SilverAutoShutdown : RocketPlugin<Config>
    {
        System.Timers.Timer exec;
        Config cfg;
        public UnityEngine.Color MessageColor { get; set; }
        public static SilverAutoShutdown Instance { get; private set; }
        protected override void Load()
        {
            Instance = this;
            cfg = Configuration.Instance;
            MessageColor = (Color)UnturnedChat.GetColorFromHex(cfg.MessageColor);

            TimeSpan day = new TimeSpan(24, 00, 00);    // 24 hours in a day.
            TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));     // The current time in 24 hour format
            TimeSpan activationTime = new TimeSpan(cfg.RebootTimeHourUTC, cfg.RebootTimeMinuteUTC, 0);    // 4 AM

            TimeSpan timeLeftUntilFirstRun = ((day - now) + activationTime);
            if (timeLeftUntilFirstRun.TotalHours > 24)
                timeLeftUntilFirstRun -= new TimeSpan(24, 0, 0);

            if (timeLeftUntilFirstRun.Hours < cfg.DontRebootIfStartedWithinHours)
                timeLeftUntilFirstRun += new TimeSpan(24, 0, 0);

            exec = new System.Timers.Timer(timeLeftUntilFirstRun.TotalMilliseconds);
            exec.Elapsed += TimerElapsed;
            exec.Enabled = true;
            exec.Start();

            Rocket.Core.Logging.Logger.Log($"Rebooting the server in {timeLeftUntilFirstRun}");
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TaskDispatcher.QueueOnMainThread(() => StartShutdown());
        }

        public async void StartShutdown()
        {
            Rocket.Core.Logging.Logger.Log("Initiating Timed Server Shutdown");
            exec.Elapsed -= TimerElapsed;
            var players = Provider.clients;
            var alertsLeft = cfg.NumAlerts;
            short key = (short)(cfg.UiId + 10);
            while (alertsLeft > 0)
            {
                int timeleft = alertsLeft * cfg.AlertDelay;
                ChatManager.serverSendMessage(cfg.ChatMessage.Replace("{0}", timeleft.ToString()), MessageColor);
                if (cfg.EnableUi)
                {
                    foreach (var pl in players)
                    {
                        EffectManager.askEffectClearByID(cfg.UiId, pl.player.channel.GetOwnerTransportConnection());
                        EffectManager.sendUIEffect(cfg.UiId, key, pl.player.channel.GetOwnerTransportConnection(), true, cfg.UiString1, cfg.UiString2.Replace("{0}", timeleft.ToString()));
                    }
                }
                alertsLeft--;
                await Task.Delay(cfg.AlertDelay * 1000);
            }
            TaskDispatcher.QueueOnMainThread(() => ShutDown());
        }

        public void ShutDown()
        {
            Rocket.Core.Logging.Logger.Log("Shutting Server Down");
            var kickPlayers = new List<SteamPlayer>(Provider.clients);
            foreach (var pl in kickPlayers)
            {
                UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(pl);
                if (p != null) p.Kick(cfg.KickMessage);
            }
            Provider.shutdown();
        }

        protected override void Unload()
        {
            exec.Elapsed -= TimerElapsed;
        }
    }
}
