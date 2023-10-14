using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                            var senderName = (chatEvent.ChatType == XivChatType.TellOutgoing || chatEvent.ChatType == XivChatType.Echo)
                                ? state.LocalPlayer.Name
                                : chatEvent.Sender.ToString();
                            var senderWorld = string.Empty;

                            // for debugging. Make sure to comment this out for releases.
                            /*
                            PluginLog.Debug($"Type: {chatEvent.ChatType} Sender: {chatEvent.Sender.TextValue} "
                                + $"Message: {chatEvent.Message.TextValue}");
                            */

                            try
                            {
                                if (state.LocalPlayer != null)
                                {
                                    var playerLink = chatEvent.Sender.Payloads.FirstOrDefault(x => x.Type == PayloadType.Player) as PlayerPayload;

                                    if (playerLink == null)
                                    {
                                        // chat messages from the local player do not include a player link, and are just the raw name
                                        // but we should still track other instances to know if this is ever an issue otherwise

                                        // Special case 2 - When the local player talks in party/alliance, the name comes through as raw text,
                                        // but prefixed by their position number in the party (which for local player may always be 1)
                                        if (chatEvent.Sender.TextValue.EndsWith(state.LocalPlayer.Name.TextValue))
                                        {
                                            senderName = state.LocalPlayer.Name;
                                        }
                                        else
                                        {
                                            // Franz is really tired of getting playerlink is null when there shouldn't be a player link for certain things
                                            switch (chatEvent.ChatType)
                                            {
                                                case XivChatType.Debug:
                                                    break;
                                                case XivChatType.Urgent:
                                                    break;
                                                case XivChatType.Notice:
                                                    break;
                                                case XivChatType.TellOutgoing:
                                                    senderName = state.LocalPlayer.Name;
                                                    // senderWorld = this.plugin.Interface.Clientstate.LocalPlayer.HomeWorld.GameData.Name;
                                                    break;
                                                case XivChatType.StandardEmote:
                                                    playerLink = chatEvent.Message.Payloads.FirstOrDefault(x => x.Type == PayloadType.Player) as PlayerPayload;
                                                    senderName = playerLink.PlayerName;
                                                    senderWorld = playerLink.World.Name;
                                                    // we need to get the world here because cross-world people will be assumed local player's otherwise.
                                                    /*
                                                    senderWorld = chatEvent.Message.TextValue.TrimStart(senderName.ToCharArray()).Split(' ')[0];
                                                    if (senderWorld.EndsWith("'s")) // fuck having to do this
                                                        senderWorld = senderWorld.Substring(0, senderWorld.Length - 2);
                                                    */
                                                    break;
                                                case XivChatType.Echo:
                                                    senderName = state.LocalPlayer.Name;
                                                    // senderWorld = this.plugin.Interface.Clientstate.LocalPlayer.HomeWorld.GameData.Name;
                                                    break;
                                                case (XivChatType)61: // NPC Talk
                                                    senderName = chatEvent.Sender.TextValue;
                                                    senderWorld = "NPC";
                                                    break;
                                                case (XivChatType)68: // NPC Announcement
                                                    senderName = chatEvent.Sender.TextValue;
                                                    senderWorld = "NPC";
                                                    break;
                                                default:
                                                    if ((int)chatEvent.ChatType >= 41 && (int)chatEvent.ChatType <= 55) //ignore a bunch of non-chat messages
                                                        break;
                                                    if ((int)chatEvent.ChatType >= 57 && (int)chatEvent.ChatType <= 70) //ignore a bunch of non-chat messages
                                                        break;
                                                    if ((int)chatEvent.ChatType >= 72 && (int)chatEvent.ChatType <= 100) // ignore a bunch of non-chat messages
                                                        break;
                                                    if ((int)chatEvent.ChatType > 107) // don't handle anything past CWLS8 for now
                                                        break;
                                                    PluginLog.Error($"playerLink was null.\nChatType: {chatEvent.ChatType} ({(int)chatEvent.ChatType}) Sender: {chatEvent.Sender.TextValue} Message: {chatEvent.Message.TextValue}");
                                                    senderName = chatEvent.Sender.TextValue;
                                                    break;
                                            }



                                        }

                                        // only if we still need one
                                        if (senderWorld.Equals(string.Empty))
                                            senderWorld = state.LocalPlayer.HomeWorld.GameData.Name;

                                        // PluginLog.Information($"FRANZDEBUGGINGNULL Playerlink is null: {senderName}@{senderWorld}");
                                    }
                                    else
                                    {
                                        senderName = chatEvent.ChatType == XivChatType.TellOutgoing
                                            ? state.LocalPlayer.Name
                                            : playerLink.PlayerName;
                                        senderWorld = chatEvent.ChatType == XivChatType.TellOutgoing
                                            ? state.LocalPlayer.HomeWorld.GameData.Name
                                            : playerLink.World.Name;
                                        // PluginLog.Information($"FRANZDEBUGGING Playerlink was not null: {senderName}@{senderWorld}");
                                    }
                                }
                                else
                                {
                                    // only do this one if it's debug
                                    /*
                                    PluginLog.Debug($"Plugin interface LocalPlayer was null.\n"
                                        + $"ChatType: {chatEvent.ChatType} ({(int)chatEvent.ChatType}) Sender: {chatEvent.Sender.TextValue} Message: {chatEvent.Message.TextValue}");
                                    */
                                    senderName = string.Empty;
                                    senderWorld = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                PluginLog.Error(ex, "Could not deduce player name.");
                            }


                            try
                            {
                                await this.plugin.ChatTranslate.Translate(chatEvent.Message.TextValue, senderName.TextValue, senderWorld, chatEvent.ChatType);
                            }
                            catch (Exception e)
                            {
                                PluginLog.Error(e, "Could not send discord message.");
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        PluginLog.Error(e, "Could not process event.");
                    }
                }

                Thread.Yield();
            }
        }
    }
}
