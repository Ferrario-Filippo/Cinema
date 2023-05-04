using Cinema.DataAccess.Repository.Interfaces;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
	internal sealed class OrdersRepository : Repository<Order>, IOrdersRepository
	{
		private readonly CinemaDbContext _db;

		public OrdersRepository(CinemaDbContext db) : base(db)
		{
			_db = db;
		}

		public void UpdateStatus(int orderId, string orderStatus, string? paymentStatus = null)
		{
			var order = _db.Orders.Find(orderId);

			if (order is null || string.IsNullOrWhiteSpace(paymentStatus))
				return;

			order.OrderStatus = orderStatus;
			order.PaymentStatus = paymentStatus;
		}

		public void UpdateStripeSessionId(int orderId, string sessionId)
		{
			var order = _db.Orders.Find(orderId);

			if (order is not null)
				order.SessionId = sessionId;
		}

		public void UpdateStripePaymentIntentId(int orderId, string paymentIntentId)
		{
			var order = _db.Orders.Find(orderId);

			if (order is not null)
				order.PaymentIntentId = paymentIntentId;
		}
	}
}
