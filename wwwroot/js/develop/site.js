$(function () {

	$(document).on("keyup", "input[type=tel]", function () {
		$(this).val($(this).val().replace(/[^0-9]/g, ""));
	});

	$(".btnHome").click(function () {
		MovePath("/", "2");
	});

	$("#loginId, #loginPassword").keydown(function (key) {
		if (key.keyCode == 13) {
			Login();
		}
	});

	$(".btnLogin").click(function () {
		Login();
	});

	$(".btnLoginModal").click(function () {
		Modal("Login");
	});

	$(".btnLogout").click(function () {
		Logout();
	});

	$("#btnProductBuy").click(function () {
		let productIdx = parseInt($("#productIdx").val());
		//let quantity = parseInt($("#quantitys").text());
		MovePath(`/Product/Buy/${productIdx}?isCancel=false`);
	});

	$("#btnSendAuthCode").click(function () {
		_checkParams = "#memberEmail";

		if (CheckParams()) {
			CallAjax("sendAuthCode");
		}
	});

	$("#btnCheckAuthCode").click(function () {
		_checkParams = "#memberEmail, #authCode";

		if (CheckParams()) {
			if (_isAuthSend) {
				if ($("#authSendKey").val().length !== 32) {
					MovePath("/Error");
				}
				else {
					CallAjax("checkAuthCode");
				}
			}
			else {
				alert("인증코드 전송을 해주세요.");
			}
		}
	});

	$("#btnSignup").click(function () {
		_checkParams = "#memberEmail, #authCode, #memberPassword, #checkMemberPassword, #memberName, #hpNumber, #shippingAddress, #coinAddress";

		if (CheckParams()) {
			if (_isAuthSend) {
				if ($("#authSendKey").val().length !== 32) {
					MovePath("/Error");
				}
				else {
					if (_isAuthCheck) {
						if ($("#authCheckKey").val().length !== 32) {
							MovePath("/Error");
						}
						else {
							if (!$('input:checkbox[id="checkTermsOfUse"]').is(":checked") || !$('input:checkbox[id="checkPrivacyPolicy"]').is(":checked")) {
								alert("약관에 동의 해주세요.");
							}
							else {
								if (confirm("회원가입 하시겠습니까?")) {
									CallAjax("signup");
								}
							}
						}
					}
					else {
						alert("이메일 인증을 해주세요.");
					}
				}
			}
			else {
				alert("인증코드 전송을 해주세요.");
			}
		}
	});

	$(document).on("click", ".btnCopy", function () {
		Copy($(".copyThis").text());
	});

	$("#btnResetPassword").click(function () {
		_checkParams = "#memberEmail, #authCode";

		if (CheckParams()) {
			if (_isAuthSend) {
				if ($("#authSendKey").val().length !== 32) {
					MovePath("/Error");
				}
				else {
					if (_isAuthCheck) {
						if ($("#authCheckKey").val().length !== 32) {
							MovePath("/Error");
						}
						else {
							MovePath("/resetpassword/" + $("#memberEmail").val() + "/" + $("#authSendKey").val() + "/" + $("#authCheckKey").val(), "2");
						}
					}
					else {
						alert("이메일 인증을 해주세요.");
					}
				}
			}
			else {
				alert("인증코드 전송을 해주세요.");
			}
		}
	});

	$("#btnEditPassword").click(function () {
		_checkParams = "#newPassword, #newCheckPassword";

		if (CheckParams()) {
			if ($("#authSendKey").val().length !== 32 && $("#authCheckKey").val().length !== 32) {
				MovePath("/Error");
			}
			else {
				CallAjax("editPassword");
			}
		}
	});

	$(".tabBtn").click(function () {
		const obj = $(this);
		const turn = obj.index();

		obj.addClass("active");
		_areaDataId = obj.data("list");

		if (turn === 1) {
			GetList();
		}

		$(".g-tab-container").eq(turn).addClass("active");
	});

	$("#areaTabOrderList").click(function () {
		MovePath('/My/OrderList');
	});

	$("#btnServiceWithdrawal").click(function () {
		if (!$('input:checkbox[id="isCheckServiceWithdrawal"]').is(":checked")) {
			alert("약관에 동의 해주세요.");
			return;
		}

		_checkParams = "#memberPassword";

		if (CheckParams()) {
			CallAjax("checkCurrentPassword", true, false);
		}
	});

	$("button[id^='btn_areaInput'], button[id^='btn_areaView']").click(function () {
		let obj = $(this);
		let id = obj.attr("id");
		obj.parent().parent().addClass("hide");
		$("#" + id.replace("btn_", "")).removeClass("hide");
		$("#areaInput" + id.replace("btn_areaInput", "").replace("btn_areaView", "")).find("input").val("");
	});

	$(".btnEditMember").click(function () {
		let typeCode = $(this).data("typecode");

		if (typeCode === "0001") {
			_checkParams = "#memberPassword, #newPassword, #newCheckPassword";
		}
		else if (typeCode === "0002") {
			_checkParams = "#hpNumber";
		}
		else if (typeCode === "0003") {
			_checkParams = "#shippingAddress";
		}
		else if (typeCode === "0004") {
			_checkParams = "#coinAddress";
		}

		if (CheckParams()) {
			if (confirm("변경하시겠습니까?")) {
				$("#memberEditTypeCode").val(typeCode);
				CallAjax("editMemberInfo", false, false);
			}
		}
	});

	$(".faq .faqtitle").click(function () {
		$(this).siblings(".faqtitle").removeClass("active");
		$(this).toggleClass("active");
		$(this).siblings(".answer").stop().slideUp();
		$(this).next().stop().slideToggle();
	});

	$("#btnOrderListSearch").click(function () {
		let movePath = _info.originalPath + "?searchStartDay=" + $("#searchStartDay").val() + "&searchEndDay=" + $("#searchEndDay").val();
		movePath += "&searchStateCode=" + $("#searchStateCode option:selected").val();
		movePath += "&searchProductName=" + $("#searchProductName").val();
		MovePath(movePath);
	});

	$("#isDefaultShippingAddress").click(function () {
		ShippingAddressControl();
	});

	$("input:radio[name='orderTypeCode']").click(function () {
		OrderTypeControl();
	});
});

