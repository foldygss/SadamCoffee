using CoffeeLoyaltyApp.Models; // استدعاء مجلد النماذج عشان نقدر نستخدم كلاس Customer
using Microsoft.EntityFrameworkCore; // مكتبة استدعاء أدوات ربط قاعدة البيانات

namespace CoffeeLoyaltyApp.Data
{
    // هذا الكلاس هو "المدير" المسؤول عن التواصل مع قاعدة البيانات
    public class ApplicationDbContext : DbContext
    {
        // هذه الدالة (Constructor) تمرر إعدادات الاتصال للنظام لفتح قناة الاتصال
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // هذا السطر هو اللي يخبر قاعدة البيانات بإنشاء جدول حقيقي باسم Customers
        // اعتماداً على القالب (الكلاس) الذي صممناه سابقاً في ملف Customer
        public DbSet<Customer> Customers { get; set; }
    }
}