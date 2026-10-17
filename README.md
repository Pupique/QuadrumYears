# Quadrums as Years

A RimWorld mod that reinterprets the in-game calendar so that **one year equals one quadrum** (Guild-style timekeeping). Seasonal length and order stay familiar; years simply arrive much sooner.

**Package ID:** `Pupique.QuadrumYears`  
**Author:** Pupique  
**License:** [GPL-3.0](LICENSE)

Supported game versions and the player-facing description live in [`About/About.xml`](About/About.xml)—treat that as the source of truth when they change.

## Premise

In vanilla RimWorld, a colony can climb from a fledgling tribe to a spacefaring power in little more than a decade. The same chieftain who founded the settlement is often still around as starship captain. Shortening seasons would fix the year length but would also disrupt the seasonal cadence that makes the game feel right, and it would demand a huge amount of content patching.

This mod takes a different approach, inspired by *The Guild*’s calendar: treat each **quadrum** as a full **calendar year**. The planet’s seasonal rhythm is left alone, while calendar labeling—and systems that should stay coherent with “a year”—are brought into line with that shorter year. Aging and related life-cycle timing are part of that coherence, so generations matter more and long campaigns feel less like a single lifetime of unbroken progress.

Exact gameplay effects may expand or shift as the mod develops; the north star is the Guild-style year, not any one patch list.

## How the repository is organized

Shared mod metadata sits at the repository root (`About/`, `LoadFolders.xml`, this file). Loadable content is versioned: each supported RimWorld release has its own folder (for example `1.7/`), wired through `LoadFolders.xml`.

Within a version folder you will typically find:

- **Source/** — C# project and Harmony patches that implement the timekeeping changes
- **Assemblies/** — build output loaded by the game
- **Languages/** and other assets as needed

Adding another game version means mirroring that tree, then updating `LoadFolders.xml` and the supported-versions list in `About/About.xml`.

## Requirements

### To play

- A supported RimWorld version (see `About/About.xml`)
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077) (`brrainz.harmony`)

Enable this mod after Harmony. Restart RimWorld or reload mods after updating assemblies.

### To build from source

- A [.NET SDK](https://dotnet.microsoft.com/download) compatible with the version project’s `TargetFramework`
- A matching RimWorld install (managed game assemblies for compile references)
- Harmony assemblies for that same game version (`0Harmony.dll`)

Machine-specific paths are not committed. Each version’s project uses a gitignored `Local.props` beside the `.csproj` (see below).

## Building

Commands below use the current `1.7` tree as an example; substitute the version folder you are building.

1. Clone this repository (into RimWorld’s `Mods` folder, or elsewhere if you prefer to copy/symlink the loadable tree).

2. Configure local paths:

   ```bash
   cp 1.7/Source/Local.props.example 1.7/Source/Local.props
   ```

   Edit `Local.props` and set `RimWorldManagedDir` and `HarmonyAssemblyDir` to the directories that contain `Assembly-CSharp.dll` and `0Harmony.dll` respectively. The example file has typical Steam path hints.

3. Restore and build:

   ```bash
   dotnet restore 1.7/Source/QuadrumYears.csproj
   dotnet build 1.7/Source/QuadrumYears.csproj -c Release
   ```

   Output lands under that version’s `Assemblies/` folder, where `LoadFolders.xml` expects it.

4. Load the mod in-game with Harmony enabled, then restart or reload mods so the new assembly is picked up.

Do not commit `Local.props`, `bin/`, `obj/`, or built `*.dll` files (already covered by `.gitignore`).

## Contributing

Issues and forks are welcome. Keep changes coherent with the core idea: **a year should feel like a quadrum**, without tearing up seasonal timing unless that is an intentional design change.

For agent/tooling conventions used in this repo, see [AGENTS.md](AGENTS.md).

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).
