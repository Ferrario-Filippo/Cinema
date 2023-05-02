namespace Cinema.Models.ViewModels
{
    public sealed class UserHistoryViewModel
    {
        public IEnumerable<Ticket> RefundableTickets { get; set; } = null!;

        public IEnumerable<Ticket> NonRefundableTickets { get; set; } = null!;

        public IEnumerable<UserReviewViewModel> FilmsWatched { get; set; } = null!;
    }
}
