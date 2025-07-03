using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;

[Authorize]
public class UserController : Controller
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = int.Parse(User.FindFirst("UserId")!.Value);

        var users = await _context.Users
            .OrderByDescending(u => u.LastLoginTime)
            .ToListAsync();

        ViewBag.CurrentUserId = currentUserId;
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> BulkAction(string actionType, List<int> selectedIds)
    {
        var currentUserId = int.Parse(User.FindFirst("UserId")!.Value);
        var users = await _context.Users.Where(u => selectedIds.Contains(u.Id)).ToListAsync();

        switch (actionType)
        {
            case "block":
                users.ForEach(u => u.Status = "Blocked");
                break;
            case "unblock":
                users.ForEach(u => u.Status = "Active");
                break;
            case "delete":
                _context.Users.RemoveRange(users);
                break;
        }

        await _context.SaveChangesAsync();

        if (selectedIds.Contains(currentUserId) && actionType != "unblock")
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        return RedirectToAction("Index");
    }
}