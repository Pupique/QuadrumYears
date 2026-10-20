# Mod overview — Quadrum Years

Canonical agent knowledge base for this repository’s mod. Prefer this file over sifting `Mod/About/` XML when you need identity, purpose, or conventions.

General RimWorld agent rules live in [`AGENTS.md`](../AGENTS.md). Keep this overview in sync when you deliberately change the corresponding fields in `Mod/About/` (or related conventions in code).

## Purpose

Reinterpret the in-game calendar so a **year** aligns with a **quadrum** (Guild-style timekeeping): each quadrum should feel like a full year, without shortening seasons.

Premise implications (current scope): chronological years pass 4× faster relative to seasonal time; biological aging is aligned with that; demographic pressure from natural aging matters. Features may expand (calendar UI, simulation timing, related systems) as long as they stay coherent with this timekeeping idea.

Ask when desired behavior under that premise is ambiguous; do not invent a conflicting interpretation of “year” vs “quadrum” without checking.

## Identity

| Item | Value |
|------|--------|
| Display name | Quadrums as Years |
| Author | Pupique |
| Package id | `Pupique.QuadrumYears` |
| Harmony id | `Pupique.QuadrumYears` |
| Deploy folder under RimWorld `Mods/` | `QuadrumYears` |
| Player-facing log prefix | `[Quadrum Years]` |
| Supported game versions | 1.7 |
| Required dependency | Harmony (`brrainz.harmony`) |

Keep package id, Harmony id, deploy folder, and log prefix stable unless the user changes them deliberately.

## Done criteria (mod-specific)

- Changes advance or support the mod’s timekeeping premise within the scope the user set for the task.
