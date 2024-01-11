using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class ProductController : BaseController
	{
        [Route("Product/Info/{productIdx}")]
		public IActionResult Info(int productIdx)
		{
			Product_Dto productDto = new()
			{ 
				PRODUCT_IDX = productIdx,
			};

			ViewData["isLogin"] = _isLogin;

			return View(new Product_Dac().Select_ProductInfo(productDto));
		}

        [Route("Product/Buy/{productIdx}")]
        public IActionResult Buy(int productIdx, int quantity, bool isCancel, string? errorMessage = null)
        {
			if (!_isLogin || !CheckId()) return Redirect("/");

			quantity = 1;
			Product_Dto productDto = new()
			{
				PRODUCT_IDX = productIdx,
			};
			Member_Dto memberDto = new()
			{
				MEMBER_IDX = _memberIdx,
			};

			productDto = new Product_Dac().Select_ProductInfo(productDto);
			productDto.QUANTITY = quantity;

			memberDto = new Member_Dac().Select_MemberInfo(memberDto);

			ViewData["memberInfo"] = memberDto;
			ViewData["productInfo"] = productDto;
			ViewData["isCancel"] = isCancel;
			ViewData["errorMessage"] = errorMessage! == null ? "사용자 결제 취소" : errorMessage; // 실패 사유

			return View();
        }
    }
}