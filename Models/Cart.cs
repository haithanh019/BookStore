using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookStore.Models
{
    public class Cart
    {
        private readonly BookStoreContext _context;

        public Cart(BookStoreContext context)
        {
            _context = context;
        }

        public string Id { get; set; }

        public List<CartItem> CartItems { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

            var context = services.GetService<BookStoreContext>();
            string cartId = session.GetString("Id") ?? Guid.NewGuid().ToString();

            session.SetString("Id", cartId);

            return new Cart(context)
            {
                Id = cartId
            };
        }
        public CartItem GetCartItem(Book book)
        {
            return _context.CartItems.SingleOrDefault(c => c.Book.Id == book.Id && c.CartId == Id);
        }
        public void AddToCart(Book book, int quantity)
        {
            var cartItem = GetCartItem(book);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Book = book,
                    Quantity = quantity,
                    CartId = Id
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }
            _context.SaveChanges();
        }
        public List<CartItem> GetAllCartItems()
        {
            return CartItems ?? (CartItems = _context.CartItems.Where(c => c.CartId == Id).Include(c => c.Book).ToList());
        }

        public int ReduceQuantity(Book book)
        {
            var cartItem = GetCartItem(book);

            var remainingQuantity = 0;

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    remainingQuantity = --cartItem.Quantity;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }
            }
            _context.SaveChanges();
            return remainingQuantity;
        }

        public int IncreaseQuantity(Book book)
        {
            var cartItem = GetCartItem(book);


            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            _context.SaveChanges();
            return cartItem?.Quantity ?? 0;
        }

        public void RemoveFromCart(Book book)
        {
            var cartItem = GetCartItem(book);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
            }
            _context.SaveChanges();
        }

        public void ClearCart()
        {
            var cartItem = _context.CartItems.Where(c => c.CartId == Id);
            _context.CartItems.RemoveRange(cartItem);
            _context.SaveChanges();
        }

        public float GetCartTotal()
        {
            return _context.CartItems.Where(c => c.CartId == Id).Select(c => c.Book.Price * c.Quantity).Sum();
        }
    }
}
