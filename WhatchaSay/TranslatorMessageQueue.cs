using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
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

        private int failed_results = 0;

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

                            if (chatEvent.Message.TextValue == null || chatEvent.Message.TextValue == "")
                                return;

                            if (playerPayload == default(PlayerPayload) && this.plugin.Configuration.Translate_Self == true) await this.plugin.ChatTranslate.Translate(chatEvent.Message.TextValue, state.LocalPlayer.Name.TextValue);
                            else if (playerPayload != default(PlayerPayload)) await this.plugin.ChatTranslate.Translate(chatEvent.Message.TextValue, playerPayload.PlayerName);
                            
                            failed_results = 0;
                        }

                        else if (resultEvent is StringTranslateItem translateEvent)
                        {
                            //PluginLog.Information($"New translation request for: {translateEvent.Message} Some debug text");
                            string TranslatedString = await this.plugin.ChatTranslate.Translate(translateEvent);
                            plugin.TranslationWindow.IsWaiting = false;

                            if (TranslatedString == "" || TranslatedString == null)
                            {
                                TranslatedString = "Error translating text.";
                            }

                            plugin.TranslationWindow.TextOutput = Encoding.UTF8.GetBytes(TranslatedString);
                            if (TranslatedString != "Error translating text.")
                            {
                                plugin.TranslationWindow.CopyResult(TranslatedString);
                                Plugin.Chat.Print(new XivChatEntry { Message = "Copied translation to clipboard.", Type = XivChatType.SystemMessage });
                            }
                            failed_results = 0;
                        }

                    }
                    catch (Exception e)
                    {
                        PluginLog.Error(e, "Failed to process the event.");
                        if (plugin.TranslationWindow.IsWaiting == true)
                          plugin.TranslationWindow.IsWaiting = false;

                        failed_results++;

                        if (failed_results > 5)
                        {
                            plugin.Configuration.Enabled = false;
                            Plugin.Chat.Print(new XivChatEntry { Message = "Failed to get 5 consecutive translations. Please report this error.", Type = XivChatType.SystemMessage });
                        }
                    }
                }

                Thread.Yield();
            }
        }
    }
}
