using Cinema.Helpers.Constants;

namespace Cinema.Helpers
{
	public static class DateHelpers
	{
		public static (int day, int month, int year) GetTodayIfInvalid(int? day, int? month, int? year)
		{
			if (year is null || year < Time.MIN_YEAR || year > Time.MAX_YEAR)
				year = DateTime.Now.Year;
			
			if (month is null || month < 1 || month > 12)
				month = DateTime.Now.Month;

			if (day is null || day < 1 || day > DateTime.DaysInMonth((int)year, (int)month))
				day = DateTime.Now.Day;

			return ((int)day, (int)month, (int)year);
		}
	}
}
