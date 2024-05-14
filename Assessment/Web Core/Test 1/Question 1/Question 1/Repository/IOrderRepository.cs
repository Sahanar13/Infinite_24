using Question_1.Models;

namespace Question_1.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int id);
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(int id);
        object PlaceOrder(Order order);
        Task<string?> GetOrderByOrderId(int id);
        Task<string?> GetOrderDetailsByOrderId(int id);
        Task<string?> GetCustomersByOrderDate(DateTime orderDate);
        Task<string?> GetCustomerWithHighestOrder();
    }
}
