# WhatchaSay

An automatic translation plugin for Dalamud with support for multiple translation services.

## Main Points

* Auto Traslate Chat
  * Select between multiple translation services:
    * [LibreTranslate] Free Open Source, Translation Engine
      * Mirrors listed on the LibreTranslate Repo added by default.
      * Support to connect to a custom url for self hosted instance.
      * It's possible a node will be setup specifically for this project.
    * [DeepL] Supports Free Translation of 500,000 characters/month with signup.
    * Planned Support for [Microsoft Translator][Microsoft-Translator] Supports Free Translation of 2,000,000 characters/month with signup.
* Planned Translation Command to translate messages before you send them.
* Ability to select which channels of chat will be automatically translated.
  * Does not support System, Battle, or Event channels.
  
[LibreTranslate]: https://github.com/LibreTranslate/LibreTranslate
[DeepL]: https://deepl.com/translator
[Microsoft-Translator]: https://www.microsoft.com/translator/personal/
## How To Use

### Getting Started

As of now, this plugin is not officially supported by the Dalamud project and will require 3rd-party plugin respository to be added.

Link to Plugin Usage Documentation

### Prerequisites

This plugin requires Dalamud in order to work.

[Dalamud] is a plugin development framework for Final Fantasy XIV that can be installed with [XIVLauncher].

[Dalamud]: https://github.com/goatcorp/Dalamud
[XIVLauncher]: https://goatcorp.github.io/

### Building

1. Open up `WhatchaSay.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `WhatchaSay/bin/x64/Debug/WhatchaSay.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the 3rd-party repository.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and the `WhatchaSay` should be visible. Enable it.
3. You should now be able to use `/WhatchaSay` (chat)

### Contributing to this Plugin
If any bugs related strictly to the plugin are found, please open an issue on the project. Please do not create an issue related to an inaccurate translation as that is related to the selected translation engine. If you'd like to contribute code to this repository, please fork the master branch and create a pull request when completed.

###Credit
During the development of this project I used [Dalamud.DiscordBridge][DalamudLink] as an example for how to handle messages in a concurrent way.

[SimplePlugin] was used as a base, and the [DalamudAPI] was referenced.

[DalamudLink]: https://github.com/goaaats/Dalamud.DiscordBridge
[SimplePlugin]: https://github.com/goatcorp/SamplePlugin
[DalamudAPI]: https://goatcorp.github.io/Dalamud/api/index.html