using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.Controllers
{
    public class ShoppingController : Controller
    {
        ApplicationDbContext db;

        public ShoppingController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Create", new { userId = user.Id });
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                return RedirectToAction("Create", new { userId = user.Id });
            }
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }

        public IActionResult Edit(int id)
        {
            var item = db.Items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Завантажуємо існуючий запис з БД
                    var existingItem = db.Items.FirstOrDefault(i => i.Id == id);
                    if (existingItem == null)
                    {
                        return NotFound();
                    }

                    // Оновлюємо потрібні поля
                    existingItem.ProductName = item.ProductName;
                    existingItem.ProductDescription = item.ProductDescription;
                    existingItem.ProductSorted = item.ProductSorted;
                    existingItem.Price = item.Price;
                    existingItem.UserId = item.UserId;

                    db.SaveChanges();

                    return RedirectToAction("ShoppingList", new { userId = item.UserId });
                }
                catch (Exception)
                {
                    return StatusCode(500, "Помилка при збереженні змін");
                }
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var item = db.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("ShoppingList", new { userId = item.UserId });
        }

        public IActionResult Create(int userId)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.User = user;
            return View();
        }

        [HttpPost]
        public IActionResult Create(int userId, Item item)
        {
            // Перевіряємо, чи існує користувач
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Прив’язуємо Item до користувача через зовнішній ключ
            item.UserId = user.Id;

            db.Items.Add(item);
            db.SaveChanges();

            return RedirectToAction("ShoppingList", new { userId = user.Id });
        }

        public async Task<IActionResult> ShoppingList(SortState sortOrder = SortState.ProductNameAsc)
        {
            IQueryable<Item>? items = db.Items;
            ViewData["ProductNameSort"] = sortOrder == SortState.ProductNameAsc ? SortState.ProductNameDesc : SortState.ProductNameAsc;
            ViewData["ProductDescriptionSort"] = sortOrder == SortState.ProductDescriptionAsc ? SortState.ProductDescriptionDesc : SortState.ProductDescriptionAsc;
            ViewData["ProductSortedSort"] = sortOrder == SortState.ProductSortedAsc ? SortState.ProductSortedDesc : SortState.ProductSortedAsc;
            ViewData["PriceSort"] = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            items = sortOrder switch
            {
                SortState.ProductNameDesc => items.OrderByDescending(s => s.ProductName),
                SortState.ProductDescriptionAsc => items.OrderBy(s => s.ProductDescription),
                SortState.ProductDescriptionDesc => items.OrderByDescending(s => s.ProductDescription),
                SortState.ProductSortedAsc => items.OrderBy(s => s.ProductSorted),
                SortState.ProductSortedDesc => items.OrderByDescending(s => s.ProductSorted),
                SortState.PriceAsc => items.OrderBy(s => s.Price),
                SortState.PriceDesc => items.OrderByDescending(s => s.Price),
                _ => items.OrderBy(s => s.ProductName),
            };
            return View(await items.AsNoTracking().ToListAsync());
        }
    }
}
