# Unsupported Costumes

The `costumed` need is based off of VME's Need_Anonymity.NeedInterval() implementation: it scans worn apparel and raises the need only if an item’s bodyPartGroups contains FullHead. UpperHead or Eyes coverage alone doesn’t qualify. Social interactions can affect the need separately, but they don’t change this apparel check.

So even if other mods provide costumes, those costumes aren't valid if they don't cover FullHead.

[Lava's Spooky Halloween Extravaganza (1.6)](https://steamcommunity.com/sharedfiles/filedetails/?id=3785375776)

None qualifies under the current FullHead rule: the top hat and widow’s veil cover only UpperHead, while its four masks cover only Eyes. Its costume clothing also uses the OnSkin layer, which the generator won’t replace.

[Lisa's Peasant Apparel](https://steamcommunity.com/sharedfiles/filedetails/?id=3793374806)

None of this mod’s items count as FullHead—its hats cover UpperHead, its masks cover Eyes, and its costume clothing is OnSkin. The current pool filter and generator would exclude them.

[ClownCore - Style](https://steamcommunity.com/sharedfiles/filedetails/?id=3823461701)

Ideology style of existing Vanilla apparel.

[Biomes! Fossils](https://steamcommunity.com/sharedfiles/filedetails/?id=3100958580)

Costumes don't cover FullHead.

[Hunterphage - Equipment Only](https://steamcommunity.com/sharedfiles/filedetails/?id=3430020141)

Its hats and Rogue, Phantom, and Crow Feathered hoods cover only UpperHead; the mod defines no FullHead apparel.

[Halloween Harvest](https://steamcommunity.com/sharedfiles/filedetails/?id=3811893311)

Every paired headdress covers only UpperHead; none covers FullHead. The body outfits cover the torso and other body groups, not the head. Because the costume pool filters entries unless either item covers FullHead, all 13 Halloween Harvest entries are discarded. The mod’s package ID matches the LoadFolders.xml condition, so the issue is eligibility—not the DefNames or load gate. This is consistent with keeping your strict FullHead rule; these costumes can’t be added without changing that rule or the apparel defs.
