using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;
    private MyContext db;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpGet("weddings")]
    public IActionResult Index()
    {
        List<Wedding> allWeddings = db.Weddings.Include(wedding => wedding.AllAssociations).ToList();
        return View("AllWeddings", allWeddings);
    }

    [HttpGet("weddings/new")]
    public IActionResult NewWedding()
    {
        return View("NewWedding");
    }

    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if (!ModelState.IsValid)
        {
            return View("NewWedding");
        }

        newWedding.UserId = (int)HttpContext.Session.GetInt32("UserId");

        db.Weddings.Add(newWedding);

        db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet("weddings/{id}")]
    public IActionResult ViewWedding(int id)
    {
        Wedding? wedding = db.Weddings.Include(wedding => wedding.AllAssociations).ThenInclude(association => association.User)
        .FirstOrDefault(wedding => wedding.WeddingId == id);

        if (wedding == null)
        {
            return RedirectToAction("Index");
        }

        return View("ViewWedding", wedding);
    }

    [HttpPost("weddings/{id}")]
    public IActionResult UpdateGuests(int id)
    {
        Association newAssociation = new Association()
        {
            UserId = HttpContext.Session.GetInt32("UserId"),
            WeddingId = id
        };

        db.Associations.Add(newAssociation);

        db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost("weddings/{id}/unrsvp")]
    public IActionResult UnRSVP(int id)
    {
        Association? newAssociation = db.Associations.FirstOrDefault(association => association.UserId == HttpContext.Session.GetInt32("UserId") && association.WeddingId == id);

        if (newAssociation != null)
        {
            db.Associations.Remove(newAssociation);
            db.SaveChanges();
        }
        
        return RedirectToAction("Index");
    }

    // [SessionCheck]
    [HttpPost("weddings/{id}/delete")]
    public IActionResult Delete(int id)
    {
        Wedding wedding = db.Weddings.FirstOrDefault(wedding => wedding.WeddingId == id);

        if (wedding != null && wedding.UserId == HttpContext.Session.GetInt32("UserId"))
        {
            db.Weddings.Remove(wedding);

            db.SaveChanges();
        }

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

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "User", null);
        }
    }
}
