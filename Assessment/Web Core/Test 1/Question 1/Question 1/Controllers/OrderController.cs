using Microsoft.AspNetCore.Mvc;
using Question_1.Models;
using Question_1.Repositories;
using Question_1.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Question_1.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                await _orderRepository.PlaceOrder(order);
                return RedirectToAction(nameof(OrderDetails, _orderRepository: Get_orderRepository()), new { id = order.OrderId });
            }

            return View(order);
        }

        private string? nameof(Func<int, IOrderRepository, Task<IActionResult>> orderDetails, IOrderRepository _orderRepository)
        {
            throw new NotImplementedException();
        }

        public IOrderRepository Get_orderRepository()
        {
            return _orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id, IOrderRepository _orderRepository)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDetails = await _orderRepository.GetOrderDetailsByOrderId(id);
            return View(orderDetails);
        }

        [HttpGet]
        public async Task<IActionResult> DisplayBill(int id)
        {
            var order = await _orderRepository.GetOrderByOrderId(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetailsByOrderDate(DateTime orderDate)
        {
            var customers = await _orderRepository.GetCustomersByOrderDate(orderDate);
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerWithHighestOrder()
        {
            var customer = await _orderRepository.GetCustomerWithHighestOrder();
            return View(customer);
        }
    }
}
