# Quadrums as Years

A RimWorld mod that reinterprets the in-game calendar so that **one year equals one quadrum** (Guild-style timekeeping). Seasonal length and order stay familiar; years simply arrive much sooner.

**Package ID:** `Pupique.QuadrumYears`  
**Author:** Pupique  
**License:** [GPL-3.0](LICENSE)

Supported game versions and the player-facing description live in [`Mod/About/About.xml`](Mod/About/About.xml)—treat that as the source of truth when they change.

## Premise

In vanilla RimWorld, a colony can climb from a fledgling tribe to a spacefaring power in little more than a decade. The same chieftain who founded the settlement is often still around as starship captain. Shortening seasons would fix the year length but would also disrupt the seasonal cadence that makes the game feel right, and it would demand a huge amount of content patching.

This mod takes a different approach, inspired by *The Guild*’s calendar: treat each **quadrum** as a full **calendar year**. The planet’s seasonal rhythm is left alone, while calendar labeling—and systems that should stay coherent with “a year”—are brought into line with that shorter year. Aging and related life-cycle timing are part of that coherence, so generations matter more and long campaigns feel less like a single lifetime of unbroken progress.

Exact gameplay effects may expand or shift as the mod develops; the north star is the Guild-style year, not any one patch list.

## How the repository is organized

Only the [`Mod/`](Mod/) directory is what RimWorld (and Steam Workshop) should load. Everything else is development metadata and source.

| Path | Role |
|------|------|
| `Mod/` | Loadable mod root (`About/`, `LoadFolders.xml`, versioned content) |
| `Mod/About/` | Mod metadata and preview |
| `Mod/<version>/` | Assemblies and other assets for that game version |
| `<version>/Source/` | C# project and Harmony patches (not shipped inside `Mod/`) |

`LoadFolders.xml` wires each supported RimWorld release to its folder under `Mod/` (for example `Mod/1.7/`).

Adding another game version means mirroring the version tree under `Mod/`, adding a matching `Source/` project when needed, then updating `Mod/LoadFolders.xml` and the supported-versions list in `Mod/About/About.xml`.

## Requirements

### To play

- A supported RimWorld version (see `Mod/About/About.xml`)
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077) (`brrainz.harmony`)

Enable this mod after Harmony. Restart RimWorld or reload mods after updating assemblies.

### To build from source

- A [.NET SDK](https://dotnet.microsoft.com/download) compatible with the version project’s `TargetFramework`
- A matching RimWorld install (managed game assemblies for compile references)

Machine-specific paths are not committed. Each version’s project uses a gitignored `Local.props` beside the `.csproj` (see below).

## Building

Commands below use the current `1.7` tree as an example; substitute the version folder you are building.

1. Clone this repository somewhere convenient (not necessarily inside RimWorld’s `Mods` folder).

2. Configure local paths:

   ```bash
   cp 1.7/Source/Local.props.example 1.7/Source/Local.props
   ```

   Edit `Local.props` and set `RimWorldManagedDir` to the directory that contains `Assembly-CSharp.dll`. Optionally set `RimWorldModsDir` or `RimWorldModInstallDir` if the default deploy path derived from `RimWorldManagedDir` is wrong for your install. The example file has typical Steam path hints.

3. Restore and build:

   ```bash
   dotnet restore 1.7/Source/QuadrumYears.csproj
   dotnet build 1.7/Source/QuadrumYears.csproj -c Release
   ```

   Output lands under `Mod/1.7/Assemblies/`. After a successful build, the project mirrors `Mod/` into RimWorld’s `Mods/QuadrumYears` folder.

4. Load the mod in-game with Harmony enabled, then restart or reload mods so the new assembly is picked up.

Do not commit `Local.props`, `bin/`, `obj/`, or built `*.dll` files (already covered by `.gitignore`).

## Contributing

Issues and forks are welcome. Keep changes coherent with the core idea: **a year should feel like a quadrum**, without tearing up seasonal timing unless that is an intentional design change.

For agent/tooling conventions used in this repo, see [AGENTS.md](AGENTS.md).

## License

This project is licensed under the [GNU General Public License v3.0](LICENSE).
