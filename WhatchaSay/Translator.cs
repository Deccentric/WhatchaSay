using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dalamud.Game.Text;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using WhatchaSay.Model;

namespace WhatchaSay
{
    public class Translator : IDisposable
    {

        private readonly Plugin plugin;
        private string SendMessage;

        public readonly TranslatorMessageQueue MessageQueue;

        private readonly HttpClient Client = new HttpClient();

        private Configuration configuration;

        private readonly IClientState state;

        public int failed_libre = 0;
        public int failed_deepL = 0;

        public Translator(Plugin plugin)
        {
            this.plugin = plugin;
            this.MessageQueue = new TranslatorMessageQueue(this.plugin);
            this.state = Plugin.State;
            this.configuration = plugin.Configuration;
        }

        public async Task<String> Translate(StringTranslateItem StringItem)
        {
            string output = "";
            //PluginLog.Information($"Attempting to translate {StringItem.Message} to {StringItem.Target}");
            if (configuration.Service == 0)
            {
                output = await LibreTranslate(StringItem);
            }
            else if (configuration.Service == 1)
            {
                output = await DeepLTranslate(StringItem);
            }

            return output;
        }

        public async Task Translate(string message, string senderName)
        {
            if (failed_deepL > 10 || failed_libre > 10)
            {
                Plugin.Chat.Print(new XivChatEntry { Message = "Something seems wrong so the translate plugin is disabling itself. Please check /xllog and report it.", Type = XivChatType.SystemMessage });
                if (failed_deepL > 10)
                    Plugin.Chat.Print(new XivChatEntry { Message = "Seems like the DeepL Api cannot be reached.", Type = XivChatType.SystemMessage });
                if (failed_libre > 10)
                    Plugin.Chat.Print(new XivChatEntry { Message = "Seems like the LibreTranslate Api cannot be reached.", Type = XivChatType.SystemMessage });

                plugin.Configuration.Enabled = false;
                return;
            }

            //PluginLog.Information($"Attempting to send {message} from {senderName}");
            if (configuration.Service == 0)
            {
                await LibreTranslate(message, senderName);
            } else if (configuration.Service == 1)
            {
                if (configuration.Api_Key != null && configuration.Api_Key != "")
                  await DeepLTranslate(message, senderName);
            }
        }

        private async Task<string> DeepLTranslate(StringTranslateItem TranslatePkt)
        {
            LibreTranslateResponse responseJSON;

            // Grab Translation from selected LibreTranslate to determine Source
            try
            {
                responseJSON = await MakeLibreTranslateRequest(TranslatePkt.Message, TranslatePkt.Target);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "There was an error getting a response from LibreTranslate. Trying one more time.");
                try
                {
                    responseJSON = await MakeLibreTranslateRequest(TranslatePkt.Message, TranslatePkt.Target);
                }
                catch (Exception nested_ex)
                {
                    PluginLog.Error(nested_ex, "LibreTranslated failed to return a response, aborting operation.");
                    failed_libre++;
                    return "";
                }
            }

            failed_libre = 0;

            //PluginLog.Information($"Contacting DeepL with API Key: {configuration.Api_Key}!!!");

            if (responseJSON == null || responseJSON.detectedLanguage.language == TranslatePkt.Target)
                return TranslatePkt.Message;


            // DeepL Object
            try
            {
                var DeepLTranslator = new DeepL.Translator(configuration.Api_Key);


                string language_identity = TranslatePkt.Target.ToUpper();

                if (language_identity == "EN")
                    language_identity = "EN-US";

                var translatedText = await DeepLTranslator.TranslateTextAsync(
                 TranslatePkt.Message,
                 null,
                 language_identity);

                //PluginLog.Information($"Translate: DeepL({translatedText})");
                failed_deepL = 0;
                return translatedText.ToString();
            }
            catch
            {
                PluginLog.Error("There was an error receiving requested data from DeepL.");
                failed_deepL++;
                return "";
            }
        }

