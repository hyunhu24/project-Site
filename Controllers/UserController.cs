using BRILLIANT_NFT_MARKET_FRONT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class UserController : BaseController
    {
        /// <summary>
        /// 회원 가입
        /// </summary>
        /// <returns></returns>
        [Route("signup")]
        public IActionResult Signup()
        {
            if (_isLogin) return Redirect("/");

            return View();
        }

		/// <summary>
		///	인증코드 전송
		/// </summary>
		/// <returns></returns>
		[Route("SendAuthCodeProcess")]
		[HttpPost]
		public JsonResult SendAuthCodeProcess()
		{
			if (!CheckDomain()) throw new Exception("domain error");

			string result = string.Empty;			
            string purposeCode = Request.Form["purposeCode"]!;
            bool status = purposeCode.Equals("0001") || purposeCode.Equals("0002") && !_isLogin;
            string movePage = status ? string.Empty : "/Error";

			if (status)
			{
				bool isSignup = purposeCode.Equals("0001");
				string memberEmail = Request.Form["memberEmail"]!;

				Dictionary<string, string> paramList = new()
				{
					{ "memberEmail", memberEmail }
				};

				foreach (KeyValuePair<string, string> item in paramList)
				{
					status = Function.CheckValidation(item.Key, item.Value);

					if (!status)
					{
						result = Function.GetMessage(item.Key, "0002");
						break;
					}
				}

				if (status)
				{
					Member_Dac memberDac = new();
					Member_Dto memberDto = new()
					{
						MEMBER_ID = memberEmail
                    };

					// 아이디 존재여부 체크
					status = memberDac.Select_MemberIdDuplicateCheck(memberDto);

					// 비밀번호 재설정의 경우는 아이디가 존재해야 한다.
					if (!isSignup)
					{
						status = !status;
					}

					if (!status)
					{
						result = isSignup ? "이미 존재하는 아이디 입니다." : "가입된 아이디가 아닙니다.";
					}

					if (status)
					{
						string authSendKey = Function.GetGuId();
						string authCode = _isDebug ? "1111" : Function.GetRandomNumber(10000);
						AuthLog_Dto authLogDto = new()
						{
							EMAIL_ADDRESS = memberEmail,
							MEMBER_IDX = _isLogin ? _memberIdx : 0,
							PURPOSE_CODE = purposeCode,
							TITLE = "[" + _serviceName + "] " + (isSignup ? "회원가입" : "비밀번호 찾기") + " 인증코드 전송",
							CONTENTS = "인증 코드는 " + authCode + "입니다.",
							AUTH_CODE = authCode
						};

						if (!_isDebug)
						{
							status = new SendProcess().SendEmail(authLogDto);
						}

						if (status)
						{
                            authLogDto.EXP_DATE = DateTime.Now.AddMinutes(_authEndMinutes).ToString("yyyy-MM-dd HH:mm:ss.fff");
                            authLogDto.AUTH_SEND_KEY = authSendKey;

							status = new AuthLog_Dac().Insert_AuthLog(authLogDto);
							result = status ? authSendKey : string.Format(_defaultErrorMessage, "0001");
						}
						else
						{
							result = "인증코드 전송에 실패하였습니다.";
						}
					}
				}
			}

			return Json(new { status, result, movePage });
		}

        /// <summary>
        ///	인증 코드 확인
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CheckAuthCodeProcess")]
        public JsonResult CheckAuthCodeProcess()
        {
            if (!CheckDomain()) throw new Exception("domain error");

            string result = string.Empty;
            string purposeCode = Request.Form["purposeCode"]!;
            bool status = purposeCode.Equals("0001") || purposeCode.Equals("0002") && !_isLogin;
            string movePage = status ? string.Empty : "/Error";

            if (status)
            {
				bool isSignup = purposeCode.Equals("0001");
				string memberEmail = Request.Form["memberEmail"]!;

                Dictionary<string, string> paramList = new()
                {
                    { "memberEmail", memberEmail }
                };

                foreach (KeyValuePair<string, string> item in paramList)
                {
                    status = Function.CheckValidation(item.Key, item.Value);

                    if (!status)
                    {
                        result = Function.GetMessage(item.Key, "0002");
                        break;
                    }
                }

                if (status)
                {
                    Member_Dac memberDac = new();
                    Member_Dto memberDto = new()
                    {
                        MEMBER_ID = memberEmail
                    };

                    // 아이디 존재여부 체크
                    status = memberDac.Select_MemberIdDuplicateCheck(memberDto);

                    // 비밀번호 재설정의 경우는 아이디가 존재해야 한다.
                    if (!isSignup)
                    {
                        status = !status;
                    }

                    if (!status)
                    {
                        result = isSignup ? "이미 존재하는 아이디 입니다." : "가입된 아이디가 아닙니다.";
                    }

                    string authSendKey = Request.Form["authSendKey"]!;
                    string authCode = Request.Form["authCode"]!;

                    paramList = new() {
                        { "authSendKey", authSendKey },
                        { "authCode", authCode },
                    };

                    foreach (KeyValuePair<string, string> item in paramList)
                    {
                        if (!Function.CheckValidation(item.Key, item.Value))
                        {
                            if (item.Key.Equals("authSendKey")) throw new Exception("authSendKey error");

                            status = false;
                            result = Function.GetMessage(item.Key, "0002");
                            break;
                        }
                    }

                    if (status)
                    {
                        AuthLog_Dac authLogDac = new();
                        AuthLog_Dto authLogDto = new()
                        {
                            EMAIL_ADDRESS = memberEmail,
                            MEMBER_IDX = _isLogin ? _memberIdx : 0,
                            PURPOSE_CODE = purposeCode,
                            AUTH_SEND_KEY = authSendKey,
                            AUTH_CODE = authCode
                        };

                        long logIdx = authLogDac.Select_AuthCheck(authLogDto);
                        status = logIdx > 0;

                        if (status)
                        {
                            string authCheckKey = Function.GetGuId();
                            authLogDto = new()
                            {
                                LOG_IDX = logIdx,
                                AUTH_COMPLETE_KEY = authCheckKey
                            };

                            status = authLogDac.Update_AuthLogComplete(authLogDto);

                            if (status)
                            {
                                result = authCheckKey;
                            }
                            else
                            {
                                result = string.Format(_defaultErrorMessage, "0001");
                            }
                        }
                        else
                        {
                            result = "인증코드를 확인해 주세요.";
                        }
                    }
                }
            }

            return Json(new { status, result, movePage });
        }

        /// <summary>
        ///	회원 가입 처리
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SignupProcess")]
        public JsonResult SignupProcess()
        {
            if (!CheckDomain()) throw new Exception("domain error");

            string result = string.Empty;
            string purposeCode = Request.Form["purposeCode"]!;
            bool status = purposeCode.Equals("0001") && !_isLogin;
            string movePage = status ? string.Empty : "/Error";

            if (status)
            {
                try
                {
                    string memberEmail = Request.Form["memberEmail"]!;
                    string authSendKey = Request.Form["authSendKey"]!;
                    string authCheckKey = Request.Form["authCheckKey"]!;
                    string authCode = Request.Form["authCode"]!;
                    string memberPassword = Request.Form["memberPassword"]!;
                    string memberName = Request.Form["memberName"]!;
                    string hpNumber = Request.Form["hpNumber"]!;
                    string shippingAddress = Request.Form["shippingAddress"]!;
                    string coinAddress = Request.Form["coinAddress"]!;

                    status = hpNumber.Length > 3;

                    if (status)
                    {
                        Dictionary<string, string> paramList = new()
                        {
                            { "memberEmail", memberEmail },
                            { "authSendKey", authSendKey },
                            { "authCheckKey", authCheckKey },
                            { "authCode", authCode },
                            { "memberPassword", memberPassword },
                            { "memberName", memberName },
                            { hpNumber[..3].Equals("010") ? "koreaHpNumber" : "notKoreaHpNumber", hpNumber },
						    { "shippingAddress", shippingAddress }
					    };
                        if (!string.IsNullOrEmpty(coinAddress))
                        {
                            paramList.Add("coinAddress", coinAddress);
                        }

                        foreach (KeyValuePair<string, string> item in paramList)
                        {
                            string key = item.Key;
                            status = Function.CheckValidation(key, item.Value);

                            if (!status)
                            {
                                if (key.Equals("authSendKey")) throw new Exception("authSendKey error");
                                else if (key.Equals("authCheckKey")) throw new Exception("authCheckKey error");

                                result = Function.GetMessage(key, "0002");
                                break;
                            }
                        }

                        if (status)
                        {
                            string memberKey = Function.GetGuId();

                            Member_Dac memberDac = new();
                            Member_Dto memberDto = new()
                            {
                                MEMBER_ID = memberEmail
                            };

                            // 아이디 존재여부 체크
                            status = memberDac.Select_MemberIdDuplicateCheck(memberDto);

                            if (status)
                            {
                                AuthLog_Dac authLogDac = new();
                                AuthLog_Dto authLogDto = new()
                                {
                                    PURPOSE_CODE = "0001",
                                    EMAIL_ADDRESS = memberEmail,
                                    AUTH_SEND_KEY = authSendKey,
                                    AUTH_COMPLETE_KEY = authCheckKey
                                };

                                // 인증 완료 여부 체크
                                long authCompleteIdx = authLogDac.Select_AuthCompleteIdx(authLogDto);

                                status = authCompleteIdx > 0;

                                if (status)
                                {
                                    // 인증정보 삭제
                                    authLogDto.LOG_IDX = authCompleteIdx;
								    authLogDac.Delete_AuthLog(authLogDto);

                                    // 회원정보 저장
                                    memberDto.MEMBER_KEY = memberKey;
                                    memberDto.MEMBER_PASSWORD = _isDebug ? memberPassword : Encrypter.Encrypt_MD5(memberPassword);
                                    memberDto.MEMBER_NAME = memberName;
                                    memberDto.HP_NUMBER = hpNumber;
                                    memberDto.SHIPPING_ADDRESS = Function.SetXSSStringConvert(shippingAddress);
                                    memberDto.COIN_ADDRESS = coinAddress;
                                    status = memberDac.Insert_MemberInfo(memberDto);

                                    if (status)
                                    {
                                        StringBuilder request = new(200);
                                        request.Append('{');
                                        request.AppendFormat("\"emailAddress\":\"{0}\"", memberEmail);
                                        request.AppendFormat(",\"memberPassword\":\"{0}\"", memberPassword);
                                        request.AppendFormat(",\"memberNickname\":\"{0}\"", memberKey);
                                        request.Append(",\"partnerCode\":\"0002\"");
                                        request.Append('}');

                                        new SendProcess().CallBerrypieApi("POST", "user/createmember", request.ToString());
                                        result = memberKey;
                                    }
                                    else
                                    {
                                        movePage = "/Error";
                                    }
                                }
                                else
                                {
                                    result = "이메일 인증정보를 확인해 주세요.";
                                }
                            }
                            else
                            {
                                result = "이미 존재하는 아이디 입니다.";
                            }
                        }
                    }
				}
				catch (Exception ex)
				{
                    _exceptionLogDac.Insert_ExceptionLog(ex);
					throw;
				}
			}

            return Json(new { status, result, movePage });
        }

        /// <summary>
        /// 회원 가입 완료
        /// </summary>
        /// <returns></returns>
        [Route("signupcomplete/{memberKey}")]
        public IActionResult SignupComplete(string memberKey)
        {
            if (_isLogin) return Redirect("/");

            Member_Dto memberDto = new()
            {
                MEMBER_KEY = memberKey
            };

            return View(new Member_Dac().Select_MemberInfo(memberDto));
        }

        /// <summary>
        ///	로그인 처리
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("loginprocess")]
        public JsonResult LoginProcess()
        {
			if (!CheckDomain()) throw new Exception("domain error");
			bool status = !_isLogin;
            string result = status ? string.Empty : "이미 로그인 상태 입니다.";
            string? movePage = string.Empty;

            if (status)
            {
                string loginId = Request.Form["loginId"]!;
                string loginPassword = Request.Form["loginPassword"]!;

                Dictionary<string, string> paramList = new()
                {
                    { "loginId", loginId },
                    { "loginPassword", loginPassword }
                };

                foreach (KeyValuePair<string, string> item in paramList)
                {
                    if (!Function.CheckValidation(item.Key, item.Value))
                    {
                        status = false;
                        result = Function.GetMessage(item.Key, "0002");
                        break;
                    }
                }

                if (status)
                {
                    Member_Dto memberDto = new()
                    {
                        MEMBER_ID = loginId,
                        MEMBER_PASSWORD = _isDebug ? loginPassword : Encrypter.Encrypt_MD5(loginPassword),
                    };
                    if (_isLocal) memberDto.MEMBER_PASSWORD = null;

					Member_Dto memberInfo = new Member_Dac().Select_MemberInfo(memberDto);
                    status = memberInfo != null;

					if (status)
                    {
                        string loginCookie = Encrypter.Encrypt(
                            memberInfo!.MEMBER_IDX + "|" +
                            memberInfo.MEMBER_ID
						);

						SetCookie(_loginCookieKey, loginCookie);
					}
                    else
                    {
						result = "로그인 정보를 확인해 주세요.";
                    }
                }
            }

            return Json(new { status, result, movePage });
        }

        /// <summary>
        /// 로그아웃 처리
        /// </summary>
        /// <returns></returns>
        [Route("logoutprocess")]
        public IActionResult LogoutProcess()
        {
			if (!CheckDomain()) throw new Exception("domain error");
			bool status = _isLogin;
            string result = status ? string.Empty : "이미 로그아웃 상태 입니다.";
            string movePage = string.Empty;

            if (status)
            {
                DeleteCookie(_loginCookieKey);
            }

            return Json(new { status, result, movePage });
        }

		/// <summary>
		/// 비밀번호 찾기
		/// </summary>
		/// <returns></returns>
		[Route("findpassword")]
		public IActionResult FindPassword()
		{
			if (_isLogin) return Redirect("/");

			return View();
		}

		/// <summary>
		/// 비밀번호 찾기
		/// </summary>
		/// <param name="memberEmail"></param>
		/// <param name="authSendKey"></param>
		/// <param name="authCheckKey"></param>
		/// <returns></returns>
		[Route("resetpassword/{memberEmail}/{authSendKey}/{authCheckKey}")]
		public IActionResult ResetPassword(string memberEmail, string authSendKey, string authCheckKey)
		{
			if (_isLogin) return Redirect("/");

            bool status = true;

			Dictionary<string, string> paramList = new()
			{
				{ "memberEmail", memberEmail },
				{ "authSendKey", authSendKey },
				{ "authCheckKey", authCheckKey }
			};

			foreach (KeyValuePair<string, string> item in paramList)
			{
				string key = item.Key;

				if (!Function.CheckValidation(key, item.Value))
				{
                    status = false;
					break;
				}
			}

            if (status)
            {
                AuthLog_Dac authLogDac = new();
                AuthLog_Dto authLogDto = new()
                {
                    PURPOSE_CODE = "0002",
                    EMAIL_ADDRESS = memberEmail,
                    AUTH_SEND_KEY = authSendKey,
                    AUTH_COMPLETE_KEY = authCheckKey
                };

				// 인증 완료 여부 체크
				status = authLogDac.Select_AuthCompleteIdx(authLogDto) > 0;
            }

            if (!status)
            {
                return Redirect("/Error");
            }

            ViewData["memberEmail"] = memberEmail;
			ViewData["authSendKey"] = authSendKey;
			ViewData["authCheckKey"] = authCheckKey;

			return View();
		}

		/// <summary>
		///	비밀번호 변경 처리
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("EditPaawordProcess")]
		public JsonResult EditPaawordProcess()
		{
			if (!CheckDomain()) throw new Exception("domain error");

			string result = string.Empty;
			bool status = !_isLogin;
			string movePage = status ? string.Empty : "/Error";

			if (status)
			{
				string memberEmail = Request.Form["memberEmail"]!;
				string authSendKey = Request.Form["authSendKey"]!;
				string authCheckKey = Request.Form["authCheckKey"]!;
				string newPassword = Request.Form["newPassword"]!;

				Dictionary<string, string> paramList = new()
				{
					{ "memberEmail", memberEmail },
					{ "authSendKey", authSendKey },
					{ "authCheckKey", authCheckKey },
					{ "newPassword", newPassword }
				};

				foreach (KeyValuePair<string, string> item in paramList)
				{
					string key = item.Key;
					status = Function.CheckValidation(key, item.Value);

					if (!status)
					{
						if (key.Equals("authSendKey")) throw new Exception("authSendKey error");
						else if (key.Equals("authCheckKey")) throw new Exception("authCheckKey error");

						result = Function.GetMessage(key, "0002");
						break;
					}
				}

				if (status)
				{
					Member_Dac memberDac = new();
					Member_Dto memberDto = new()
					{
						MEMBER_ID = memberEmail
					};

					// 아이디 존재여부 체크
					status = !memberDac.Select_MemberIdDuplicateCheck(memberDto);

					if (status)
					{
						AuthLog_Dac authLogDac = new();
						AuthLog_Dto authLogDto = new()
						{
							PURPOSE_CODE = "0002",
							EMAIL_ADDRESS = memberEmail,
							AUTH_SEND_KEY = authSendKey,
							AUTH_COMPLETE_KEY = authCheckKey
						};

						// 인증 완료 여부 체크
						long authCompleteIdx = authLogDac.Select_AuthCompleteIdx(authLogDto);

						status = authCompleteIdx > 0;

						if (status)
						{
							// 인증정보 삭제
							authLogDac.Delete_AuthLog(authLogDto);

							// 회원정보 저장
							memberDto.MEMBER_PASSWORD = _isDebug ? newPassword : Encrypter.Encrypt_MD5(newPassword);
							status = memberDac.Update_MemberInfo(memberDto);

							if (!status)
							{
								movePage = "/Error";
							}
						}
						else
						{
							result = "이메일 인증정보를 확인해 주세요.";
						}
					}
					else
					{
                        movePage = "/Error";
					}
				}
			}

			return Json(new { status, result, movePage });
		}

		/// <summary>
		/// 비밀번호 변경 완료
		/// </summary>
		/// <returns></returns>
		[Route("editpasswordcomplete")]
		public IActionResult EditPasswordComplete()
		{
			if (_isLogin) return Redirect("/");

			return View();
		}
	}
}