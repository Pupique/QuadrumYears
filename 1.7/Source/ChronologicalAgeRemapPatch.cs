using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace QuadrumYears;

/// <summary>
/// One-shot migration of vanilla chronological spans (3,600,000 ticks per year) onto the
/// calendar year (one quadrum). Does not assign chronological ticks from biological age.
/// </summary>
public sealed class QuadrumYearsWorldComponent : WorldComponent
{
  private bool chronologicalAgesRemapped;

  public QuadrumYearsWorldComponent(World world) : base(world)
  {
  }

  public override void ExposeData()
  {
    base.ExposeData();
    Scribe_Values.Look(ref chronologicalAgesRemapped, "qyChronologicalAgesRemapped");
  }

  public override void FinalizeInit(bool fromLoad)
  {
    base.FinalizeInit(fromLoad);

    // New games reach this before starting pawns exist. Mark the world migrated so the
    // later load path does not scale pawns that GenerateRandomAge already scaled.
    // Loaded games call this before maps are read, so pawn migration waits until LoadGame ends.
    if (fromLoad || chronologicalAgesRemapped)
    {
      return;
    }

    chronologicalAgesRemapped = true;
  }

  internal void MigrateChronologicalAgesIfNeeded()
  {
    if (chronologicalAgesRemapped)
    {
      return;
    }

    var count = 0;
    var seen = new HashSet<Pawn>();
    foreach (var pawn in PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead)
    {
      if (pawn?.ageTracker == null || !seen.Add(pawn))
      {
        continue;
      }

      ChronologicalAgeUtility.ScaleVanillaSpanToCalendarYears(pawn.ageTracker);
      count++;
    }

    chronologicalAgesRemapped = true;
    Log.Message($"[Quadrum Years] Scaled chronological age to calendar years for {count} pawn(s).");
  }
}

/// <summary>
/// Vanilla writes chronological age as years × <see cref="GenDate.TicksPerYear"/>.
/// Scale that final span once so birth year and the calendar readout agree.
/// </summary>
[HarmonyPatch]
internal static class PawnGenerator_GenerateRandomAge_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.Method(typeof(PawnGenerator), "GenerateRandomAge");

  [HarmonyPostfix]
  private static void Postfix(Pawn pawn)
  {
    ChronologicalAgeUtility.ScaleVanillaSpanToCalendarYears(pawn?.ageTracker);
  }
}

/// <summary>
/// These two paths call <c>GenerateRandomAge</c> and then overwrite chronological ticks
/// with another vanilla-length span. Scale that final value, not the earlier one.
/// </summary>
[HarmonyPatch]
internal static class MetalhorrorUtility_SpawnMetalhorror_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.Method(typeof(MetalhorrorUtility), nameof(MetalhorrorUtility.SpawnMetalhorror));

  [HarmonyPostfix]
  private static void Postfix(Pawn __result)
  {
    ChronologicalAgeUtility.ScaleVanillaSpanToCalendarYears(__result?.ageTracker);
  }
}

[HarmonyPatch]
internal static class QuestNode_Root_RefugeePodCrash_Baby_GeneratePawn_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.Method(typeof(QuestNode_Root_RefugeePodCrash_Baby), "GeneratePawn_NewTemp");

  [HarmonyPostfix]
  private static void Postfix(Pawn __result)
  {
    ChronologicalAgeUtility.ScaleVanillaSpanToCalendarYears(__result?.ageTracker);
  }
}

/// <summary>
/// Map pawns are scribed after <see cref="World.FinalizeInit"/>, so a load-time scale
/// has to run once the save has finished materializing them.
/// </summary>
[HarmonyPatch]
internal static class Game_LoadGame_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.Method(typeof(Game), nameof(Game.LoadGame));

  [HarmonyPostfix]
  private static void Postfix(Game __instance)
  {
    __instance.World?.GetComponent<QuadrumYearsWorldComponent>()?.MigrateChronologicalAgesIfNeeded();
  }
}
