# CS2 Beta resolution fix

The simple app let you change your resolution, aspect ratio and refresh rate to arbitrary values in CS2 Beta.
It was made for my own purpose, due to the problem with unavailable resolutions and refresh rates in the options of the game.

You can do the changes easily without the app. It was made for people that don't want to mess around with config files.
If you are looking for fix without using the app, simply go to `Steam/userdata/{steamid}/730/local/cfg/cs2_video.txt` and change the lines listed below.

## Additional interpolation settings

With the v1.0.3 version, I decided to add the interpolation settings to the app. The settings are based on [@JLOPEZOMG config tweet](https://twitter.com/JLOPEZOMG/status/1699112405433999444) and should help you if you struggle with lags and choppy gameplay feeling.



# Instruction
Download the current [release](https://github.com/komeg1/cs2res_fix/releases/tag/v1.0.3) of the app.

![App's GUI](https://github.com/komeg1/cs2res_fix/blob/master/images/gui.png)

Simply find the Steam directory on your computer. Once it's done, the app will look for CS2 config file.
If it finds the needed file, you will be able to choose the aspect ratio and input the resolution and refresh rate.

After clicking `Apply` button, the app will change the following lines of the `cs2_video.txt` file:
```txt
"setting.defaultres"		"XYZ"
"setting.defaultresheight"		"XYZ"
"setting.refreshrate_numerator"		"XYZ"
...
"setting.fullscreen"		"XYZ"
"setting.nowindowborder"    "XYZ"
...
"setting.aspectratiomode"		"XYZ"
```
where `XYZ` is the value you input in the app.

You can open the directory of the file by clicking on the `Open cs2_video.txt directory` button.

If you logged on more than one account with CS2 access, you could have more than one SteamID folder with config on your computer. In that case you will be asked to choose the ID.

![multiple IDs gui](https://github.com/komeg1/cs2res_fix/blob/master/images/multipleid.png)

 If you don't know how to check it [check the following tutorial](https://help.steampowered.com/en/faqs/view/2816-BE67-5B69-0FEC).

 ## Interpolation settings

Simply find CS:GO directory on your computer. Once it's done, the app will look for `cfg` folder. Then, you will be able to choose the settings you want to add to your config (more information in [@JLOPEZOMG config tweet](https://twitter.com/JLOPEZOMG/status/1699112405433999444)).

After clicking `Modify` button, the app will create/modify the autoexec.cfg file by adding/deleting the following lines:
```txt
cl_interp_ratio "1"
cl_update_rate "128"
cl_interp "X"
```
where `X` is the value you choose in the app. 

**After 09.06.2023 CS2 Beta update, the default value is `cl_interp 0.046875` which seems to be working fine**, however you can still experiment with the other values:

- Use `cl_interp 0.015625` if you have stable internet and good ping.
- Use `cl_interp 0.03125` if you have unstable internet and/or high ping.



# Additional information

From my experience, the settings doesn't fix the problem for everyone. 

Additionaly, when you enter the CS2 options in game, it will still say the wrong info (e.g. resolution 1366x768 instead of the one you set). It seems to be a visual bug.

The app may contain some bugs. It was made quickly to fix the problem. If you encounter any big bug, contact me on Discord - komeg1, [Steam](https://steamcommunity.com/profiles/76561198173528881/) or use the issues tab.

`autoexec.cfg` config will load the settings every time you launch the game. Once you delete the file, you will have to add them manually after every launch.

And finally - 
config files aren't bannable. I don't know about the resolution settings,but I don't believe it's bannable. However, **use it at own risk**.
