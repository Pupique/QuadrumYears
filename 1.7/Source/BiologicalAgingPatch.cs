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
/// Rebuild age tooltips so both ages use a 15-day year. Life-years still come from
/// biological ticks divided by a vanilla year, because those ticks already advance at 4×.
/// </summary>
[HarmonyPatch]
internal static class Pawn_AgeTracker_AgeTooltipString_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.AgeTooltipString));

  [HarmonyPrefix]
  private static bool Prefix(Pawn_AgeTracker __instance, ref string __result)
  {
    TicksToBiologicalAgePeriod(__instance.AgeBiologicalTicks, out var bioYears, out var bioDays);
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
    sb.Append("AgeBiological".Translate(bioYears, 0, bioDays));

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

  /// <summary>
  /// One life-year is still <see cref="GenDate.TicksPerYear"/> biological ticks.
  /// The remainder is shown as 0–14 days, matching a calendar year of one quadrum.
  /// </summary>
  private static void TicksToBiologicalAgePeriod(long ticks, out int years, out int days)
  {
    if (ticks < 0)
    {
      ticks = 0;
    }

    years = (int)(ticks / GenDate.TicksPerYear);
    var ticksPerCalendarDay = GenDate.TicksPerYear / GenDate.DaysPerQuadrum;
    days = (int)(ticks % GenDate.TicksPerYear / ticksPerCalendarDay);
  }
}
