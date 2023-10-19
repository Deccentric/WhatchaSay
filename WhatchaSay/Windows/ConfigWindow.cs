using System;
using System.IO;
using System.Numerics;
using System.Text;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using DeepL.Model;
using ImGuiNET;

namespace WhatchaSay.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    private Plugin ConfigPlugin;

    public ConfigWindow(Plugin plugin) : base(
        "WhatchaSay Configuration",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar)
    {
        this.Size = new Vector2(400, 500);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
        this.ConfigPlugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var enabledValue = this.Configuration.Enabled;
        var selfValue = this.Configuration.Translate_Self;
        var languageValue = this.Configuration.Language;
        var serviceValue = this.Configuration.Service;

        byte[] apiValue = new byte[40];
        apiValue = Encoding.UTF8.GetBytes(this.Configuration.Api_Key);

        var chatTypeSay = this.Configuration.ChatTypeEnabled[XivChatType.Say];
        var chatTypeShout = this.Configuration.ChatTypeEnabled[XivChatType.Shout];
        var chatTypeYell = this.Configuration.ChatTypeEnabled[XivChatType.Yell];
        var chatTypeParty = this.Configuration.ChatTypeEnabled[XivChatType.Party];
        var chatTypePvPTeam = this.Configuration.ChatTypeEnabled[XivChatType.PvPTeam];
        var chatTypeTell = this.Configuration.ChatTypeEnabled[XivChatType.TellIncoming];
        var chatTypeAlliance = this.Configuration.ChatTypeEnabled[XivChatType.Alliance];
        var chatTypeFreeCompany = this.Configuration.ChatTypeEnabled[XivChatType.FreeCompany];
        var chatTypeLs1 = this.Configuration.ChatTypeEnabled[XivChatType.Ls1];
        var chatTypeLs2 = this.Configuration.ChatTypeEnabled[XivChatType.Ls2];
        var chatTypeLs3 = this.Configuration.ChatTypeEnabled[XivChatType.Ls3];
        var chatTypeLs4 = this.Configuration.ChatTypeEnabled[XivChatType.Ls4];
        var chatTypeLs5 = this.Configuration.ChatTypeEnabled[XivChatType.Ls5];
        var chatTypeLs6 = this.Configuration.ChatTypeEnabled[XivChatType.Ls6];
        var chatTypeLs7 = this.Configuration.ChatTypeEnabled[XivChatType.Ls7];
        var chatTypeLs8 = this.Configuration.ChatTypeEnabled[XivChatType.Ls8];
        var chatTypeNoviceNetwork = this.Configuration.ChatTypeEnabled[XivChatType.NoviceNetwork];
        var chatTypeCustomEmote = this.Configuration.ChatTypeEnabled[XivChatType.CustomEmote];
        var chatTypeCrossLinkShell1 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell1];
        var chatTypeCrossLinkShell2 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell2];
        var chatTypeCrossLinkShell3 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell3];
        var chatTypeCrossLinkShell4 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell4];
        var chatTypeCrossLinkShell5 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell5];
        var chatTypeCrossLinkShell6 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell6];
        var chatTypeCrossLinkShell7 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell7];
        var chatTypeCrossLinkShell8 = this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell8];

        ImGui.Columns(1);

        if (ImGui.Checkbox("Enabled", ref enabledValue))
        {
            this.Configuration.Enabled = enabledValue;
            this.Configuration.Save();

            if (enabledValue == true)
            {
                ConfigPlugin.ChatTranslate.failed_deepL = 0;
                ConfigPlugin.ChatTranslate.failed_libre = 0;
            }
        }

        if (ImGui.Checkbox("Translate Self", ref selfValue))
        {
            this.Configuration.Translate_Self = selfValue;
            this.Configuration.Save();
        }

        ImGui.Text("This setting is your desired INCOMING language.");
        ImGui.SetNextItemWidth(150);
        if (ImGui.Combo("Target Language", ref languageValue, Plugin.LanguageDropdown))
        {
            this.Configuration.Language = languageValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        ImGui.Text("Translation Service");
        if (ImGui.RadioButton("LibreTranslate", ref serviceValue, 0))
        {
            this.Configuration.Service = 0;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        ImGui.SameLine();
        if (ImGui.RadioButton("DeepL", ref serviceValue, 1))
        {
            this.Configuration.Service = 1;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        ImGui.SetNextItemWidth(200);
        if (ImGui.InputText("DeepL API Key", apiValue, 40))
        {
            this.Configuration.Api_Key = Encoding.UTF8.GetString(apiValue);

            this.Configuration.Save();
        }

        ImGui.Text("Active Channels");
        ImGui.Columns(3);
        if (ImGui.Checkbox("Say", ref chatTypeSay))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Say] = chatTypeSay;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Shout", ref chatTypeShout))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Shout] = chatTypeShout;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Yell", ref chatTypeYell))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Yell] = chatTypeYell;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Party", ref chatTypeParty))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Party] = chatTypeParty;
            this.Configuration.ChatTypeEnabled[XivChatType.CrossParty] = chatTypeParty;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("PVP Team", ref chatTypePvPTeam))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.PvPTeam] = chatTypePvPTeam;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Tell", ref chatTypeTell))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.TellIncoming] = chatTypeTell;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Alliance", ref chatTypeAlliance))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Alliance] = chatTypeAlliance;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("FC", ref chatTypeFreeCompany))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.FreeCompany] = chatTypeFreeCompany;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Novice Network", ref chatTypeNoviceNetwork))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.NoviceNetwork] = chatTypeNoviceNetwork;
            this.Configuration.Save();
        }

        if (ImGui.Checkbox("Custom Emote", ref chatTypeCustomEmote))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CustomEmote] = chatTypeCustomEmote;
            this.Configuration.Save();
        }

        ImGui.NextColumn();

        if (ImGui.Checkbox("LS1", ref chatTypeLs1))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls1] = chatTypeLs1;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS2", ref chatTypeLs2))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls2] = chatTypeLs2;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS3", ref chatTypeLs3))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls3] = chatTypeLs3;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS4", ref chatTypeLs4))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls4] = chatTypeLs1;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS5", ref chatTypeLs1))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls5] = chatTypeLs5;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS6", ref chatTypeLs6))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls6] = chatTypeLs6;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS7", ref chatTypeLs7))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls7] = chatTypeLs7;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("LS8", ref chatTypeLs8))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.Ls8] = chatTypeLs8;
            this.Configuration.Save();
        }

        ImGui.NextColumn();

        if (ImGui.Checkbox("Cross LS1", ref chatTypeCrossLinkShell1))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell1] = chatTypeCrossLinkShell1;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS2", ref chatTypeCrossLinkShell2))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell2] = chatTypeCrossLinkShell2;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS3", ref chatTypeCrossLinkShell3))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell3] = chatTypeCrossLinkShell3;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS4", ref chatTypeCrossLinkShell4))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell4] = chatTypeCrossLinkShell1;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS5", ref chatTypeCrossLinkShell1))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell5] = chatTypeCrossLinkShell5;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS6", ref chatTypeCrossLinkShell6))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell6] = chatTypeCrossLinkShell6;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS7", ref chatTypeCrossLinkShell7))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell7] = chatTypeCrossLinkShell7;
            this.Configuration.Save();
        }
        if (ImGui.Checkbox("Cross LS8", ref chatTypeCrossLinkShell8))
        {
            this.Configuration.ChatTypeEnabled[XivChatType.CrossLinkShell8] = chatTypeCrossLinkShell8;
            this.Configuration.Save();
        }
        ImGui.Columns(1);
    }
}
