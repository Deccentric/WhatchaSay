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

        public async Task Translate(string message, string senderName)
        {
            PluginLog.Information($"Attempting to send {message} from {senderName}");
            if (configuration.Service == 0)
            {
                await LibreTranslate(message, senderName);
            } else if (configuration.Service == 1)
            {
                await DeepLTranslate(message, senderName);
            }
        }

        private async Task DeepLTranslate(string message, string senderName)
        {
            LibreTranslateResponse responseJSON;

            // Grab Translation from selected LibreTranslate to determine Source
            try
            {
                responseJSON = await MakeLibreTranslateRequest(message);
            } catch (Exception ex)
            {
                PluginLog.Error(ex, "There was an error getting a response from LibreTranslate. Trying one more time.");
                try
                {
                    responseJSON = await MakeLibreTranslateRequest(message, senderName);
                } catch (Exception nested_ex)
                {
                    PluginLog.Error(nested_ex, "LibreTranslated failed to return a response, aborting operation.");
                    return;
                }
            }

            if (responseJSON == null || responseJSON.detectedLanguage.language == lang_identifier[configuration.Language])
                return;
            

            // DeepL Object
            try
            {
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
                PluginLog.Information($"{senderName}: DeepL({translatedText})");
                Client.DefaultRequestHeaders.Authorization = null;
            } catch
            {
                PluginLog.Error("There was an error receiving requested data from DeepL.");
            }
            
        }

        private async Task LibreTranslate(string message, string senderName)
        {
            LibreTranslateResponse responseJSON;

            try
            {
                responseJSON = await MakeLibreTranslateRequest(message);
                
            } catch(Exception ex)
            {
                PluginLog.Error("There was an error getting a response from LibreTranslate. Trying one more time.");
                try
                {
                    responseJSON = await MakeLibreTranslateRequest(message);
                }
                catch (Exception nested_ex)
                {
                    PluginLog.Error(nested_ex, "LibreTranslated failed to return a response, aborting operation.");
                    return;
                }
            }

            if (responseJSON == null) {
                return;
            }

            if (responseJSON.detectedLanguage.language != lang_identifier[configuration.Language])
            {
                lastSentMessage = $"{senderName}: LibreTranslate({responseJSON.translatedText})";
                Plugin.Chat.Print(new XivChatEntry { Message = lastSentMessage, Type = XivChatType.SystemMessage });
                PluginLog.Information($"{senderName}: {responseJSON.translatedText} {responseJSON.detectedLanguage.language}");
            }


        }

        public async Task<LibreTranslateResponse> MakeLibreTranslateRequest(string message, string target = "")
        {
            string target_lang = target == "" ? lang_identifier[configuration.Language]: target;
            var values = new Dictionary<string, string>
            {
              { "q", message },
              { "source", "auto" },
              { "target", target_lang }
            };

            // Grab Translation from selected LibreTranslate Source
            var content = new FormUrlEncodedContent(values);
            var response = await Client.PostAsync(translation_url[0], content);

            LibreTranslateResponse responseJSON = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();

            return responseJSON;
        }

        public async Task Start()
        {
            this.MessageQueue.Start();
        }

        public void Dispose()
        {
            this.MessageQueue?.Stop();
        }
    }
}
