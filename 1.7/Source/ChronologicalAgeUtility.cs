using RimWorld;
using Verse;

namespace QuadrumYears;

/// <summary>
/// Chronological age has no engine multiplier. Store and display it in calendar-year
/// length (one quadrum) without copying biological ticks, so spawn gaps stay intact.
/// </summary>
internal static class ChronologicalAgeUtility
{
  /// <summary>
  /// Shrink a vanilla chronological span (<see cref="GenDate.TicksPerYear"/> per year)
  /// to one calendar year per quadrum. Leaves biological ticks alone.
  /// </summary>
  public static void ScaleVanillaSpanToCalendarYears(Pawn_AgeTracker ageTracker)
  {
    if (ageTracker == null)
    {
      return;
    }

    var ticks = ageTracker.AgeChronologicalTicks;
    ageTracker.AgeChronologicalTicks = ticks * QuadrumTime.TicksPerCalendarYear / GenDate.TicksPerYear;
  }

  public static int Years(long ticks) =>
    (int)(ticks / QuadrumTime.TicksPerCalendarYear);

  public static float YearsFloat(long ticks) =>
    ticks / (float)QuadrumTime.TicksPerCalendarYear;

  public static void TicksToCalendarAgePeriod(long ticks, out int years, out int days)
  {
    if (ticks < 0)
    {
      ticks = 0;
    }

    years = Years(ticks);
    days = (int)(ticks % QuadrumTime.TicksPerCalendarYear / GenDate.TicksPerDay);
  }
}
