let _pathType;
let _isAuthSend = false;
let _isAuthCheck = false;
let _authCodeCheckTimer;

function GetPath(actionType, isReturn) {
	let path;

	if (actionType === "login") {
		path = isReturn ? window.location.href : "/loginprocess";
	}
	else if (actionType === "logout") {
		path = isReturn ? "/" : "/logoutprocess";
	}
	else if (actionType === "payment") {
		path = isReturn ? "/" : "/Order/Payment";
	}
	else if (actionType === "sendAuthCode") {
		path = isReturn ? "" : "/SendAuthCodeProcess";
	}
	else if (actionType === "checkAuthCode") {
		path = isReturn ? "" : "/CheckAuthCodeProcess";
	}
	else if (actionType === "signup") {
		path = isReturn ? "/signupcomplete/" : "/SignupProcess";
	}
	else if (actionType === "editPassword") {
		path = isReturn ? "/editpasswordcomplete" : "/EditPaawordProcess";
	}
	else if (actionType === "checkCurrentPassword") {
		path = isReturn ? "" : "/CheckCurrentPassword";
	}
	else if (actionType === "serviceWithdrawal") {
		path = isReturn ? "/my/servicewithdrawalcomplete" : "/ServiceWithdrawalProcess";
	}
	else if (actionType === "editMemberInfo") {
		path = isReturn ? "" : "/EditMemberProcess";
	}
	else if (actionType === "checkAccount") {
		path = isReturn ? "" : "/Order/Checked";
	}

	return path;
}

function GetParams(actionType) {
	let params = $("form").serialize();

	if (actionType === "payment") {
		params += "&paymentType=" + encodeURIComponent("0002");
	}

	return params;
} 

let _isCallAjaxFlag = false;
function CallAjax(actionType, async, isLoading, dataType, isCallbackFuntion, params) {
	isCallbackFuntion = isCallbackFuntion === undefined ? false : isCallbackFuntion;

	if (_isCallAjaxFlag && !isCallbackFuntion) return false;
	_isCallAjaxFlag = true;


	dataType = dataType === undefined ? "json" : dataType;
	isLoading = isLoading === undefined ? true : isLoading;
	async = async === undefined ? true : async;

	let url = GetPath(actionType, false);

	$.ajax({
		type: "post",
		url,
		dataType,
		data: GetParams(actionType),
		async,
		cache: false,
		processData : true,
		contentType : "application/x-www-form-urlencoded; charset=utf-8",
		beforeSend: function () {
			if (isLoading) {
				Loading();
			}
		},
		success: function (response) {
			CallbackAjax(actionType, response, params);
		},
		error: function (error) {
			MovePath("/Error", "2");
		},
		complete: function () {
			_isCallAjaxFlag = false;
			if (isLoading) {
				if (actionType !== "checkAccount") {
					setTimeout(function () {
						Loading(true);
					}, 300);
				}
			}
		}
	});
}

