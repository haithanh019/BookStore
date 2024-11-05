using BookStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStore.Components
{
    public class GenreViewComponent : ViewComponent
    {
        private readonly BookStoreContext _context;

        public GenreViewComponent(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _context.Books
                .Select(b => b.Genre)
                .Distinct()
                .ToListAsync();

            return View(genres);
        }
    }
}