function GetQueryStringValue(key) {
	const queryString = window.location.search;
	const urlParams = new URLSearchParams(queryString);

	return urlParams.get(key);
}

function Loading(isOff) {
	const obj = $(".pageLoaging-bg");

	if (isOff) {
		obj.addClass("hide");
	}
	else {
		obj.removeClass("hide");
	}
}

function MovePath(url, type) {
	type = type === undefined ? "1" : type;

	if (type === "1") {
		location.href = url;
	}
	else if (type === "2") {
		location.replace(url);
	}
	else if (type === "3") {
		window.open(url);
	}
}

function SetComma(value, isComma) {
	isComma = isComma === undefined ? true : isComma;

	if (isComma) {
		value = parseFloat(value);
		value = value.toString();

		var processValue = value;
		var appendValue = "";
		var isDecimal = false;

		if (value.indexOf(".") !== -1) {
			isDecimal = true;
			processValue = value.split(".")[0];
			appendValue = value.split(".")[1];
		}

		var commaValue = processValue.replace(/(\d)(?=(?:\d{3})+(?!\d))/g, '$1,');

		if (isDecimal) {
			value = commaValue + "." + appendValue;
		}
		else {
			value = commaValue;
		}
	}

	return value;
}

function SetRemoveComma(value) {
	if (value.toString().indexOf(",") !== -1) {
		value = value.toString().replace(/,/g, "");
	}

	return value;
}

function Login() {
	_checkParams = "#loginId, #loginPassword";

	if (CheckParams()) {
		CallAjax("login");
	}
}

function Logout() {
	CallAjax("logout", false, false);
}

function Modal(type, isOff) {
	isOff = isOff === undefined ? false : isOff;

	const obj = $("#modal" + type);

	if (obj) {
		if (type === "login") {
			obj.find("input").val("");
		}
		obj.modal(isOff ? "hide" : { backdrop: "static", keyboard: false });
	}
}

function Calendar() {
	$("#searchStartDay, #searchEndDay").datepicker({
		showAnim: "",
		beforeShow: function (input, inst) {
			var startDateLeft = $("#searchStartDay").offset().left;
			var startDateWidth = $("#searchStartDay").outerWidth(true);
			var endDateWidth = $("#searchEndDay").outerWidth(true);

			setTimeout(() => {
				$("#ui-datepicker-div").css({
					left: startDateLeft,
					width: startDateWidth + endDateWidth + 35,
				});
			});
		},
		onSelect: function (date) {
			$(this).addClass("ui-state-custom");
			return [true, "ui-state-custom", "closed"];
		},
	});

	$("#searchStartDay, #searchEndDay").attr("readonly", true);
	$("#searchStartDay, #searchEndDay").datepicker("option", { maxDate: "today" });

	$("div.ui-datepicker").on("click", function () {
		var startDateWidth = $("#searchStartDay").outerWidth(true);
		var endDateWidth = $("#searchStartDay").outerWidth(true);

		$(this).outerWidth(startDateWidth + endDateWidth + 35);
	});
}

