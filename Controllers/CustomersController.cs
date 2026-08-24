using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeLoyaltyApp.Data;
using CoffeeLoyaltyApp.Models;

namespace CoffeeLoyaltyApp.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // عرض قائمة العملاء مع رقم الجوال وخيارات التحكم، ودعم البحث
        public async Task<IActionResult> Index(string searchQuery)
        {
            var customers = from c in _context.Customers select c;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                customers = customers.Where(c => c.PhoneNumber.Contains(searchQuery) || c.Id.ToString() == searchQuery || c.Name.Contains(searchQuery));
            }

            return View(await customers.ToListAsync());
        }

        // صفحة إضافة عميل جديد (فتح الصفحة)
        public IActionResult Create()
        {
            return View();
        }

        // استقبال بيانات العميل الجديد مع فحص الأدمن وشرط رقم الجوال ومطابقة الاسم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // 1. شرط الأدمن السري (ينقلك للوحة التحكم مباشرة)
                if (!string.IsNullOrEmpty(customer.Name) && customer.Name.Trim().ToLower() == "sadam" && customer.PhoneNumber == "Ss123123Ss")
                {
                    return RedirectToAction(nameof(Index));
                }

                // 2. شرط رقم الجوال للعملاء: يجب أن يبدأ بـ 05 ويتكون من 10 أرقام
                if (string.IsNullOrEmpty(customer.PhoneNumber) || !customer.PhoneNumber.StartsWith("05") || customer.PhoneNumber.Length != 10)
                {
                    ModelState.AddModelError("PhoneNumber", "رقم الجوال يجب أن يبدأ بـ 05 ويتكون من 10 أرقام!");
                    return View(customer);
                }

                // 3. نتأكد هل رقم الجوال مسجل مسبقاً في قاعدة البيانات؟
                var existingCustomer = _context.Customers.FirstOrDefault(c => c.PhoneNumber == customer.PhoneNumber);

                if (existingCustomer != null)
                {
                    // إذا الرقم موجود بس الاسم المكتوب غير مطابق للاسم المسجل أول مرة، يرفض الدخول لحماية البطاقة!
                    if (existingCustomer.Name.Trim().ToLower() != customer.Name.Trim().ToLower())
                    {
                        ModelState.AddModelError("Name", "رقم الجوال هذا مسجل مسبقاً باسم آخر! تأكد من كتابة الاسم الصحيح.");
                        return View(customer);
                    }

                    // إذا كان الاسم ورقم الجوال متطابقين، يفتح بطاقته القديمة
                    return RedirectToAction(nameof(Details), new { id = existingCustomer.Id });
                }

                // 4. إذا مو مسجل، ضيفه كعميل جديد
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = customer.Id });
            }
            return View(customer);
        }

        // عرض بطاقة الولاء للعميل
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // دالة تحديث الأختام (زيادة أو نقصان من أزرار الجدول)
        [HttpPost]
        public async Task<IActionResult> UpdateStamps(int id, int change)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.StampCount += change;
                if (customer.StampCount < 0) customer.StampCount = 0;

                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // دالة تحديث الأكواب المجانية (زيادة أو نقصان من أزرار الجدول)
        [HttpPost]
        public async Task<IActionResult> UpdateFreeCups(int id, int change)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.FreeCupsCount += change;
                if (customer.FreeCupsCount < 0) customer.FreeCupsCount = 0;

                _context.Update(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // حذف العميل مباشرة من الجدول دون الحاجة لصفحة تأكيد
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}