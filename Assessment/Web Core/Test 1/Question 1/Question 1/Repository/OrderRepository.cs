using Question_1.Models;
using Question_1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Question_1.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EkartContext _dbContext;

        public OrderRepository(EkartContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> PlaceOrder(Order order)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _dbContext.Orders.FindAsync(id);
        }

        public async Task<List<OrderDetail>> GetOrderDetailsByOrderId(int orderId)
        {
            return await _dbContext.OrderDetails.Where(od => od.OrderId == orderId).ToListAsync();
        }

        public async Task<Order> GetOrderByOrderId(int orderId)
        {
            return await _dbContext.Orders.FindAsync(orderId);
        }

        public async Task<List<Customer>> GetCustomersByOrderDate(DateTime orderDate)
        {
            return await _dbContext.Customers
                .Where(c => c.Orders.Any(o => o.OrderDate == orderDate))
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerWithHighestOrder()
        {
            var customerWithHighestOrder = await _dbContext.Customers
                .OrderByDescending(c => c.Orders.Sum(o => o.TotalAmount))
                .FirstOrDefaultAsync();

            return customerWithHighestOrder;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            throw new NotImplementedException();
        }

        Order IOrderRepository.GetOrderById(int id)
        {
            throw new NotImplementedException();
        }

        public void AddOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(int id)
        {
            throw new NotImplementedException();
        }

        object IOrderRepository.PlaceOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
