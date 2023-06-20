using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private MyContext db;

    public UserController(ILogger<UserController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View("Index");
    }

    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }

        PasswordHasher<User> hashPw = new PasswordHasher<User>();
        user.Password = hashPw.HashPassword(user, user.Password);

        db.Users.Add(user);
        db.SaveChanges();

        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("FirstName", user.FirstName);
        return RedirectToAction("Index", "Wedding");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser user)
    {
        if(!ModelState.IsValid)
        {
            return Index();
        }

        User? dbUser = db.Users.FirstOrDefault(u=> u.Email == user.LoginEmail);

        if (dbUser == null)
        {
            ModelState.AddModelError("LoginEmail", "not found");
            return Index();
        }

        PasswordHasher<LoginUser> hashPw = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashPw.VerifyHashedPassword(user, dbUser.Password, user.LoginPassword);

        if (pwCompareResult == 0)
        {
            ModelState.AddModelError("LoginPassword", "invalid password");
            return Index();
        }

        HttpContext.Session.SetInt32("UserId", dbUser.UserId);
        HttpContext.Session.SetString("FirstName", dbUser.FirstName);
        return RedirectToAction("Index", "Wedding");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
