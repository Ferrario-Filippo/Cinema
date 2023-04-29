using System.Security.Claims;

namespace Cinema.Helpers
{
	public static class IdentityHelpers
	{
		public static bool TryGetUserIdentity(ClaimsPrincipal user, out Claim? identity)
		{
			identity = null;

			var userIdentity = user.Identity;
			if (userIdentity is null)
				return false;

			var claimsIdentity = (ClaimsIdentity)userIdentity;
			identity = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			return identity is not null;
		}
	}
}
