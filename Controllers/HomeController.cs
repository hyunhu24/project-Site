using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
	public class HomeController : BaseController
	{
		public IActionResult Index()
		{
			Default_Dto defaultDto = new()
			{
				IS_LOGIN = _isLogin,
				PATH = string.Empty
			};

			List<Product_Dto> productList = new Product_Dac().Select_ProductList();

			ViewData["productList"] = productList;

			return View(defaultDto);
		}

		[Route("DownloadBerryPieGuide")]
		public IActionResult DownloadBerryPieGuide()
		{
			string fileName = Path.GetFileName(_berrypieGuideFileUrl);
			string contentType = "application/pdf";

			bool status = System.IO.File.Exists(_berrypieGuideFileUrl);
			byte[] fileBytes = status ? System.IO.File.ReadAllBytes(_berrypieGuideFileUrl) : null!;

			return status ? File(fileBytes, contentType, fileName) : NotFound();
		}
	}
}