function CallbackAjax(actionType, response, params) {
	const status = response.status;
	const result = response.result;
	const movePage = response.movePage;
	const arrayData = result.indexOf("|") ? result.split("|") : "";

	if (!status && movePage) {
		MovePath(movePage, "2");
		return false;
	}

	const path = GetPath(actionType, true);

	if (status) {
		if (actionType === "payment") {
			const url = movePage;
			
			//$("#").val();
			window.location.href = url;
			//var width = 600;
			//var height = 800;
			//var screenWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
			//var screenHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
			//var left = (screenWidth - width) / 2;
			//var top = (screenHeight - height) / 2;

			//window.open(url, 'PayPalPopup', 'width=' + width + ',height=' + height + ',left=' + left + ',top=' + top);
		}
		else if (actionType === "sendAuthCode") {
			alert("인증코드가 전송되었습니다.");

			$("#authCode").focus();
			_isAuthCheck = false;
			_isAuthSend = true;
			$("#authSendKey").val(result);
			$("#authCheckKey").val("");
			$("#authCode").attr("readonly", false);
			$("#btnCheckAuthCode").attr("disabled", false);

			let authResendSeconds = 60;
			const btnSendAuthCode = $("#btnSendAuthCode");
			btnSendAuthCode.attr("disabled", true);
			btnSendAuthCode.text("Re-send (" + authResendSeconds + "s)");

			let resendTimer = setInterval(function () {
				authResendSeconds--;

				if (authResendSeconds === 0) {
					clearInterval(resendTimer);
					btnSendAuthCode.attr("disabled", false);
					btnSendAuthCode.text("인증 코드 전송");
				}
				else {
					btnSendAuthCode.text("Re-send (" + authResendSeconds + "s)");
				}
			}, 1000);

			let areaAuthCodeCheck = $("#areaAuthCodeCheck");
			areaAuthCodeCheck.removeClass("hide");
			let authTimer = areaAuthCodeCheck.children("span").eq(1);

			let isAuthCodeCheckExp = false;
			let authCodeCheckMinutes = parseInt(_info.authEndMinutes - 1);
			let authCodeCheckSeconds = 60;
			authTimer.text((authCodeCheckMinutes + 1) + ":00");
			_authCodeCheckTimer = setInterval(function () {
				authCodeCheckSeconds--;

				if (authCodeCheckSeconds === 0) {
					if (authCodeCheckMinutes > 0) {
						authCodeCheckMinutes--;
						authCodeCheckSeconds = 59;
					}

					isAuthCodeCheckExp = authCodeCheckMinutes === 0 && authCodeCheckSeconds === 0;
					if (isAuthCodeCheckExp) {
						clearInterval(_authCodeCheckTimer);
						_isAuthSend = false;
						$("#authSendKey, #authCheckKey, #authCode").val("");
						$("#authCode").attr("readonly", true);
						$("#btnCheckAuthCode").attr("disabled", true);
					}
				}

				let viewTime = authCodeCheckMinutes + ":" + (authCodeCheckSeconds.toString().length == 1 ? "0" + authCodeCheckSeconds.toString() : authCodeCheckSeconds);
				authTimer.text(isAuthCodeCheckExp ? "" : viewTime);

				if (isAuthCodeCheckExp) {
					authTimer.text("");
					areaAuthCodeCheck.addClass("hide");
					alert("인증시간이 초과하였습니다. 인증코드를 재전송 해주세요.");
				}
			}, 1000);
		}
		else if (actionType === "checkAuthCode") {
			_isAuthCheck = true;
			$("#authCheckKey").val(result);

			alert("인증되었습니다.");
			if (_authCodeCheckTimer) {
				clearInterval(_authCodeCheckTimer);
			}

			let areaAuthCodeCheck = $("#areaAuthCodeCheck");
			let authTimer = areaAuthCodeCheck.children("span").eq(1);
			authTimer.text("");
			areaAuthCodeCheck.addClass("hide");

			$("#authCode").attr("readonly", true);
			$("#btnCheckAuthCode").attr("disabled", true);
			$("#memberPassword").focus();
		}
		else if (actionType === "sendCoin") {
			MovePath(path + "?result=" + result, "2");
		}
		else if (actionType === "editMemberGameCount") {
			MovePath(result);
		}
		else if (actionType === "signup") {
			MovePath(path + result, "2");
		}
		else if (actionType === "editPassword") {
			MovePath(path, "2");
		}
		else if (actionType === "checkCurrentPassword") {
			if (confirm("탈퇴 하시겠습니까?")) {
				CallAjax("serviceWithdrawal", true, true, "json", true);
			}
		}
		else if (actionType === "editMemberInfo") {
			alert("변경되었습니다.");
			let editType = arrayData[0];
			$("#area" + editType).text(arrayData[1]);
			$("#btn_areaView" + editType).trigger("click");
		}
		else if (actionType === "checkAccount") {
			let productIdx = $("#productIdx").val();
			//let quantity = $("#quantity").val();
			let shippingAddress = encodeURIComponent($("#shippingAddress").val());
			let depositName = encodeURIComponent($("#depositName").val());
			let bankName = encodeURIComponent(arrayData[0]);
			let accountOwner = encodeURIComponent(arrayData[1]);
			let accountNumber = arrayData[2];

			MovePath(`/Order/${productIdx}?paymentType=0001&shippingAddress=${shippingAddress}&depositName=${depositName}&bankName=${bankName}&accountOwner=${accountOwner}&accountNumber=${accountNumber}`);
		}
		else {
			if (result.length > 0) {
				alert(result);
			}
			if (path.length > 0) {
				MovePath(path, "2");
			}
		}
	}
	else {
		if (actionType === "checkAccount") {
			alert(arrayData[0]);
			if (arrayData[1]) {
				$("#areaBankName").text("은행 : " + arrayData[1]);
				$("#areaAccountOwner").text("예금주 : " + arrayData[2]);
				$("#areaAccountNumber").text("계좌번호 : " + arrayData[3]);
			}
			Loading(true);
		}
		else {
			if (movePage) {
				MovePath(movePage, "2");
			}
			else {
				if (result) {
					alert(result);
				}
			}
		}
	}
}