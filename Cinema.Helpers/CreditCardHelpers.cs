using static Cinema.Constants.Messages;

namespace Cinema.Helpers
{
	public static class CreditCardHelpers
	{
		public static bool IsCreditCardValid(string name, string number, string expire, string ccv, out string message)
		{
			if (string.IsNullOrWhiteSpace(number) ||
				number.Length != 12 ||
				!long.TryParse(number, out _))
			{
				message = INVALID_NUMBER;
				return false;
			}

			if (string.IsNullOrWhiteSpace(expire) || 
				expire.Length != 5 ||
				!ValidateDate(expire))
			{
				message = INVALID_EXPIRATION;
				return false;
			}

			if (string.IsNullOrWhiteSpace(name) ||
				string.IsNullOrWhiteSpace(ccv) ||
				ccv.Length != 3)
			{
				message = PAYMENT_REFUSED;
				return false;
			}

			message = string.Empty;
			return true;
		}

		private static bool ValidateDate(string expire)
		{
			if (expire[2] != '/')
				return false;

			var components = expire.Split('/');
			if (components.Length != 2)
				return false;

			if (!int.TryParse(components[0], out var month) || !int.TryParse(components[1], out var year))
				return false;

			year += 2000;

			return year > DateTime.Now.Year || (year == DateTime.Now.Year && month >= DateTime.Now.Month);
		}
	}
}
