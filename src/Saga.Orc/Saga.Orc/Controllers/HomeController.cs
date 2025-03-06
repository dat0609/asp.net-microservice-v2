using Microsoft.AspNetCore.Mvc;

namespace Saga.Orc.Controllers;

public class HomeController : ControllerBase
{
    public IActionResult Index()
    {
        return Redirect("~swagger");
    }
}