using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace QuadrumYears;

/// <summary>
/// Birth abs-ticks encode life span with vanilla <see cref="GenDate.TicksPerYear"/>.
/// Remapped <see cref="GenDate.Year"/> would place birth ~4× further in the past; keep
/// vanilla year-length math here so birth year matches chronological/biological age labels.
/// </summary>
[HarmonyPatch]
internal static class Pawn_AgeTracker_BirthYear_Patch
{
  private static MethodBase TargetMethod() =>
    AccessTools.PropertyGetter(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BirthYear));

  [HarmonyPostfix]
  private static void Postfix(Pawn_AgeTracker __instance, ref int __result)
  {
    __result = GenDate.DefaultStartingYear
      + Mathf.FloorToInt(__instance.BirthAbsTicks / (float)GenDate.TicksPerYear);
  }
}