function Payment() {
	let orderTypeCode = $("input:radio[name='orderTypeCode']:checked").val();
	let isPaypal = orderTypeCode === "0002";
	_checkParams = "#shippingAddress";

	if (!isPaypal) {
		_checkParams += ",#depositName";
	}

	if (CheckParams()) {
		if (confirm("구매 하시겠습니까?")) {
			CallAjax(isPaypal ? "payment" : "checkAccount");
		}
	}
}

function Copy(text) {
	var copyArea = $("<textarea></textarea>").text(text);

	$("body").append(copyArea);
	copyArea.select();
	document.execCommand("copy");
	copyArea.remove();

	alert("복사되었습니다.");
}

function MainNotice() {
	var hideModalToday = localStorage.getItem('hideModalToday');
	var currentDate = new Date().toDateString();

	if (!hideModalToday || hideModalToday !== currentDate) {
		Modal("Notice");
	}

	$("#modalNotice").find(".closeBtn").on('click', function () {
		if ($('#isTodayHide').prop('checked')) {
			localStorage.setItem('hideModalToday', currentDate);
		} else {
			localStorage.removeItem('hideModalToday');
		}
	});
}

function SetOrderCount(isMinus) {
	if (isMinus === undefined) {
		isMinus = false;
	}

	let count = parseInt($("#quantitys").text());
	let status = isMinus ? count > 1 : count < 10;

	if (status) {
		if (isMinus) {
			count--;
		}
		else {
			count++;
		}

		$("#quantitys").text(count);
		$("#areaTotalAmount").text((parseInt($("#amount").val()) * count).toLocaleString() + "￦");
	}
}

function CheckOrderCancel() {
	if (JSON.parse($("#isCancel").val())) {
		$(".toastMessage").show();
		$(".toastGbtn").click(function () {
			$(".toastMessage").fadeOut();
		});
	}
}

function MobileMenu() {
	$(".side-menu").hide();
	$(".hamburger").click(function () {
		$(this).next(".side-menu").fadeIn();
		$(this).next(".side-menu").find(".menu-contents").addClass("active");
	});
	$(".clickClose").click(function () {
		$(this).parent(".side-menu").fadeOut();
		$(this).next(".menu-contents").removeClass("active");
	});
	$(".g-icon-btn.closeSideMenu").click(function () {
		$(this).parents(".side-menu").fadeOut();
		$(this).parent(".menu-contents").removeClass("active");
	});
}

function loadAndFadeImage(url) {
	$("<img />")
		.attr("src", url)
		.on("load", function () {
			$(this).remove();
			$("#areaProductList").css('background-image', 'url("' + url + '")').fadeIn();
		});
}

function SwiperProductList() {
	let areaProductList = new Swiper("#areaProductList", {
		slidesPerView: 1,
		spaceBetween: 30,
		loop: true,
		pagination: {
			el: ".swiper-pagination",
			clickable: true,
			dynamicBullets: true,
			type: "bullets",
		},
		navigation: {
			nextEl: ".swiper-button-next",
			prevEl: ".swiper-button-prev",
		},
	});

	areaProductList.on("slideChangeTransitionEnd", function () {
		let index = (areaProductList.realIndex + _backgroundImageUrls.length) % _backgroundImageUrls.length;
		loadAndFadeImage(_backgroundImageUrls[index]);
	});
}

function OrderTypeControl() {
	let orderTypeCode = $("input:radio[name='orderTypeCode']:checked").val();
	$(".orderTypeInfo_0001, .orderTypeInfo_0002").removeClass("active");
	$(".orderTypeInfo_" + orderTypeCode).addClass("active");

	if (orderTypeCode === "0001") {
		alert("무통장 입금으로 구매하실 경우, 구매 시 입력한 주소가 아닌 최종 주소로 발송되며, 오후 7시(KST)에 순차적으로 일괄 발송됩니다.\r\n오후 6시(KST) 이후 구매된 디지털 굿즈는 익일 발송됩니다.\r\n감사합니다.\r\n\r\nWhen purchasing through bank transfer, your digital goods will be shipped to your final address, not the one entered during checkout, and they will be dispatched in order at 7 PM(KST).Digital goods purchased after 6 PM(KST) will be shipped the following day.\r\nThank you.");
	}
}

function ShippingAddressControl() {
	let isChecked = $("#isDefaultShippingAddress").is(":checked");
	$("#shippingAddress").val(isChecked ? $("#areaDefaultShippingAddress").text() : "").attr("readonly", isChecked);
}