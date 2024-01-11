namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    using BRILLIANT_NFT_MARKET_FRONT.Models;
	using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.Extensions.Configuration;
	using System;

	public abstract class BaseController : Controller
    {
		protected readonly static IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
		protected readonly static bool _isDebug = _config.GetValue<bool>("IsDebug");
		protected readonly static string _domain = _config.GetValue<string>("Domain")!;
        protected readonly static string _loginCookieKey = _config.GetValue<string>("CookieKey:Login")!;
		protected readonly static string _serviceName = _config.GetValue<string>("ServiceName")!;
        protected readonly static string _berrypieGuideFileUrl = _config.GetValue<string>("BerrypieGuideFileUrl")!;
        protected readonly int _pageSize = _config.GetValue<int>("PageSize");
        protected readonly int _authEndMinutes = _config.GetValue<int>("AuthEndMinutes");

        protected readonly string _defaultErrorMessage = "System Error(Code: {0})";
        protected readonly ExceptionLog_Dac _exceptionLogDac = new();
        protected bool _isLocal = false;
        protected bool _isLogin = false;
        protected int _memberIdx = 0;
        protected string _memberId = string.Empty;

		public override void OnActionExecuting(ActionExecutingContext context)
        {
			base.OnActionExecuting(context);

			_isLocal = GetDomain().IndexOf("localhost") != -1;
			SettingLogin();
        }

		/// <summary>
		/// 로그인 정보 셋팅
		/// </summary>
		/// <param name="key"></param>
		public void SettingLogin()
		{
            _memberIdx = 0;
            _memberId = string.Empty;

			string loginCookie = GetCookie(_loginCookieKey);
            _isLogin = loginCookie != null;

            if (_isLogin)
			{
				string decLoginCookie = Encrypter.Decrypt(loginCookie!);
				string[] arrayLoginInfo = decLoginCookie.Split('|');

                _memberIdx = Convert.ToInt32(arrayLoginInfo[0]);
                _memberId = arrayLoginInfo[1];
			}
		}

        /// <summary>
        ///	도메인 조회
        /// </summary>
        /// <returns></returns>
        public string GetDomain()
        {
            return Request.Host.Value.ToLower().Replace("www.", "");
        }

        /// <summary>
        /// 쿠키값조회
        /// </summary>
        /// <param name="key"></param>
        public string GetCookie(string key)
		{
			return Request.Cookies[key]!;
		}

		/// <summary>
		/// 쿠키생성
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetCookie(string key, string value)
		{
			Response.Cookies.Append(key, value);
		}

		/// <summary>
		/// 쿠키삭제
		/// </summary>
		/// <param name="key"></param>
		public void DeleteCookie(string key)
		{
			if (GetCookie(key) != null)
			{
				Response.Cookies.Delete(key);
			}
		}

		/// <summary>
		///	접속 아이디 체크
		/// </summary>
		/// <returns></returns> 
		public bool CheckId()
		{
            Member_Dto adminDto = new()
			{
				MEMBER_IDX = _memberIdx
			};

			bool status = new Member_Dac().Select_MemberCheck(adminDto);
			if (!status)
			{
				DeleteCookie(_loginCookieKey);
			}

			return status;
		}

		/// <summary>
		///	도메인 체크
		/// </summary>
		/// <returns></returns> 
		public bool CheckDomain()
		{
			bool status = _domain.Equals(GetDomain());
			if (!status)
			{
				DeleteCookie(_loginCookieKey);
			}

			return status;
		}

        /// <summary>
        ///	모바일 접속 여부
        /// </summary>
        /// <returns></returns>
        public bool GetIsMobile()
        {
            return Request.Headers["User-Agent"].ToString().ToLower().IndexOf("mobile") > -1;
        }
    }
}