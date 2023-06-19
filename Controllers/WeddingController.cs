#pragma warning disable CS8618
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8629
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
  public IActionResult Weddings()
  {
    List<Wedding> allWeddings = db.Weddings
      .Include(w => w.AllRsvps)
      .ThenInclude(r => r.User)
      .Include(w => w.Creator)
      .ToList();
    return View("Weddings", allWeddings);
  }

  [HttpGet("weddings/{WeddingId}/rsvp")]
  public IActionResult Rsvp(int weddingId)
  {
    int? userId = HttpContext.Session.GetInt32("UserId");
    Wedding? wedding = db.Weddings
      .Include(w => w.AllRsvps)
      .FirstOrDefault(r => r.WeddingId == weddingId);
    User? user = db.Users.FirstOrDefault(w => w.UserId == userId);
    if (user == null || wedding == null)
    {
      return View("Weddings");
    }
    if (user.AllRsvps.Any(r => r.WeddingId == weddingId)) // check if they're already RSVP'd
    {
      return RedirectToAction("Weddings"); // handle RSVP removes here
    }
    RSVP newRsvp = new RSVP
    {
      User = user,
      Wedding = wedding
    };
    db.RSVPs.Add(newRsvp);
    db.SaveChanges();
    return RedirectToAction("Weddings");
  }

  [HttpGet("weddings/{WeddingId}/rsvp/remove")]
  public IActionResult RemoveRsvp(int weddingId)
  {
    int? userId = HttpContext.Session.GetInt32("UserId");
    Wedding? wedding = db.Weddings
        .Include(w => w.AllRsvps)
        .FirstOrDefault(w => w.WeddingId == weddingId);
    User? user = db.Users.FirstOrDefault(u => u.UserId == userId);
    RSVP? rsvpToRemove = wedding.AllRsvps.FirstOrDefault(r => r.UserId == userId);
    if (rsvpToRemove == null || user == null || wedding == null)
    {
      return RedirectToAction("Weddings");
    }
    db.RSVPs.Remove(rsvpToRemove);
    db.SaveChanges();
    return RedirectToAction("Weddings");
  }

  // redirect to New.cshtml
  [HttpGet("weddings/new")]
  public IActionResult NewWedding()
  {
    return View("NewWedding");
  }

  // Create new wedding
  [HttpPost("weddings/create")]
  public IActionResult CreateWedding(Wedding wedding)
  {
    if (!ModelState.IsValid)
    {
      return View("NewWedding");
    }
    int userId = (int)HttpContext.Session.GetInt32("UserId");
    wedding.UserId = userId;
    db.Weddings.Add(wedding);
    db.SaveChanges();
    return RedirectToAction("ShowWedding", wedding);
  }

  // Get one Wedding
  [HttpGet("weddings/{WeddingId}")]
  public IActionResult ShowWedding(int WeddingId)
  {
    Wedding? wedding = db.Weddings
        .Include(w => w.AllRsvps)
        .ThenInclude(r => r.User)
        .FirstOrDefault(d => d.WeddingId == WeddingId);

    if (wedding == null)
    {
      return RedirectToAction("Index");
    }
    return View("ViewWedding", wedding);
  }

  // Delete the Wedding
  [HttpPost("weddings/{WeddingId}/destroy")]
  public IActionResult DestroyWedding(int WeddingId)
  {
    int userId = (int)HttpContext.Session.GetInt32("UserId");
    Wedding wedding = db.Weddings.Include(w => w.Creator).FirstOrDefault(d => d.WeddingId == WeddingId);
    if (wedding == null)
    {
      return RedirectToAction("Weddings");
    }
    if (wedding.Creator.UserId != userId)
    {
      // User is not the creator of the wedding, so they are not authorized to delete it
      return RedirectToAction("Weddings");
    }
    db.Weddings.Remove(wedding);
    db.SaveChanges();
    return RedirectToAction("Weddings");
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
