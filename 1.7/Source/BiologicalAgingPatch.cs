using System.Reflection;
using System.Text;
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

[HarmonyPatch]
internal static class Pawn_AgeTracker_AgeChronologicalYears_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeChronologicalYears));

  [HarmonyPostfix]
  private static void Postfix(Pawn_AgeTracker __instance, ref int __result)
  {
    __result = ChronologicalAgeUtility.Years(__instance.AgeChronologicalTicks);
  }
}

[HarmonyPatch]
internal static class Pawn_AgeTracker_AgeChronologicalYearsFloat_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeChronologicalYearsFloat));

  [HarmonyPostfix]
  private static void Postfix(Pawn_AgeTracker __instance, ref float __result)
  {
    __result = ChronologicalAgeUtility.YearsFloat(__instance.AgeChronologicalTicks);
  }
}

/// <summary>
/// Rebuild age tooltips so chronological age uses calendar-year length and neither age shows quadrums.
/// </summary>
[HarmonyPatch]
internal static class Pawn_AgeTracker_AgeTooltipString_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTooltipString));

  [HarmonyPrefix]
  private static bool Prefix(Pawn_AgeTracker __instance, ref string __result)
  {
    QuadrumTime.FoldAgePeriodQuadrums = true;
    try
    {
      GenDate.TicksToPeriod(__instance.AgeBiologicalTicks, out var bioYears, out var bioQuadrums, out var bioDays, out _);
      ChronologicalAgeUtility.TicksToCalendarAgePeriod(
        __instance.AgeChronologicalTicks,
        out var chronoYears,
        out var chronoDays);

      var birthDate = "FullDate".Translate(
        Find.ActiveLanguageWorker.OrdinalNumber(__instance.BirthDayOfSeasonZeroBased + 1),
        __instance.BirthQuadrum.Label(),
        __instance.BirthYear);

      var sb = new StringBuilder();
      sb.AppendLine("Born".Translate(birthDate));
      sb.AppendLine("AgeChronological".Translate(chronoYears, 0, chronoDays));
      sb.Append("AgeBiological".Translate(bioYears, bioQuadrums, bioDays));

      if (Prefs.DevMode)
      {
        sb.AppendLine().AppendLine().Append("Dev mode info:");
        sb.Append("\nageBiologicalTicksInt: ").Append(__instance.AgeBiologicalTicks);
        sb.Append("\nbirthAbsTicksInt: ").Append(__instance.BirthAbsTicks);
        sb.Append("\nBiologicalTicksPerTick: ").Append(__instance.BiologicalTicksPerTick);
      }

      __result = sb.ToString();
      return false;
    }
    finally
    {
      QuadrumTime.FoldAgePeriodQuadrums = false;
    }
  }
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
