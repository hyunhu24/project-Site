const _blockSize = 10;
let _flagGetList = false;
let _isListFlag = false;
let _totalCount = 0;
let _areaDataId = "";
let _callUrl = "";
let _page = 0;

$(function () {
	GetList();
});

function CallListData() {
	if (_isListFlag) return false;
	_isListFlag = true;

	let isLoading = true;
	$("#page").val(_page);

	$.ajax({
		type: "post",
		url: _callUrl,
		dataType: "json",
		cache: false,
		data: $("form").serialize(),
		beforeSend: function () {
			if (isLoading) {
				Loading();
			}
		},
		success: function (response) {
			PagingCallback(response);
		},
		error: function () {
			MovePath("/Error", "2");
		},
		complete: function () {
			if (isLoading) {
				setTimeout(function () {
					Loading(true);
				}, 300);
			}

			_isListFlag = false;
		}
	});
}

function PagingCallback(response) {
	const columnCount = $("#" + _areaDataId).prev().find("th").length;
	$("#" + _areaDataId).children().remove();

	if (response) {
		_totalCount = parseInt(response[0].TOTAL_COUNT);
		PagingCall(_page);

		$.each(response, function (index) {
			DataMapping(response, index);
		});
	} else {
		_totalCount = 1;
		PagingCall(1);

		let emptyHtml;		
		if (_info.path === "/notice") {
			emptyHtml = `<div class="no-history">내역이 없습니다.</div>`;
		}
		else {
			emptyHtml = `<tr class="no-history"><td colspan="${columnCount}" style="text-align:center;" id="emptyList">내역이 없습니다.</td></tr>`;
		}

		$("#" + _areaDataId).html(emptyHtml);
	}

	$("#totalCount").text(response ? _totalCount.toLocaleString() : 0);
}

function DataMapping(response, index) {
	let html = "";

	if (_areaDataId === "areaNoticeList") {
		html += `
			<div class="post" onclick="MovePath('/notice/info/${response[index].NOTICE_IDX}')">
				<div class="title">${response[index].TITLE}</div>
				<div class="date">${response[index].INS_DATE.substring(2, 10).replace(/-/g, "/")}</div>
			</div>
		`;
	}
	else if (_areaDataId === "areaOrderList") {
		let tokenId = parseInt(response[index].TOKEN_ID);
		html += `
			<tr class="infoTable"> 
				<td class="thumbnail" onClick="MovePath('/My/Order/${response[index].ORDER_NUMBER}/Detail?paymentType=${response[index].ORDER_TYPE_CODE}')">
					<div class="thumbnailImg">
						<img src="${response[index].THUMBNAIL_IMAGE_URL}" alt="thumbnail img" />
					</div>
					<div class="thumbnailText">
						<div class="thumbnailTitle">${response[index].PRODUCT_NAME}</div> 
						<div>${response[index].AMOUNT}￦</div> 
					</div>
				</td>
				<td>${response[index].INS_DATE}</td>
				<td>${response[index].ORDER_NUMBER}</td> 
				<td>${tokenId > 0 ? `#` + tokenId : `-`}</td>
				<td>${response[index].ORDER_AMOUNT}￦</td> 
				<td>${response[index].STATE_CODE_NAME}</td> 
			</tr>
		`;
	}

	$("#" + _areaDataId).append(html);
}

function PagingCall(page) {	
	let areaPaging = ".pagination";
	let pageSize = _info.pageSize;
	let divisionCount = _totalCount / pageSize;
	let divisionRestLen = _totalCount % pageSize;
	let totalPageCount = 1;
	let divisionAddCount = 0;
	if (divisionRestLen > 0) {
		divisionAddCount = 1;
	}
	totalPageCount = parseInt(divisionCount) + divisionAddCount;

	let maxPageAddCount = _blockSize - 1;
	let pagingMinDivisionCount = parseInt(page) / _blockSize;
	let pagingMinRestCount = parseInt(page) % _blockSize;
	let pagingMinCount = 1;
	let pagingMaxCount = 1;

	if (parseInt(pagingMinDivisionCount) >= 1) {
		pagingMinCount = (parseInt(pagingMinDivisionCount) * _blockSize) + 1;
	}
	if (parseFloat(pagingMinRestCount) === 0) {
		pagingMinCount = parseInt(pagingMinCount) - _blockSize;
	}

	if ((totalPageCount - pagingMinCount) < maxPageAddCount) {
		maxPageAddCount = totalPageCount - pagingMinCount;
	}
	if ((pagingMinCount + maxPageAddCount) <= totalPageCount) {
		pagingMaxCount = pagingMinCount + maxPageAddCount;
	} else if (parseFloat(pagingMinCount) === parseFloat(totalPageCount)) {
		pagingMaxCount = totalPageCount;
	}

	let prevPage = parseInt(page) - 1;
	let nextPage = parseInt(page) + 1;
	let prevMoveType = 0;
	let moveType = 0;
	let nextMoveType = 0;

	if (parseFloat(page) === 1) {
		prevMoveType = 1;
	} else {
		prevMoveType = 0;
	}

	if (parseFloat(page) === totalPageCount) {
		nextMoveType = 1;
	} else {
		nextMoveType = 0;
	}

	$(areaPaging).children().remove();
	let appendData = "";

	if (prevMoveType === 0) {
		appendData += "<li class='page-item prev-next'><a class='page-link' href='javascript:MovePage(" + prevPage + ", " + prevMoveType + ");'>이전</a></li>";
	}
	else {
		appendData += "<li class='page-item prev-next'><a class='page-link'>이전</a></li>";
	}

	for (let i = pagingMinCount; i <= pagingMaxCount; i++) {
		moveType = parseFloat(page) === i ? 1 : 0;

		if (parseFloat(moveType) === 0) {
			appendData += "<li class='page-item'><a class='page-link' href='javascript:MovePage(" + i + ", " + moveType + ");'>" + i + "</a></li>";
		} else {
			appendData += "<li class='page-item active'><a class='page-link'>" + i + "</a></li>";
		}
	}

	if (nextMoveType === 0) {
		appendData += "<li class='page-item prev-next'><a class='page-link' href='javascript:MovePage(" + nextPage + ", " + nextMoveType + ");'>다음</a></li>";
	}
	else {
		appendData += "<li class='page-item prev-next'><a class='page-link'>다음</a></li>";
	}

	$(areaPaging).append(appendData);
}

function MovePage(page, moveType) {
	if (SetIsPaging()) {
		_page = page;

		if (moveType === 0) {
			CallListData();
		}
	}
}

function GetList() {
	if (_flagGetList) return false;
	_flagGetList = true;

	if (SetIsPaging()) {
		_page = 1;
		CallListData();
	}

	_flagGetList = false;
}

function SetIsPaging() {
	let isPaging = true;
	let path = _info.path;

	if (path === "/notice") {
		_areaDataId = "areaNoticeList";
		_callUrl = "/ListData";
	}
	else if (path === "/faq") {
		_areaDataId = "areaFaqList";
		_callUrl = "/ListData";
	}
	else if (path === "/my/orderlist") {
		_areaDataId = "areaOrderList";
		_callUrl = "/Order/ListData";
	}
	else {
		isPaging = false;
	}

	if (!isPaging) {
		_areaDataId = "";
		_callUrl = "";
	}

	return isPaging;
}