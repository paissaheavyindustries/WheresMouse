# Where's Mouse?

This Dalamud plugin is an accessibility plugin to help you locate your mouse cursor when you need it the most.

A vigorous shake of your wrist is enough to divine the location of your mouse pointer, even if it's cruising on other monitors or out of the game.

![Example indicator](https://i.imgur.com/xOWZa3s.jpg)

## How does it work

When you move your mouse, that movement distance is accumulated into a counter, and when the counter hits an upper threshold value, the effect is enabled. This counter decays over time of course, to prevent it from accumulating and triggering from minute movements. It's intended that only lots of movement in a small time frame (ie. a vigorous shake) would trigger the effect.

If the effect is active, the counter continues to decay, and when the counter has decayed under a lower threshold value the effect will be disabled.

## How to install

You will find the plugin in my Dalamud plugin repository at https://github.com/paissaheavyindustries/Dalamud-Repo! Follow the instructions there on how to use the repository.

## In-game usage and configuration

* Type `/wheremouse` to open the configuration UI
* General settings
  * "Indicator enabled" simply toggles the mouse pointer indicators on and off entirely
  * "Only show in combat" limits the indicator to combat situations
  * "Distance accumulation hysteresis" are the minimum and maximum threshold values for triggering the effect based on the distance accumulation counter
  * "Distance accumulation decay factor" is a multiplier that determines how quickly the accumulation counter decays when the effect is not active (smaller = faster)
  * "Active indicator decay factor" is a multiplier that determines how quickly the accumulation counter decays when the effect is active (smaller = faster)
* Indicator circle settings
  * "Draw circle" draws a circle on the mouse location
  * "Circle radius in pixels" determines how big the circle will be
  * "Circle color" defines the color of the circle
* Cardinal line settings
  * "Draw cardinal lines" draws cardinal lines from the mouse location to (what is practically) infinity
  * "Cardinal line thickness in pixels" determines how thick the lines will be
  * "Cardinal line color" defines the color of the lines
* Intercardinal line settings
  * "Draw intercardinal lines" draws intercardinal (diagonal) lines from the mouse location to (what is practically) infinity
  * "Intercardinal line thickness in pixels" determines how thick the lines will be
  * "Intercardinal line color" defines the color of the lines  

## Support / Discord

There's a publicly available Discord server for announcements, suggestions, and questions - feel free to join at:

https://discord.gg/6f9MY55
