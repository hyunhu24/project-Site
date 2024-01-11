$(function () {
	$(".g-input-box input").blur(function () {
		$this = $(this);
		if ($this.val().length > 0) {
			$this.addClass("active");
		} else {
			$this.removeClass("active");
		}
	});
});

function fnExcelReport(id, title) {
	var tab_text = '<html xmlns:x="urn:schemas-microsoft-com:office:excel">';
	tab_text = tab_text + '<head><meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8">';
	tab_text = tab_text + "<xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>";
	tab_text = tab_text + "<x:Name>Test Sheet</x:Name>";
	tab_text = tab_text + "<x:WorksheetOptions><x:Panes></x:Panes></x:WorksheetOptions></x:ExcelWorksheet>";
	tab_text = tab_text + "</x:ExcelWorksheets></x:ExcelWorkbook></xml></head><body>";
	tab_text = tab_text + "<table border='1px'>";
	var exportTable = $("#" + id).clone();
	exportTable.find("input").each(function (index, elem) {
		$(elem).remove();
	});
	tab_text = tab_text + exportTable.html();
	tab_text = tab_text + "</table></body></html>";
	var data_type = "data:application/vnd.ms-excel";
	var ua = window.navigator.userAgent;
	var msie = ua.indexOf("MSIE ");
	var fileName = title + ".xls";
	//Explorer 환경에서 다운로드
	if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
		if (window.navigator.msSaveBlob) {
			var blob = new Blob([tab_text], {
				type: "application/csv;charset=utf-8;",
			});
			navigator.msSaveBlob(blob, fileName);
		}
	} else {
		var blob2 = new Blob([tab_text], {
			type: "application/csv;charset=utf-8;",
		});
		var filename = fileName;
		var elem = window.document.createElement("a");
		elem.href = window.URL.createObjectURL(blob2);
		elem.download = filename;
		document.body.appendChild(elem);
		elem.click();
		document.body.removeChild(elem);
	}
}

function initCalendar() {
	$("#startDate").click(function () {
		$("#startDate").addClass("active");
	});

	$("#startDate, #endDate").datepicker({
		showAnim: "",
		beforeShow: function (input, inst) {
			var startDateLeft = $("#startDate").offset().left;

			// var startDateWidth = $("#startDate").outerWidth(true);
			// var endDateWidth = $("#endDate").outerWidth(true);

			setTimeout(() => {
				$("#ui-datepicker-div").css({
					left: startDateLeft - 10,
					width: startDateWidth + endDateWidth + 55,
				});
			});
		},
		onSelect: function (date) {
			$(this).addClass("ui-state-custom");
			return [true, "ui-state-custom", "closed"];
		},
	});

	$("#startDate, #endDate").datepicker("option", { maxDate: new Date() });

	$("#startDate").addClass("active");
	$("#startDate").datepicker("setDate", new Date().last().week());
	$("#endDate").datepicker("setDate", new Date());

	$("div.ui-datepicker").on("click", function () {
		var startDateWidth = $("#startDate").outerWidth(true);
		var endDateWidth = $("#endDate").outerWidth(true);

		$(this).outerWidth(startDateWidth + endDateWidth + 55);
	});
}
