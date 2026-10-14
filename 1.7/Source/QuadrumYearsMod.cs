using System.Reflection;
using HarmonyLib;
using Verse;

namespace QuadrumYears;

public sealed class QuadrumYearsMod : Mod
{
  public QuadrumYearsMod(ModContentPack content) : base(content)
  {
    var harmony = new Harmony("Pupique.QuadrumYears");
    Log.Message("[Quadrum Years] Applying Harmony patches...");
    harmony.PatchAll(Assembly.GetExecutingAssembly());
    Log.Message("[Quadrum Years] Harmony patches applied.");
  }
}
