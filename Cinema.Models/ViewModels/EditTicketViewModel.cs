﻿namespace Cinema.Models.ViewModels
{
	public class EditTicketViewModel
	{
		public TicketInfo OldTicket { get; set; } = null!;

		public Ticket NewTicket { get; set; } = null!;
	}
}
