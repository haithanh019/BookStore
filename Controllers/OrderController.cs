using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly BookStoreContext _context;
        private readonly Cart _cart;

        public OrderController(BookStoreContext context, Cart cart)
        {
            _context = context;
            _cart = cart;
        }
        public IActionResult Index()
        {
            return View();
        }


        
        [HttpGet]
        public IActionResult Checkout()
        {
            return View(new Order());
        }


        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            var cartItem = _cart.GetAllCartItems();
            _cart.CartItems = cartItem;
            if (_cart.CartItems.Count == 0)
            {
                ModelState.AddModelError("","Cart is empty, please add the book first.");
            }

            if (ModelState.IsValid)
            {
                CreateOrder(order);
                _cart.ClearCart();
                return View("CheckoutComplete",order);
            }
            return View(order);
        }

        public IActionResult CheckoutComplete(Order order)
        {

            return View(order);
        }

        public void CreateOrder(Order order)
        {
            order.OrderPlaced = DateTime.Now;
            order.OrderItems = new List<OrderItem>();
            var cartItems = _cart.CartItems;

            foreach (var cartItem in cartItems)
            {
                var orderItems = new OrderItem()
                {
                    Quantity = cartItem.Quantity,
                    BookId = cartItem.Book.Id,
                    OrderId = order.Id,
                    Price = cartItem.Book.Price * cartItem.Quantity
                };
                order.OrderItems.Add(orderItems);
                order.OrderTotal += orderItems.Price;
            }
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
    }
}

