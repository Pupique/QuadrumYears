using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace QuadrumYears;

[HarmonyPatch(typeof(GenDate), nameof(GenDate.Year))]
internal static class GenDate_Year_Patch
{
  [HarmonyPrefix]
  private static bool Prefix(long absTicks, float longitude, ref int __result)
  {
    var localTicks = absTicks + GenDate.LocalTicksOffsetFromLongitude(longitude);
    __result = GenDate.DefaultStartingYear
      + Mathf.FloorToInt(localTicks / (float)QuadrumTime.TicksPerCalendarYear);
    return false;
  }
}
