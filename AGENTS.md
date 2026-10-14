# AGENTS.md — Quadrum Years

Core guidelines for agents working in this repository. Follow them unless the user explicitly overrides them for a task. Prefer durable conventions over snapshots of the tree; discover current files, versions, and APIs from the repo and declared local paths rather than assuming a fixed inventory here.

## Mod purpose

This RimWorld mod reinterprets the in-game calendar so that a **year** aligns with a **quadrum** (Guild-style timekeeping): time and labeling should feel like each quadrum is a full year.

That premise is the north star. Features, patches, defs, and balance may expand or shift as the mod develops—calendar display, simulation timing, related gameplay systems, and whatever else the user scopes in—as long as work stays coherent with that larger timekeeping idea. Do not treat this file as a frozen feature list or exclude future work that still fits the premise.

Identity (keep stable unless the user changes them deliberately):

- Package id: `Pupique.QuadrumYears`
- Harmony id: `Pupique.QuadrumYears`
- Meta and display strings live in `About/`; treat them as source of truth for name, author, dependencies, and supported versions.

## Hard restrictions

Non-negotiable without clear user consent for that action.

### 1. Repository directory boundaries

- Do not read, write, modify, install, delete, or otherwise target paths **outside** this repository root without consent.
- **Exception:** paths declared in the machine-local `Local.props` for the version tree being built (`RimWorldManagedDir`, `HarmonyAssemblyDir`, and any future properties added there). Those may be checked and read autonomously for builds, reference validation, and game-API inspection.
- Still ask before changing `Local.props` values, writing under those external trees, or exploring paths not listed in `Local.props`.
- Prefer solving problems with files already in this repo when that is enough.

### 2. Git write commands

- Do not autonomously run **write** git operations (`add`, `commit`, `push`, `pull`, `merge`, `rebase`, `stash`, branch switches that alter the working tree, `reset`, `tag`, `clean`, remote mutation, etc.).
- Read-only git (`status`, `diff`, `log`, `show`) is fine.
- Perform write git actions only when the user explicitly asks.

### 3. Versioned mod layout

RimWorld loads content through `LoadFolders.xml` and per-game-version directories.

- Keep version-specific code, assemblies, defs, and related assets under the matching version folder (`<major.minor>/…`).
- Keep shared mod metadata at the repository root (`About/`, `LoadFolders.xml`, this file, and similar).
- Prefer a full mirrored tree per supported game version, but stay flexible: new versions may largely copy and adapt a prior tree when APIs allow.
- When adding or updating a game version: sync the version directory, `LoadFolders.xml`, and `About` supported-versions metadata; keep that version’s project and local build props consistent with how the rest of the repo builds.
- Do not place version-specific loadable content at the root in ways that bypass `LoadFolders.xml`.
- Do not invent a parallel loading scheme unless the user redesigns it.

## Build and local configuration

- Each supported game version that ships C# should have its own project under that version’s `Source/` (or the layout already established for that version).
- Machine-specific paths belong only in gitignored `Local.props`. Commit a `Local.props.example` with placeholders beside it; never commit real machine paths.
- `Local.props` must supply whatever the project validates (at minimum the managed game assembly directory and the Harmony assembly directory, unless the project evolves).
- Build outputs for a version must land where `LoadFolders.xml` loads them for that version (typically that version’s `Assemblies/`).
- Do not commit `bin/`, `obj/`, or other local build intermediates covered by `.gitignore`.
- Restore/build with the version’s project file after `Local.props` exists; confirm the expected DLL (or other loadable outputs) update under the correct version folder.

## Code and patching guidelines

- Stay consistent with existing project namespace, naming, and style in the version tree you edit.
- Keep Harmony id aligned with package identity unless the user intentionally remaps both.
- Prefer small, targeted Harmony patches (or other hooks) over broad rewrites; verify real game member signatures via `Local.props` assemblies before attributing patches.
- Prefix player-facing log lines so they are easy to filter (existing convention: `[Quadrum Years]`).
- Avoid unused scaffolding, assets, or defs unless the task needs them.
- Ask when the desired behavior for a system under the timekeeping premise is ambiguous; do not invent a conflicting interpretation of “year” vs “quadrum” without checking.

## Agent workflow

1. Discover the current version trees, load folders, and sources from the repo; do not assume a single-version stub forever.
2. Edit the version(s) relevant to the task; keep `LoadFolders.xml` and `About` in sync when load paths or supported versions change.
3. Stay inside the repo except for read/check access to `Local.props` paths.
4. After shipping behavioral/binary changes, remind the user to restart RimWorld or reload mods so new assemblies are picked up.
5. Do **not** start RimWorld autonomously.
6. For load/runtime failures, ask for a `Player.log` excerpt (or equivalent) rather than scraping undeclared log paths outside the repo.
7. Do not expand into unrelated mods, publishing, workshop tooling, or identity renames unless asked.
8. Do not autonomously commit, push, or otherwise mutate git history/state.

## Done criteria (general)

- The touched version tree builds cleanly with that version’s local props.
- Loadable outputs and `LoadFolders.xml` / `About` metadata agree for the versions involved.
- No machine-local `Local.props` or secrets are committed; examples remain placeholders.
- Changes advance or support the mod’s timekeeping premise within the scope the user set for the task.
