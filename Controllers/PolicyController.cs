using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
	public class PolicyController : BaseController
	{
		/// <summary>
		/// 이용약관
		/// </summary>
		/// <returns></returns>
		[Route("TermsOfUse")]
		public IActionResult TermsOfUse()
		{
			return View();
		}

		/// <summary>
		/// 개인정보 처리방침
		/// </summary>
		/// <returns></returns>
		[Route("PrivacyPolicy")]
		public IActionResult PrivacyPolicy()
		{
			return View();
		}
	}
}