        private async Task DeepLTranslate(string message, string senderName)
        {
            LibreTranslateResponse responseJSON;

            // Only Run LibreTranslate Check in the Case Data Saver is turned on [Default]
            if (configuration.Data_Saver)
            {
                // Grab Translation from selected LibreTranslate to determine Source
                try
                {
                    responseJSON = await MakeLibreTranslateRequest(message);
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "There was an error getting a response from LibreTranslate. Trying one more time.");
                    try
                    {
                        responseJSON = await MakeLibreTranslateRequest(message, senderName);
                    }
                    catch (Exception nested_ex)
                    {
                        PluginLog.Error(nested_ex, "LibreTranslated failed to return a response, aborting operation.");
                        failed_libre++;
                        return;
                    }
                }
                failed_libre = 0;

                if (responseJSON == null || responseJSON.detectedLanguage.language == Plugin.LanguageIdentifiers[configuration.Language])
                    return;
            }
            

            // DeepL Object
            try
            {
                var DeepLTranslator = new DeepL.Translator(configuration.Api_Key);

                string language_identity = Plugin.LanguageIdentifiers[configuration.Language].ToUpper();

                if (language_identity == "EN")
                    language_identity = "EN-US";

                var translatedText = await DeepLTranslator.TranslateTextAsync(
                 message,
                 null,
                 language_identity);

                SendMessage = $"{senderName}: DeepL({translatedText})";
                Plugin.Chat.Print(new XivChatEntry { Message = SendMessage, Type = XivChatType.SystemMessage });
                //PluginLog.Information($"{senderName}: DeepL({translatedText})");
                failed_deepL = 0;
            } catch
            {
                PluginLog.Error("There was an error receiving requested data from DeepL.");
                failed_deepL++;
            }
            
        }

        private async Task<string> LibreTranslate(StringTranslateItem TranslatePkt)
        {
            LibreTranslateResponse responseJSON;

            try
            {
                responseJSON = await MakeLibreTranslateRequest(TranslatePkt.Message, TranslatePkt.Target);

            }
            catch (Exception ex)
            {
                PluginLog.Error("There was an error getting a response from LibreTranslate. Trying one more time.");
                try
                {
                    responseJSON = await MakeLibreTranslateRequest(TranslatePkt.Message, TranslatePkt.Target);
                }
                catch (Exception nested_ex)
                {
                    PluginLog.Error(nested_ex, "LibreTranslated failed to return a response, aborting operation.");
                    return "";
                    failed_libre++;
                }
            }
            failed_libre = 0;

            if (responseJSON == null)
            {
                return "";
            }

            SendMessage = responseJSON.translatedText;
            return SendMessage;
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
                    failed_libre++;
                    return;
                }
            }
            failed_libre = 0;

            if (responseJSON == null) {
                return;
            }

            if (responseJSON.detectedLanguage.language != Plugin.LanguageIdentifiers[configuration.Language])
            {
                SendMessage = $"{senderName}: LibreTranslate({responseJSON.translatedText})";
                Plugin.Chat.Print(new XivChatEntry { Message = SendMessage, Type = XivChatType.SystemMessage });
                //PluginLog.Information($"{senderName}: {responseJSON.translatedText} {responseJSON.detectedLanguage.language}");
            }


        }

        public async Task<LibreTranslateResponse> MakeLibreTranslateRequest(string message, string target = "")
        {
            //PluginLog.Information($"Received: {message} {target} Some debug text");
            string target_lang = target == "" ? Plugin.LanguageIdentifiers[configuration.Language]: target;
            var values = new Dictionary<string, string>
            {
              { "q", message },
              { "source", "auto" },
              { "target", target_lang.ToLower() }
            };

            // Grab Translation from selected LibreTranslate Source
            var content = new FormUrlEncodedContent(values);

            var libreTranslateLink = configuration.LibreTranslateMirror == 4 ? configuration.CustomLibre : Plugin.LibreTranslateMirrors[configuration.LibreTranslateMirror];
            var response = await Client.PostAsync(libreTranslateLink, content);

            //PluginLog.Information($"Requesting information from {libreTranslateLink}.");
            LibreTranslateResponse responseJSON = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>();

            //PluginLog.Information($"Response JSON: {responseJSON.translatedText}");
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
