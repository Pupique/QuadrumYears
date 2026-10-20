# AGENTS.md — RimWorld mod repository

Core guidelines for agents working in this repository. Follow them unless the user explicitly overrides them for a task. Prefer durable conventions over snapshots of the tree; discover current files, versions, and APIs from the repo and declared local paths rather than assuming a fixed inventory here.

## Additional agent context (`.agents/`)

The `.agents/` directory holds supplementary guidance for agents. Treat only the files enumerated below as authoritative agent context; ignore any other files that may appear under `.agents/` unless the user or this file later lists them.

| File | Purpose |
|------|---------|
| [`.agents/OVERVIEW.md`](.agents/OVERVIEW.md) | Canonical mod knowledge base for agents: identity, purpose/premise, package/Harmony ids, log prefix, deploy folder, supported versions, and related conventions. Distilled for agents—prefer it over parsing `Mod/About/` XML. Read it when starting work so changes stay coherent with this mod. When you change About metadata or those conventions, update this file to match. |

Pointers in this section stay mod-agnostic: concrete mod values live only in the enumerated files above.

## Hard restrictions

Non-negotiable without clear user consent for that action.

### 1. Repository directory boundaries

- Do not read, write, modify, install, delete, or otherwise target paths **outside** this repository root without consent.
- **Exception:** paths declared in the machine-local `Local.props` for the version tree being built (`RimWorldManagedDir`, optional `RimWorldModsDir` / `RimWorldModInstallDir`, and any future properties added there), and the machine-local `paths.env` used by `deploy.sh` (`RIMWORLD_MODS_DIR`). Those may be checked and read autonomously for builds, deploys, reference validation, and game-API inspection.
- **Allowed external writes** (when those paths resolve from `Local.props` / `paths.env`):
  - After a successful C# build: **assemblies only** into the live install’s `Mods/<mod folder>/<version>/Assemblies/` (overwrite; do not wipe the rest of the install).
  - Via `./deploy.sh`: a full wipe-and-replace of `Mods/<mod folder>` with the repo’s `Mod/` tree.
- Take the concrete `<mod folder>` name from `.agents/OVERVIEW.md`.
- Still ask before changing `Local.props` / `paths.env` values, writing under other external trees, or exploring paths not listed in those files.
- Prefer solving problems with files already in this repo when that is enough.

### 2. Git write commands

- Do not autonomously run **write** git operations (`add`, `commit`, `push`, `pull`, `merge`, `rebase`, `stash`, branch switches that alter the working tree, `reset`, `tag`, `clean`, remote mutation, etc.).
- Read-only git (`status`, `diff`, `log`, `show`) is fine.
- Perform write git actions only when the user explicitly asks.

### 3. Versioned mod layout

RimWorld loads content through `LoadFolders.xml` and per-game-version directories. In this repository, **only `Mod/` is the loadable mod root** (what belongs in RimWorld’s `Mods` folder / Steam Workshop upload).

- Keep loadable metadata at `Mod/About/` and `Mod/LoadFolders.xml`.
- Keep version-specific loadable content under `Mod/<major.minor>/…` (for example assemblies under `Mod/1.7/Assemblies/`).
- Keep C# projects under `<major.minor>/Source/` at the repository root (outside `Mod/`), so source is not shipped with the loadable tree.
- Prefer a full mirrored loadable tree per supported game version under `Mod/`, but stay flexible: new versions may largely copy and adapt a prior tree when APIs allow.
- When adding or updating a game version: sync `Mod/<version>/`, `Mod/LoadFolders.xml`, and `Mod/About` supported-versions metadata; keep that version’s project and local build props consistent with how the rest of the repo builds.
- Do not place version-specific loadable content outside `Mod/` in ways that bypass `LoadFolders.xml`.
- Do not invent a parallel loading scheme unless the user redesigns it.
- Steam / workshop publishing should target `Mod/` only—not the whole git repository.

## Build and local configuration

- Each supported game version that ships C# should have its own project under that version’s `Source/` (or the layout already established for that version).
- Machine-specific paths belong only in gitignored files: `Local.props` (per version `Source/`) and repo-root `paths.env`. Commit the matching `*.example` files with placeholders; never commit real machine paths.
- `Local.props` must supply whatever the project validates (at minimum `RimWorldManagedDir`). Optional `RimWorldModsDir` or `RimWorldModInstallDir` override where the post-build **assembly** copy goes.
- Build outputs for a version must land where `LoadFolders.xml` loads them for that version (`Mod/<version>/Assemblies/`). The project’s `OutputPath` should target that folder (typically via a `RimWorldGameVersion` property on the `.csproj`).
- After a successful build, the project also copies built `*.dll` files into the live install’s `Mods/<mod folder>/<version>/Assemblies/` (same resolution order as in the `.csproj`: explicit install dir → `RimWorldModsDir` → derived from `RimWorldManagedDir`). It must **not** delete or rewrite other install content; that is `deploy.sh`’s job.
- Do not commit `bin/`, `obj/`, or other local build intermediates covered by `.gitignore`.
- Restore/build with the version’s project file after `Local.props` exists; confirm the expected DLL updates under both `Mod/<version>/Assemblies/` and the live install Assemblies path when that install exists.

