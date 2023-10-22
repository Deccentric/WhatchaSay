# WhatchaSay

An automatic translation plugin for Dalamud with support for multiple translation services.

## Main Points

* Auto Traslate Chat
  
  ![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/chaten2jp.png?raw=true)
  * Select between multiple translation services:
    * [LibreTranslate] Free Open Source, Translation Engine
      * Mirrors listed on the LibreTranslate Repo added by default.
      * Support to connect to a custom url for self hosted instance.
      * It's possible a node will be setup specifically for this project.
    * [DeepL] Supports Free Translation of 500,000 characters/month with signup.
    * Planned Support for [Microsoft Translator][Microsoft-Translator] Supports Free Translation of 2,000,000 characters/month with signup.
* Translation Command to translate messages before you send them.
* Langauge support for all officially supported languages by FFXIV.
* Ability to select which channels of chat will be automatically translated.
  * Does not support System, Battle, or Event channels.
* Planned support for all languages supported by the language engine.
  
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

### Building [Optional]
This step if only if you'd like to modify or build the code yourself. Skip to Activating in-game if you just want to install.
1. Open up `WhatchaSay.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `WhatchaSay/bin/x64/Debug/WhatchaSay.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the 3rd-party repository.
    * Repository: `https://raw.githubusercontent.com/Deccentric/Dalamud-Depot/main/repo.json`
    
    ![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/add_repo.png?raw=true)
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * Search for `WhatchaSay` and Install!
3. You should now be able to use `/whatchasay` (chat)

### Quick Start
Using `/whatchasay` or opening the plugin's settings will open the config window.

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/config.png?raw=true)

The config allows you to choose a translation service. If you use DeepL you will need to enter your API Key. If you do not have an API Key you can make an account and use it
for up to 500,000 characters for free every month.

Your DeepL account will need a credit card to sign up. I hope an API Key will not be necessary in the future.

This Configuration allows you to choose a language (Supports the officially supported langauges by FFXIV).
I will add all languages available in the langauge translation engines in a future update.

Using `/translate` will bring up a basic translation window.

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/translate.png?raw=true)

Once you click `Translate` whatever in the `Text to Translate`` will be translated and copied to your clipboard up to 256 characters.

### Getting a DeepL API Key
This is only required if you'd like to use the DeepL API option.

Step 1: Start Creating Your Account

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/deepl1.jpg?raw=true)

Step 2: Select the Free Plan

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/deepl2.jpg?raw=true)

Step 3: Go to Your Account

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/deepl3.jpg?raw=true)

Step 4: Select the 'Account' Tab

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/deepl4.jpg?raw=true)

Step 5: Copy your API Key

![image](https://github.com/Deccentric/WhatchaSay/blob/master/Images/deepl5.jpg?raw=true)

### Contributing to this Plugin
If any bugs related strictly to the plugin are found, please open an issue on the project. Please do not create an issue related to an inaccurate translation as that is related to the selected translation engine. If you'd like to contribute code to this repository, please create a new branch and create a pull request when completed. I will review as soon as I have time.

### Credit
During the development of this project I used [Dalamud.DiscordBridge][DalamudLink] as an example for how to handle messages in a concurrent way.

[SimplePlugin] was used as a base, and the [DalamudAPI] was referenced.

[DalamudLink]: https://github.com/goaaats/Dalamud.DiscordBridge
[SimplePlugin]: https://github.com/goatcorp/SamplePlugin
[DalamudAPI]: https://goatcorp.github.io/Dalamud/api/index.html
