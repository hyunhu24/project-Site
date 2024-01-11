using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class FaqController : BaseController
    {
        /// <summary>
        /// FAQ
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
			return View(new Faq_Dac().Select_FaqList());
        }
	}
}