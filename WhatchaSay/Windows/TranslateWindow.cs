using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using WhatchaSay.Model;

namespace WhatchaSay.Windows;

public class TranslateWindow : Window, IDisposable
{
    private Configuration Configuration;

    byte[] Text2Translate = new byte[256];
    public byte[] TextOutput = new byte[256];

    public bool IsWaiting = false;

    private Translator WindowTranslate;

    int Target_language;
    string Language_Options;

    public TranslateWindow(Plugin plugin) : base(
        "WhatchaSay Translation",
        ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        this.Size = new Vector2(400, 150);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
        this.WindowTranslate = plugin.ChatTranslate;

    }

    public void CopyResult(string To_Copy)
    {
        ImGui.SetClipboardText(To_Copy);
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.InputText("Text to Translate", Text2Translate, 256);
        if (ImGui.Button("Translate")) 
        {
            if (!IsWaiting)
            {
                string QueueMessage = Encoding.UTF8.GetString(Text2Translate).TrimEnd(new char[] { (char)0 });

                if (QueueMessage != "")
                {
                    IsWaiting = true;
                    WindowTranslate.MessageQueue.Enqueue(new StringTranslateItem { Message = QueueMessage, Target = Plugin.LanguageIdentifiers[Target_language] });
                }

                // Reset the byte array
                Text2Translate = new byte[256];
            }
        }
        ImGui.SameLine();

        ImGui.SetNextItemWidth(190);
        ImGui.Combo("Target Language", ref Target_language, Plugin.LanguageDropdown);

        ImGui.InputText("Translated Text", TextOutput, 256, ImGuiInputTextFlags.ReadOnly);

        if (IsWaiting)
        {
            ImGui.Text("Loading....");
        }
    }
}
