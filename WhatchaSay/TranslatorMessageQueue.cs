using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using GSF.Communication;
using WhatchaSay.Model;

namespace WhatchaSay
{
    public class TranslatorMessageQueue
    {

        private volatile bool runQueue = true;

        private readonly Plugin plugin;
        private readonly IClientState state;
        private readonly Thread runnerThread;

        private readonly ConcurrentQueue<QueuedXivEvent> eventQueue = new ConcurrentQueue<QueuedXivEvent>();

        public TranslatorMessageQueue(Plugin plugin)
        {
            this.plugin = plugin;
            this.runnerThread = new Thread(RunMessageQueue);
            this.state = Plugin.State;
        }

        public void Start()
        {
            this.runQueue = true;
            this.runnerThread.Start();
        }

        public void Stop()
        {
            this.runQueue = false;

            if (this.runnerThread.IsAlive)
                this.runnerThread.Join();

        }

        public void Enqueue(QueuedXivEvent @event) => this.eventQueue.Enqueue(@event);

        private async void RunMessageQueue()
        {
            while (this.runQueue)
            {
                if (this.eventQueue.TryDequeue(out var resultEvent))
                {
                    try
                    {

                        if (resultEvent is ChatTranslateItem chatEvent)
                        {
                            var playerPayload = chatEvent.Sender.Payloads.SingleOrDefault(x => x is PlayerPayload) as PlayerPayload;
                            if (chatEvent.ChatType is XivChatType.StandardEmote) playerPayload = chatEvent.Message.Payloads.FirstOrDefault(x => x is PlayerPayload) as PlayerPayload;
                            
                            if (playerPayload == default(PlayerPayload)) await this.plugin.ChatTranslate.Translate(chatEvent.Message.TextValue, state.LocalPlayer.Name.TextValue);
                            else await this.plugin.ChatTranslate.Translate(chatEvent.Message.TextValue, playerPayload.PlayerName);
                        }

                    }
                    catch (Exception e)
                    {
                        PluginLog.Error(e, "Failed to process the event.");
                    }
                }

                Thread.Yield();
            }
        }
    }
}
