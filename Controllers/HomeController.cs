using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WeddingPlanner.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private MyContext db;

  public HomeController(ILogger<HomeController> logger, MyContext context)
  {
    _logger = logger;
    db = context;
  }

  [HttpGet("")]
  public IActionResult Index()
  {
    return View();
  }

  [HttpPost("users/create")]
  public IActionResult CreateUser(User newUser)
  {
    if (ModelState.IsValid)
    {
      PasswordHasher<User> Hasher = new PasswordHasher<User>();
      newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
      db.Add(newUser);
      db.SaveChanges();
      HttpContext.Session.SetInt32("UserId", newUser.UserId);
      HttpContext.Session.SetString("FirstName", newUser.FirstName);
      return RedirectToAction("Weddings", "Wedding");
    }
    else
    {
      return View("Index");
    }
  }

  [HttpPost("/users/login")]
  public IActionResult Login(LoginUser userSubmission)
  {
    if (ModelState.IsValid)
    {
      User? userInDb = db.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
      if (userInDb == null)
      {
        ModelState.AddModelError("Email", "Invalid Email/Password");
        return View("Index");
      }
      PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
      var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
      if (result == PasswordVerificationResult.Success)
      {
        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
        HttpContext.Session.SetString("FirstName", userInDb.FirstName);
        return RedirectToAction("Weddings", "Wedding");
      }
      ModelState.AddModelError("Password", "Invalid Email/Password");
    }
    return View("Index");
  }

  [HttpGet("/users/logout")]
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
// define session check in controller you're using it in.
public class SessionCheckAttribute : ActionFilterAttribute
{
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    int? userId = context.HttpContext.Session.GetInt32("UserId");
    if (userId == null)
    {
      context.Result = new RedirectToActionResult("Index", "Home", null);
    }
  }
}