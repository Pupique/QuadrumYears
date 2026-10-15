using System;
using RimWorld;

namespace QuadrumYears;

/// <summary>
/// Shared calendar/aging constants: one Guild-style year equals one vanilla quadrum.
/// </summary>
internal static class QuadrumTime
{
  /// <summary>Ticks in one calendar year under this mod (vanilla <see cref="GenDate.TicksPerQuadrum"/>).</summary>
  public const int TicksPerCalendarYear = GenDate.TicksPerQuadrum;

  /// <summary>
  /// Biological aging speed relative to vanilla so one life-year accumulates per calendar year
  /// (<see cref="GenDate.TicksPerYear"/> / <see cref="GenDate.TicksPerQuadrum"/>).
  /// </summary>
  public const float AgingRateMultiplier = (float)GenDate.TicksPerYear / GenDate.TicksPerQuadrum;

  /// <summary>
  /// When true, <see cref="GenDate.TicksToPeriod"/> folds leftover quadrums into days
  /// (used while building pawn age tooltips).
  /// </summary>
  [ThreadStatic]
  public static bool FoldAgePeriodQuadrums;
}
