using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRazorApp.Pages
{
    public class MenuModel : PageModel
    {
        public List<Dishes> Dishes { get; set; } = new();
        private readonly AppDbContext _context;

        public MenuModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Dishes = await _context.Dishes
                .Include(d => d.DishIngredients)
                    .ThenInclude(di => di.Ingredients)
                .ToListAsync();
        }

    }
}
