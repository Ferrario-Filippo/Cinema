using Cinema.Models;

namespace Cinema.DataAccess.Repository.Interfaces
{
	public interface IOrdersRepository : IRepository<Order>
	{
		void UpdateStatus(int orderId, string orderStatus, string? paymentStatus = null);
		
		void UpdateStripeSessionId(int orderId, string sessionId);

		void UpdateStripePaymentIntentId(int orderId, string paymentIntentId);
	}
}
