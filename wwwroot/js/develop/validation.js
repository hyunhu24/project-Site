let _title;
let _checkParams;

function SetTitle(id) {
	if (id === "authCode") _title = "인증 코드";
	else if (id === "memberEmail") _title = "이메일";
	else if (id === "hpNumber") _title = "휴대폰 번호";
	else if (id === "shippingAddress") _title = "배송지 주소";
	else if (id === "coinAddress") _title = "지갑 주소";
	else _title = $("label[for='" + id + "']").text().replace("*", "");
}

function CheckParams() {
	let status = true;
	$("body").find(_checkParams).each(function () {
		let paramObj = $(this);

		if (paramObj.length > 0) {
			status = CheckValidationRequired(paramObj);

			if (!status) {
				paramObj.focus();
				return false;
			}
		}
	});

	return status;
}

function CheckValidationRequired(obj) {
	var element;
	var value;
	var id = obj.attr("id");
	var status = true;
	var isRequired = obj.prop("required");

	if (id) {
		element = $(obj).prop("tagName");

		if (element.toLowerCase() === "select") {
			value = $.trim($("#" + id + " option:selected").val());
		}
		else {
			value = $.trim(obj.val());
		}

		SetTitle(id);
		
		if (isRequired) {
			if (value.length < 1) {
				status = false;
				alert("필수 정보 입니다(" + _title + ")");
			}
		}

		if (status) {
			status = CheckValidationPattern(id, isRequired);
		}

		return status;
	}
}

function CheckValidationPattern(id, isRequired) {
	isRequired = !isRequired ? false : isRequired;
	
	var obj = $("#" + id);
	var status = true;
	var pattern;
	var message = "";
	var element = obj.prop("tagName");
	var value;
	var valueLength;
	var isBeforeCheck = false;

	if (element.toLowerCase() === "select") {
		value = $("#" + id + " option:selected").val();
	}
	else {
		value = obj.val();
	}

	valueLength = value.length;

	if (isRequired || (!isRequired && valueLength > 0)) {
		if ($("#" + id).attr("maxlength")) {
			let checkLength = parseInt($("#" + id).attr("maxlength"));

			if (valueLength > checkLength) {
				status = false;
				message = _title + "은(는) " + checkLength + "자리까지 입력이 가능합니다.";
			}
		}

		if (status) {
			if (id === "loginId" || id === "memberEmail") {
				pattern = /^[A-Za-z0-9_+\.\-]+@[A-Za-z0-9\-]+\.[A-Za-z0-9\-]+/;
			}
			else if (id === "authCode") {
				pattern = /^[0-9]{4}$/;
			}
			else if (id === "loginPassword" || id === "memberPassword" || id === "newPassword") {
				pattern = /^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*()_+,-./:;<=>?@[\]^_`{|}~]{6,20}$/;
			}
			else if (id === "checkMemberPassword" || id === "newCheckPassword") {
				let isNew = id === "newCheckPassword";
				isBeforeCheck = true;
				status = value === $("#" + (isNew ? "newPassword" : "memberPassword")).val();

				if (!status) {
					if (isNew) {
						message = "새 비밀번호와 새 비밀번호 확인이 일치하지 않습니다.";
					}
					else {
						message = "비밀번호와 비밀번호 확인이 일치하지 않습니다.";
					}
				}
			}
			else if (id === "memberName") {
				pattern = /^([가-힣]{2,5}|[A-Za-z]+(?:\s[A-Za-z]+){0,2})$/;
			}
			else if (id === "hpNumber") {
				status = valueLength > 3;

				if (status) {
					pattern = value.substring(0, 3) === "010" ? /^010\d{7,8}$/ : /^\d{8,16}$/;
				}
			}
			else if (id === "shippingAddress") {
				isBeforeCheck = true;
				status = valueLength > 0;
			}
			else if (id === "coinAddress") {
				pattern = /^0x[a-fA-F0-9]{40}$/;
			}

			if (status && pattern && !isBeforeCheck) {
				status = pattern.test(value);
			}
		}

		if (!status) {
			message = !message ? _title + "을(를) 바르게 입력하세요." : message;
			alert(message);
		}
	}

	return status;
}