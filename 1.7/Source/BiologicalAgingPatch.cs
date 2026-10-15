using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace QuadrumYears;

[HarmonyPatch]
internal static class Pawn_AgeTracker_BiologicalTicksPerTick_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BiologicalTicksPerTick));

  [HarmonyPostfix]
  private static void Postfix(ref float __result)
  {
    __result *= QuadrumTime.AgingRateMultiplier;
  }
}

/// <summary>
/// Age tooltips pass years/quadrums/days to keyed strings. Fold leftover quadrums into days
/// so ages read as years + days only (quadrums are calendar years under this mod).
/// </summary>
[HarmonyPatch]
internal static class Pawn_AgeTracker_AgeTooltipString_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTooltipString));

  [HarmonyPrefix]
  private static void Prefix() => QuadrumTime.FoldAgePeriodQuadrums = true;

  [HarmonyPostfix]
  private static void Postfix() => QuadrumTime.FoldAgePeriodQuadrums = false;

  [HarmonyFinalizer]
  private static void Finalizer() => QuadrumTime.FoldAgePeriodQuadrums = false;
}

[HarmonyPatch]
internal static class GenDate_TicksToPeriod_Long_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.Method(
      typeof(GenDate),
      nameof(GenDate.TicksToPeriod),
      new[]
      {
        typeof(long),
        typeof(int).MakeByRefType(),
        typeof(int).MakeByRefType(),
        typeof(int).MakeByRefType(),
        typeof(float).MakeByRefType(),
      });

  [HarmonyPostfix]
  private static void Postfix(ref int quadrums, ref int days)
  {
    FoldQuadrumsIntoDays(ref quadrums, ref days);
  }

  internal static void FoldQuadrumsIntoDays(ref int quadrums, ref int days)
  {
    if (!QuadrumTime.FoldAgePeriodQuadrums || quadrums == 0)
    {
      return;
    }

    days += quadrums * GenDate.DaysPerQuadrum;
    quadrums = 0;
  }
}

[HarmonyPatch]
internal static class GenDate_TicksToPeriod_Int_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.Method(
      typeof(GenDate),
      nameof(GenDate.TicksToPeriod),
      new[]
      {
        typeof(int),
        typeof(int).MakeByRefType(),
        typeof(int).MakeByRefType(),
        typeof(int).MakeByRefType(),
        typeof(float).MakeByRefType(),
      });

  [HarmonyPostfix]
  private static void Postfix(ref int quadrums, ref int days)
  {
    GenDate_TicksToPeriod_Long_Patch.FoldQuadrumsIntoDays(ref quadrums, ref days);
  }
}
