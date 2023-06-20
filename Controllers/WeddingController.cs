using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
{
    private readonly ILogger<WeddingController> _logger;
    private MyContext db;

    public WeddingController(ILogger<WeddingController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [SessionCheck]
    [HttpGet("weddings")]
    public IActionResult Index()
    {
        List<Wedding> allWeddings = db.Weddings.ToList();
        return View("AllWeddings", allWeddings);
    }

    [HttpPost("weddings/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }

        db.Weddings.Add(newWedding);

        db.SaveChanges();

        return RedirectToAction("Index");
    }

    // [HttpGet("weddings/{id}")]
    // public IActionResult ViewWedding(int id)
    // {
    //     Wedding? wedding = db.Weddings.Include(wedding => wedding.AllAssociations).ThenInclude(association => association.Category)
    //     .FirstOrDefault(wedding => wedding.WeddingId == id);

    //     ViewBag.missingcategories = db.Categories.Include(category => category.AllAssociations)
    //     .Where(category => category.AllAssociations.All(association => association.WeddingId != id));

    //     if (wedding == null)
    //     {
    //         return RedirectToAction("Index");
    //     }

    //     return View("ViewWedding", wedding);
    // }

    // [HttpPost("weddings/{id}")]
    // public IActionResult UpdateCategories(int id, int categoryId)
    // {
    //     Association newAssociation = new Association()
    //     {
    //         WeddingId = id,
    //         CategoryId = categoryId
    //     };

    //     db.Associations.Add(newAssociation);

    //     db.SaveChanges();

    //     // return RedirectToAction("ViewWedding", id);
    //     return ViewWedding(id);
    // }

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
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