## Full mod deploy (`deploy.sh`)

Repo-root [`deploy.sh`](deploy.sh) mirrors the loadable `Mod/` tree into RimWorld’s `Mods/<mod folder>` (folder name from `.agents/OVERVIEW.md`).

- **Config:** gitignored `paths.env` (from `paths.env.example`) must set `RIMWORLD_MODS_DIR` to the parent Mods directory.
- **Behavior:** refuses destinations whose basename is not the expected mod folder; clears that folder if present; copies `Mod/` into it (full wipe-and-replace).
- **When to use:** after changing non-assembly loadable content (`About/`, `LoadFolders.xml`, defs, textures, etc.), or to create/refresh a complete live install. Not required for every C# rebuild—builds already refresh assemblies in place.
- **Agents may run `./deploy.sh` autonomously** when the task context calls for syncing loadable content to the live install (and `paths.env` is configured). Prefer it over inventing a parallel copy/rsync. If `paths.env` is missing, ask the user or point them at `paths.env.example` rather than inventing paths.

## Code and patching guidelines

- Stay consistent with existing project namespace, naming, and style in the version tree you edit.
- Keep Harmony id aligned with package identity unless the user intentionally remaps both (see `.agents/OVERVIEW.md` for current ids).
- Prefer small, targeted Harmony patches (or other hooks) over broad rewrites; verify real game member signatures via `Local.props` assemblies before attributing patches.
- Prefix player-facing log lines so they are easy to filter (see `.agents/OVERVIEW.md` for the existing convention).
- Avoid unused scaffolding, assets, or defs unless the task needs them.
- Ask when desired behavior under the mod’s premise is ambiguous; do not invent conflicting interpretations without checking (see `.agents/OVERVIEW.md`).

## Local scratch (`tmp/`)

Use the repository-local `tmp/` directory (create it if missing) for disposable agent work products that should survive OS restarts and `/tmp` wipes. Typical contents: DLL/API inspectors, decompiled or reflected member dumps, signature notes, and similar scratch used while authoring Harmony patches or validating game APIs.

- Prefer `tmp/` over system `/tmp/` (or other volatile OS temp paths) for these artifacts.
- Before regenerating inspection or API research, check `tmp/` for leftovers from prior agents; reuse or extend what is already there when it is still relevant.
- Keep `tmp/` gitignored (already listed in `.gitignore`); do not commit its contents.
- Do not treat `tmp/` as loadable mod content or place versioned source of truth there.

## Agent workflow

1. Read `.agents/OVERVIEW.md` for mod identity and premise. Discover the current version trees, load folders, and sources from the repo; do not assume a single-version stub forever. Also skim `tmp/` when the task may need game-API or assembly inspection—prior sessions may already have useful dumps.
2. Edit the version(s) relevant to the task; keep `Mod/LoadFolders.xml` and `Mod/About` in sync when load paths or supported versions change.
3. Stay inside the repo except for read/check access to `Local.props` / `paths.env` paths, the post-build assembly copy into `Mods/<mod folder>/<version>/Assemblies/`, and optional `./deploy.sh` full mirrors into `Mods/<mod folder>`. Write inspection/scratch dumps under repo `tmp/`, not system temp.
4. After shipping behavioral/binary changes, remind the user to restart RimWorld or reload mods so new assemblies are picked up.
5. Do **not** start RimWorld autonomously.
6. For load/runtime failures, ask for a `Player.log` excerpt (or equivalent) rather than scraping undeclared log paths outside the repo.
7. Do not expand into unrelated mods, publishing, workshop tooling, or identity renames unless asked.
8. Do not autonomously commit, push, or otherwise mutate git history/state.

## Done criteria (general)

- The touched version tree builds cleanly with that version’s local props.
- Loadable outputs and `Mod/LoadFolders.xml` / `Mod/About` metadata agree for the versions involved.
- No machine-local `Local.props`, `paths.env`, or secrets are committed; examples remain placeholders.
- Changes advance or support the mod’s purpose within the scope the user set for the task (see `.agents/OVERVIEW.md`).
