# TheHuntIsOn

Provides functions split into modules that allow teams to be modified to have various effects.

Under the mod settings the role and the module affection can be selected. **Each module is bound to the selected role**, so if you've chosen the role "Hunter" all modules which are either set to "Hunter" or "Both" are active.

TheHuntIsOn should not be used for regular save files.

## Other

### Hunter Focus Cost
Adjusts the cost of soul to heal if the player is a hunter. **Note that the extra soul is only taken once the heal has been successful.**

### Hunter Focus Speed
Adjusts the time focus takes to provide a heal if the player is a hunter.

### Hunter Spell Cost
Adjusts the amount of soul to cast a spell if the player is a hunter.

## Modules

### Auto Trigger Boss Module
Start certain boss (like dream warrior) encounters automatically once you enter their range.

### Baldur Module
Reduces Elder Baldur (at Greenpath entrance and Ancestral Mound only) HP to 5.

### Bench Module
Benches no longer provide healing. *As the code for healing in the game is a bit sketchy, the hud is disabled and enabled quickly to sync it properly.*

### Boss Module
Increases boss HP to 9999. Adds teleporters so that players can enter and exit Dream Boss arenas.

### Charm Nerf Module
Increases the notch cost of powerful PvP charms by 1. This includes Baldur Shell, Spore Shroom, Heart, and Nailmaster's Glory.

### Completion Module
Unlocks many walls, doors, dive ground etc. all around the map as it has been visited already. Provides starting items.

### Cutscene Skip Module
Skips or speeds up specific cutscenes. This includes the Mothwing Cloak dreamer sequence, the Dream Nail segment, and the Black Egg Temple door opening sequence.

### Disable Soul Gain Module
Causes all nail and Dream Nail swings against enemies to provide zero soul.

### Dream Entrance Module
Makes Dream sequences more accessible by placing dream boss bodies outside of their arenas and placing interactable teleporters to enter certain dream segments.

### Dream Heal Module
Exiting a dream sequence, like a boss fight or a dreamer does not longer provide a full heal.

### Elevator Module
Removes all small CoT elevators and places platform to help climbing them (if the cannot be climbed already by having claw for example). Also removes the lever in the big elevator and adds a door transition there to enter the other half more quickly.

### EventNetworkModule
The EventNetworkModule networks certain events, such as obtained items by the speedrunner and then grants the hunters
items based on this and sends a message.
The module uses a configuration file next to the `.dll` file named `networked-event.json`.
This file determines which events that the speedrunner causes will trigger a set of items to be granted to the hunters.
If there is no such file found next to the `.dll` file, it will generate a default file.

An example of the format of the file is the following:
```json
{
    "MothwingCloak": {
        "items": [
            "CrystalHeart", "Mask"
        ],
        "message": "Speedrunner has obtained Mothwing Cloak. The hunters have now access to Crystal Heart and gained an additional mask."
    },
    "MantisClaw": {
        "items": [
            "MonarchWings"
        ],
        "message": "Speedrunner has obtained Mantis Claw. All players have now access to Monarch Wings."
    }
}
```
This configuration will make it so that when the speedrunner obtains the Mothwing Cloak, the hunters will 
automatically get Crystal Heart and get an additional mask. The message will also be broadcast to all players to
the server. And if the speedrunner obtains Mantis Claw, the hunters will get Monarch Wings.

The first string in each entry represents the trigger and can be one of the following:  
`VengefulSpirit`, `DesolateDive`, `HowlingWraiths`, `ShadeSoul`, `DescendingDark`, `AbyssShriek`, `MothwingCloak`,
`MantisClaw`, `CrystalHeart`, `MonarchWings`, `IsmasTear`, `ShadeCloak`, `DreamNail`, `CycloneSlash`, `DashSlash`,
`GreatSlash`, `Mask`, `SoulVessel`, `Movement1`, `Movement2`, `Movement3`, `Movement4`, `Movement5`, `Movement6`,
`Dreamer`, `Tram`, `Toll`, `Grub`, `DreamWarriorStarted`, `DreamWarriorAbsorbed`, `ShopPurchase`, `NailUpgrade`
`RelicSale`, `LeverHit`, `CharmNotch`, `CharmCollected`, `PowerUp`, `BossKilled`, `Stag`, `StagDirtmouth`, 
`StagCrossroads`, `StagGreenpath`, `StagFungalWastes`, `StagCityStorerooms`, `StagRestingGrounds`, `StagKingsStation`, 
`StagDeepnest`, `StagRoyalGardens`, `StagHiddenStation`, `StagStagNest`

Note that specific Stag events only trigger while another player who is not on the Speedrunner's HKMP team is within
the same room as the Speedrunner. The general Stag event triggers regardless.

The triggers `MovementX` trigger when the speedrunner obtains the X-th movement item. The following are counted as
movement items:
Mothwing Cloak, Mantis Claw, Crystal Heart, Monarch Wings, Isma's Tear, Shade Cloak

The trigger `PowerUp` triggers when the speedrunner obtains items defined as "powerup" items. This
includes completed Masks, completed Vessels, charm notches, nail upgrades, spells, spell upgrades, and nail arts.

The items granted under `"items": [ ... ]` can be one of the following:  
`VengefulSpirit`, `DesolateDive`, `HowlingWraiths`, `ShadeSoul`, `DescendingDark`, `AbyssShriek`, `MothwingCloak`,
`MantisClaw`, `CrystalHeart`, `MonarchWings`, `IsmasTear`, `ShadeCloak`, `DreamNail`, `CycloneSlash`, `DashSlash`,
`GreatSlash`, `Mask`, `MaskShard`, `SoulVessel`, `NailUpgrade`

Note that sales and purchases made using QoL's NPCSellAll module do not currently trigger events.

### Helper Platform Module
Places some platforms around the world to help reaching some ledges with fewer items than actually required.

### Intangible Gates Module
Makes all arena gates intangible for the player character.

### Invisible Gates Module
Makes all arena gates invisible for the player character.

### Lifeseed Module
Causes killing lifeblood seed to not provide any lifeblood.

### Mask Module
Removes the full heal provided by completing a mask. The one extra hp by the mask itself is still provided.

### Notch Module
Picking up new charm notches will no longer fully heal you.

### PauseTimer Module
See the [README](Modules/PauseTimerModule/README.md).

### Respawn Module
Causes the respawn after dying to always locate to KP regardless of the last bench used.

### Shade Module
Removes the shade and all it's effect (removing geo and breaking the soul vessel).

### Shade Skip Module
Allows navigation (creates platforms) through skip locations requiring the shade.

### Spa Module
Disables healing and soul gain from spas.

### Stag Module
Disables stag travelling.

### Tram Module
Disables trams.