using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using WhatchaSay.Model;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using WhatchaSay.Windows;
using System.Threading.Tasks;
using System.Linq;

namespace WhatchaSay
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "WhatchaSay";
        private const string CommandName = "/WhatchaSay";
        private const string TranslateCommand = "/Translate";

        public Translator ChatTranslate;

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("WhatchaSay");

        [PluginService]
        public static IChatGui Chat { get; private set; } = null!;

        [PluginService] public static IClientState State { get; private set; }

        private ConfigWindow ConfigWindow { get; init; }
        public TranslateWindow TranslationWindow { get; init; }

        private static readonly XivChatType[] AllowedChatTypes = new[]
        {
            XivChatType.Say,
            XivChatType.Shout,
            XivChatType.Yell,
            XivChatType.Party,
            XivChatType.CrossParty,
            XivChatType.PvPTeam,
            XivChatType.TellIncoming,
            XivChatType.Alliance,
            XivChatType.FreeCompany,
            XivChatType.Ls1,
            XivChatType.Ls2,
            XivChatType.Ls3,
            XivChatType.Ls4,
            XivChatType.Ls5,
            XivChatType.Ls6,
            XivChatType.Ls7,
            XivChatType.Ls8,
            XivChatType.NoviceNetwork,
            XivChatType.CustomEmote,
            XivChatType.CrossLinkShell1,
            XivChatType.CrossLinkShell2,
            XivChatType.CrossLinkShell3,
            XivChatType.CrossLinkShell4,
            XivChatType.CrossLinkShell5,
            XivChatType.CrossLinkShell6,
            XivChatType.CrossLinkShell7,
            XivChatType.CrossLinkShell8
        };

        // TODO Convert all of these
        public static string[] AvailableLangauges =
        {
            "English",
            "Arabic",
            "Bulgarian",
            "Czech",
            "Danish",
            "Deutsch",
            "Greek",
            "Español",
            "Estonian",
            "Finnish",
            "Français",
            "Hungarian",
            "Indonesian",
            "Italiano",
            "日本語 - Japanese",
            "Korean",
            "Lithuanian",
            "Latvian",
            "Norwegian",
            "Dutch",
            "Polish",
            "Portuguese",
            "Romanian",
            "Russian",
            "Slovak",
            "SLovenian",
            "Swedish",
            "Turkish",
            "Ukranian",
            "简体字 - Simplified Chinese"

        };

        public static string[] LanguageIdentifiers =
        {
            "en",
            "ar",
            "bg",
            "cs",
            "da",
            "de",
            "el",
            "es",
            "et",
            "fi",
            "fr",
            "hu",
            "id",
            "it",
            "ja",
            "ko",
            "lt",
            "lv",
            "nb",
            "nl",
            "pl",
            "pt",
            "ro",
            "ru",
            "sk",
            "sl",
            "sv",
            "tr",
            "uk",
            "zh"
        };

        public static string[] LibreTranslateMirrors =
        {
            "https://translate.argosopentech.com/translate",
            "https://translate.terraprint.co/translate",
            "https://trans.zillyhuhn.com/translate",
            "https://libretranslate.eownerdead.dedyn.io/translate",
            "Custom"
        };

        public static string DeepLURL = "https://api-free.deepl.com/v2/translate";

        public static string LanguageDropdown = string.Join("\0", AvailableLangauges);
        public static string LibreTranslateDropdown = string.Join("\0", LibreTranslateMirrors);

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
            {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            this.ChatTranslate = new Translator(this);
            ConfigWindow = new ConfigWindow(this);
            TranslationWindow = new TranslateWindow(this);
            
            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(TranslationWindow);

            Task.Run(async () =>
            {
                Chat.ChatMessage += this.ChatTranslateMessage;
                await this.ChatTranslate.Start();
            });


            this.CommandManager.AddHandler(CommandName.ToLower(), new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the Configuration Window."
            });

            this.CommandManager.AddHandler(TranslateCommand.ToLower(), new CommandInfo(OnTranslateCommand)
            {
                HelpMessage = "/translate Open the translate window."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private void ChatTranslateMessage(XivChatType type, uint senderid, ref SeString sender, ref SeString message, ref bool ishandled)
        {
            if (!Configuration.Enabled) return;

            if (ishandled) return;

            if (!AllowedChatTypes.Contains(type)) return;

            if (!this.Configuration.ChatTypeEnabled[type]) return;

            this.ChatTranslate.MessageQueue.Enqueue(new ChatTranslateItem
            {
                ChatType = type,
                Message = message,
                Sender = sender
            });

        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            ConfigWindow.Dispose();
            TranslationWindow.Dispose();

            this.ChatTranslate.Dispose();
            
            this.CommandManager.RemoveHandler(CommandName.ToLower());
            this.CommandManager.RemoveHandler(TranslateCommand.ToLower());
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            ConfigWindow.IsOpen = true;
        }

        private void OnTranslateCommand(string command, string args)
        {
            TranslationWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

    }
}
