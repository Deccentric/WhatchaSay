using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace WhatchaSay.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    public ConfigWindow(Plugin plugin) : base(
        "WhatchaSay Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(232, 400);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var enabledValue = this.Configuration.Enabled;
        var languageValue = this.Configuration.Language;
        var serviceValue = this.Configuration.Service;
        var apiValue = this.Configuration.Api_Key;

        if (ImGui.Checkbox("Enabled", ref enabledValue))
        {
            this.Configuration.Enabled = enabledValue;
            this.Configuration.Save();
        }

        ImGui.Text("Target Language");
        if (ImGui.RadioButton("English", ref languageValue, 0))
        {
            this.Configuration.Language = 0;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        if (ImGui.RadioButton("French", ref languageValue, 1))
        {
            this.Configuration.Language = 1;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        if (ImGui.RadioButton("German", ref languageValue, 2))
        {
            this.Configuration.Language = 2;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        if (ImGui.RadioButton("日本語", ref languageValue, 3))
        {
            this.Configuration.Language = 3;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

        if (ImGui.RadioButton("Spanish", ref languageValue, 4))
        {
            this.Configuration.Language = 4;
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

        if (ImGui.RadioButton("DeepL", ref serviceValue, 1))
        {
            this.Configuration.Service = 1;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }

    }
}
