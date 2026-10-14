using System.Reflection;
using HarmonyLib;
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
    __result = (int)(__instance.AgeChronologicalTicks / QuadrumTime.TicksPerCalendarYear);
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
    __result = __instance.AgeChronologicalTicks / (float)QuadrumTime.TicksPerCalendarYear;
  }
}
