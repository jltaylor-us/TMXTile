**This is a custom fork of TMXTile 1.5.9 used by the Stardew Valley community.** It's bundled with
[SMAPI](https://github.com/Pathoschild/SMAPI) automatically. See
[Platonymous/TMXTile](https://github.com/Platonymous/TMXTile) for documentation and usage.

This fork is mostly identical to the official version, with a few changes specific to Stardew Valley:

* Migrated from .NET Framework 4.5 to .NET 6.
* Updated for the game's custom version of xTile. The main differences are:
  * `PropertyValue` no longer exists (replaced by string property values).
  * Tilesheets have a new `tileIndexPropertyDictionary` cache that must be populated when loading TMX files.

Here's a [list of changes](https://github.com/Pathoschild/TMXTile/compare/master...stardew-valley) in the
forked version.
