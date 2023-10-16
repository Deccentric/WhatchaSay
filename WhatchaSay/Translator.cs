using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using GSF.Communication;
using Lumina.Excel.GeneratedSheets;
using WhatchaSay.Model;
using XivCommon.Functions;
using DeepL;
using GSF;
using FFXIVClientStructs.Havok;

namespace WhatchaSay
{
    public class Translator : IDisposable
    {

        private readonly Plugin plugin;
        private string lastSentMessage;

        private string[] translation_url = new[]
        {
            "https://translate.argosopentech.com/translate",
            "https://api-free.deepl.com/v2/translate"
        };

        private string[] lang_identifier = new[]
        {
            "en",
            "fr",
            "de",
            "ja",
            "es"
        };

        public readonly TranslatorMessageQueue MessageQueue;

        private readonly HttpClient Client = new HttpClient();

        private Configuration configuration;

        private readonly IClientState state;

        public Translator(Plugin plugin)
        {
            this.plugin = plugin;
            this.MessageQueue = new TranslatorMessageQueue(this.plugin);
            this.state = Plugin.State;
            this.configuration = plugin.Configuration;
        }

        public async Task Translate(string message, string senderName, string senderWorld, XivChatType chatType)
        {
            if (configuration.Service == 0)
            {
                await LibreTranslate(message, senderName, senderWorld, chatType);
            } else if (configuration.Service == 1)
            {
                await DeepLTranslate(message, senderName, senderWorld, chatType);
            }
        }

        private async Task DeepLTranslate(string message, string senderName, string senderWorld, XivChatType chatType)
        {
            var values = new Dictionary<string, string>
            {
              { "q", message },
              { "source", "auto" },
              { "target", lang_identifier[configuration.Language] }
            };

            // Grab Translation from selected LibreTranslate Source
            var content = new FormUrlEncodedContent(values);
            var response = await Client.PostAsync(translation_url[0], content);

            // Still use LibreTranslate to identify 'for free'
            LibreTranslateResponse responseJSON = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();
            if (responseJSON.detectedLanguage.language == lang_identifier[configuration.Language])
                return;

            // DeepL Object
            var DeepLTranslator = new DeepL.Translator(configuration.Api_Key);

            string language_identity = lang_identifier[configuration.Language].ToUpper();

            if (language_identity == "EN")
                language_identity = "EN-US";

            var translatedText = await DeepLTranslator.TranslateTextAsync(
             message,
             null,
             language_identity);


            lastSentMessage = $"{senderName}: DeepL({translatedText})";
            Plugin.Chat.Print(new XivChatEntry { Message = lastSentMessage, Type = XivChatType.SystemMessage });
            PluginLog.Information($"{senderName}: {translatedText}");
            Client.DefaultRequestHeaders.Authorization = null;
        }

        private async Task LibreTranslate(string message, string senderName, string senderWorld, XivChatType chatType)
        {
            var values = new Dictionary<string, string>
            {
              { "q", message },
              { "source", "auto" },
              { "target", lang_identifier[configuration.Language] }
            };

            // Grab Translation from selected LibreTranslate Source
            var content = new FormUrlEncodedContent(values);
            var response = await Client.PostAsync(translation_url[configuration.Service], content);

            LibreTranslateResponse responseJSON = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();

            if (responseJSON.detectedLanguage.language != lang_identifier[configuration.Language])
            {
                lastSentMessage = $"{senderName}: LibreTranslate({responseJSON.translatedText})";
                Plugin.Chat.Print(new XivChatEntry { Message = lastSentMessage, Type = XivChatType.SystemMessage });
                PluginLog.Information($"{senderName}: {responseJSON.translatedText} {responseJSON.detectedLanguage.language} {chatType.ToString()}");
            }
        }

        public async Task Start()
        {
            this.MessageQueue.Start();
        }

        public void Dispose()
        {
            PluginLog.Verbose("Discord DISPOSE!!");
            this.MessageQueue?.Stop();
        }
    }
}
