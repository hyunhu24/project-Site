using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
	public class SystemController : Controller
	{
		[Route("Error")]
		public IActionResult Error()
		{
			return View();
		}

		[Route("Check")]
		public IActionResult Check()
		{
			return View();
		}
	}
}