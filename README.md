# CS2 Beta resolution fix

The simple app let you change your resolution and refresh rate to arbitrary values in CS2 Beta.
It was made for my own purpose, due to the problem with unavailable resolutions and refresh rates in the options of the game.

You can do the changes easily without the app. It was made for people that don't want to mess around with config files.

# Instruction
Download the current [release]() of the app.

Simply find the Steam directory on your computer. Once it's done, the app will look for CS2 config file.
If it finds the needed file, you will be able to input the resolution and refresh rate. 

After clicking `Apply` button, the app will change the following lines of the `cs2_video.txt` file:
```txt
"setting.defaultres"		"XYZ"
"setting.defaultresheight"		"XYZ"
"setting.refreshrate_numerator"		"XYZ"

"setting.fullscreen"		"XYZ"
"setting.nowindowborder"    "XYZ"
```

You can open the directory of the file by clicking on the `Open cs2_video.txt directory` button.

If you logged on more than one account with CS2 access, you could have more than one SteamID folder with config on your computer. In that case you will be asked to choose the ID.

![multiple IDs gui](https://github.com/komeg1/cs2res_fix/blob/master/images/multipleid.png)

 If you don't know how to check it [check the following tutorial](https://help.steampowered.com/en/faqs/view/2816-BE67-5B69-0FEC).

# Additional information

From my experience, the settings doesn't fix the problem for everyone. 

And finally - 
I don't believe it's bannable. However, **use it at own risk**.