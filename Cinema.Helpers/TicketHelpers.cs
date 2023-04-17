using Cinema.Models;

namespace Cinema.Helpers
{
	public static class TicketHelpers
	{
		private const int BASE_SEATS = 16;

		private const int SEATS_INCREASE = 2;

		private const int A_ASCII = 65;

		private const double BASE_TICKET_COST = 4.9;

		public static IEnumerable<Ticket> CreateTicketsForShow(Show show, Room associatedRoom)
		{
			var remainingSeats = associatedRoom.Seats;
			var cost = BASE_TICKET_COST + (show.Is3D ? 2 : 0);

			for (var lane = 0; remainingSeats > 0; lane++)
			{
				for (ushort number = 1; number <= (BASE_SEATS + SEATS_INCREASE * lane) && remainingSeats > 0; number++)
				{
					yield return new()
					{
						Cost = cost,
						ShowId = show.ShowId,
						Lane = (char)(A_ASCII + lane),
						Number = number
					};
					--remainingSeats;
				}
			}
		}
	}
}
