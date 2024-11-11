/*User*/
var user = JSON.parse(storageGetItem("localStorageList"));
var role = user.data.Role;
var userID = user.data.CustomerID;

$(document).ready(function () {
    onLoad();

    $('.ckAll').on("change", function () {
        var checked = $(this).prop('checked');
        $('.ckRow').prop('checked', checked);
    });
});


/*Onload*/
function onLoad(key) {
    var settings = {};

    var keySearch = $("#txtMainSearch").val();

    /*Paging*/
    var pageNumber = $("#pageIndex").val();
    var pageSize = $("#pageSize").val();

    if (key) {
        pageIndex = 1;
        $("#pageIndex").val(1);
    }

    if (keySearch == null || keySearch === "" || keySearch === "undefined") {
        settings = {
            "url": api_url + "customers?pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&isGetAll=false",
            "method": "GET",
            "timeout": 0,
        };
    } else {
        settings = {
            "url": api_url + "customers?pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&keySearch=" + keySearch + "&isGetAll=false",
            "method": "GET",
            "timeout": 0,
        };
    }

    console.log(settings);

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        var html = "";

        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);

            var data = rs.data;
            var dataLength = data.length;

            for (var i = 0; i < dataLength; i++) {
                if (data[i].SAPCode == null || data[i].SAPCode === "undefined" || data[i].SAPCode === "") {
                    data[i].SAPCode = "-";
                }

                if (data[i].TaxCode == null || data[i].TaxCode === "undefined" || data[i].TaxCode === "") {
                    data[i].TaxCode = "-";
                }

                if (data[i].FullName == null || data[i].FullName === "undefined" || data[i].FullName === "") {
                    data[i].FullName = "-";
                }

                if (data[i].PhoneNumber == null || data[i].PhoneNumber === "undefined" || data[i].PhoneNumber === "") {
                    data[i].PhoneNumber = "-";
                }

                if (data[i].Email == null || data[i].Email === "undefined" || data[i].Email === "") {
                    data[i].Email = "-";
                }

                html += `<tr>`;
                html += `<td class="text-center"><input value="${data[i].ID}" type="checkbox" class="ckRow" /></td>`;
                html += `<td><span class="ivg-view-detail">${data[i].SAPCode}</span></td>`;
                html += `<td><span>${data[i].TaxCode}</span></td>`;
                html += `<td><span>${data[i].FullName}</span></td>`;
                html += `<td><span>${data[i].PhoneNumber}</span></td>`;
                html += `<td><span>${data[i].Email}</span></td>`;
                html += `</tr>`;
            }

            pageIndexLoad(rs.paging.PageIndex, rs.paging.TotalPages, rs.paging.TotalRecords);
        } else {
            $("#loader").hide();
            console.log(rs);
            html += `<tr class='tr-mb-style'>`;
            html += `<td class='emptytable' colspan='9999'>${rs.messVN}</td>`;
            html += `</tr>`;

            pageIndexLoad(1, 1, 0);
        }

        $('#dataBody').html(html);
    });
}


/*Pagination*/
function pageIndexClick(e) {
    var pageIndex = $("#pageIndex").val();

    if (e == 'previous') {
        var newPageIndex = parseInt(pageIndex) - 1;;
        $("#pageIndex").val(newPageIndex);
    } else if (e == 'next') {
        var newPageIndex = parseInt(pageIndex) + 1;
        $("#pageIndex").val(newPageIndex);
    } else if (e == 'first') {
        $("#pageIndex").val(1);
    } else {
        var total = $("#totalPages").val();

        $("#pageIndex").val(total);
    }

    onLoad();
}

function pageIndexLoad(pageIndex, totalPages, totalRecords) {
    pageIndex = $("#pageIndex").val();

    $("#totalPages").val(totalPages);

    $("#totalRecords").text(formatNumber(totalRecords));
    $("#pageIndexText").text(formatNumber(pageIndex));
    $("#totalPagesText").text(formatNumber(totalPages));

    if (pageIndex == 1) {
        $("#firstPage").addClass('disabled');
        $("#previousPage").addClass('disabled');
    } else {
        $("#firstPage").removeClass('disabled');
        $("#previousPage").removeClass('disabled');
    }

    if (pageIndex == totalPages) {
        $("#lastPage").addClass('disabled');
        $("#nextPage").addClass('disabled');
    } else {
        $("#lastPage").removeClass('disabled');
        $("#nextPage").removeClass('disabled');
    }
}