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

			for (var lane = 0; remainingSeats > 0; lane++)
			{
				var seatsPerLane = GetMaxSeatsPerLane(lane);
				for (ushort number = 1; number <= seatsPerLane && remainingSeats > 0; number++)
				{
					yield return new()
					{
						Cost = BASE_TICKET_COST + (show.Is3D && lane > 4 ? 2 : 0),
						ShowId = show.ShowId,
						Lane = (char)(A_ASCII + lane),
						Number = number
					};
					--remainingSeats;
				}
			}
		}

		public static int GetMaxSeatsPerLane(int lane)
		{
			return BASE_SEATS + SEATS_INCREASE * (lane < 7 ? lane : 6);
		}
	}
}
