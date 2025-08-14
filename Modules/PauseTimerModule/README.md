# PauseTimer Module

PauseTimer is a Module for TheHuntIsOn, which enables various time control features.

Like the EventNetworkModule, it requires initialization to function. To use the PauseTimer module as server _or_ client, it needs to be enabled in mod options during mod setup, so you will need to restart your game after turning it on for the first time.

## Pause Controls

The pause timer module enables server-wide pauses, which can be lifted at will or with a synchronized delay. When the server is paused, all connected players will have their worlds frozen, unable to move or be interacted with.

Unpauses, and countdowns, are synchronized through UTC system time. If any clients have desynchronized system clocks relative to the host, they may experience buggy behavior.

## Respawn Timer

Authorized users can configure a respawn timer of X seconds, which forces any player upon death to wait X seconds before respawning. The clock starts at the instant of player death, so countdowns of ~5 seconds or less have next to no material effect, due to the duration of the death animation.

## /pausetimer command

Server controls are managed through the `/pausetimer`, or `/pt` command. Only authorized users can use this command.

`/pt help [<command>]` shows command instructions ingame.

`/pt pause [X]` pauses the server for all players, either indefinitely, or for `X` seconds.

`/pt unpause [X]` unpauses the server for all players, either immediately, or after `X` seconds.

`/pt countdown X [msg...]` broadcasts an info-only countdown lasting `X` seconds.

`/pt clearcountdowns` clears out all info-only countdowns.

`/pt respawntimer|deathtimer X` sets a respawn timer on death of `X` seconds for all players. Set to 0 to clear.

## Mod Options

Users can control where and how largely countdowns are displayed through the "Timer Position" and "Timer Size" options on TheHuntIsOn page. These settings are localized and do not affect other players.

## Disconnects

PauseTimer lifts its restrictions in the event of a disconnect to facilitate offline play and is not currently suitable for truly competitive, client-hostile play.

Players who disconnect during a pause should avoid significant movement before the server is unpaused. Players who disconnect while respawning will immediately respawn, but can still choose to honor the on-screen respawn timer and sit still until it finishes.
