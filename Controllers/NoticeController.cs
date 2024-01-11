using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class NoticeController : BaseController
    {
        /// <summary>
        /// 공지 사항
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

		/// <summary>
		///	공지사항 리스트 정보
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("ListData")]
		public JsonResult ListData()
		{
			int page = Convert.ToInt32(Request.Form["page"]);

			Notice_Dto noticeDto = new()
			{
				PAGE = page,
				PAGE_SIZE = _pageSize
			};

			return Json(new Notice_Dac().Select_NoticeList(noticeDto));
		}

		/// <summary>
		///	공지사항 상세 정보
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("notice/info/{noticeIdx}")]
		public IActionResult Info(int noticeIdx)
		{
			Notice_Dto noticeDto = new()
			{
				NOTICE_IDX = noticeIdx
			};

			return View(new Notice_Dac().Select_NoticeInfo(noticeDto));
		}
	}
}