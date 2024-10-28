using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CartController : Controller
    {
        private readonly BookStoreContext _context;

        private readonly Cart _cart;

        public CartController(Cart cart, BookStoreContext context)
        {
            _cart = cart;
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _cart.GetAllCartItems();
            _cart.CartItems = items;
            return View(_cart);
        }

        public IActionResult AddToCart(int id)
        {
            var selectedBook = GetBookByID(id);
            if (selectedBook != null)
            {
                _cart.AddToCart(selectedBook, 1);
            }
            return RedirectToAction("Index");
        }

        public Book GetBookByID(int id)
        {
            return _context.Books.FirstOrDefault(b => b.Id == id);
        }

        public IActionResult RemoveFromCart(int id)
        {
            var selectedBook = GetBookByID(id);

            if (selectedBook != null) 
            {
                _cart.RemoveFromCart(selectedBook);
            }
            return RedirectToAction("Index");
        }

        public IActionResult ReduceQuantity(int id)
        {
            var selectedBook = GetBookByID(id);

            if (selectedBook != null)
            {
                _cart.ReduceQuantity(selectedBook);
            }
            return RedirectToAction("Index");
        }

        public IActionResult IncreaseQuantity(int id)
        {
            var selectedBook = GetBookByID(id);

            if (selectedBook != null)
            {
                _cart.AddToCart(selectedBook,1);
            }
            return RedirectToAction("Index");
        }
    }
}
