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
            "url": api_url + "contract-types?pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&isGetAll=false",
            "method": "GET",
            "timeout": 0,
        };
    } else {
        settings = {
            "url": api_url + "contract-types?pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&keySearch=" + keySearch + "&isGetAll=false",
            "method": "GET",
            "timeout": 0,
        };
    }

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        var html = "";

        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);

            var data = rs.data;
            var dataLength = data.length;

            for (var i = 0; i < dataLength; i++) {
                if (data[i].Code == null || data[i].Code === "undefined" || data[i].Code === "") {
                    data[i].Code = "-";
                }

                if (data[i].Name == null || data[i].Name === "undefined" || data[i].Name === "") {
                    data[i].Name = "-";
                }

                if (data[i].Note == null || data[i].Note === "undefined" || data[i].Note === "") {
                    data[i].Note = "-";
                }

                if (data[i].CreatedOn == null || data[i].CreatedOn === "undefined" || data[i].CreatedOn === "") {
                    data[i].CreatedOn = "-";
                } else {
                    data[i].CreatedOn = formatDate(new Date(data[i].CreatedOn), "mm-dd-yyyy");
                }

                if (data[i].ModifiedOn == null || data[i].ModifiedOn === "undefined" || data[i].ModifiedOn === "") {
                    data[i].ModifiedOn = "-";
                } else {
                    data[i].ModifiedOn = formatDate(new Date(data[i].ModifiedOn), "mm-dd-yyyy");
                }

                html += `<tr>`;
                html += `<td class="text-center"><input value="${data[i].ID}" type="checkbox" class="ckRow" /></td>`;
                html += `<td onclick="contractTypeDetails(\`` + data[i].ID + `\`)"><span class="ivg-view-detail">${data[i].Code}</span></td>`;
                html += `<td>${data[i].Name}</td>`;
                html += `<td>${data[i].CreatedOn}</td>`;
                html += `<td>${data[i].ModifiedOn}</td>`;
                html += `<td>${data[i].Note}</td>`;
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


/*Create contract type*/
function createContractType() {
    var code = $("#txtCode").val();
    if (code == null || code === "" || code === "undefined") {
        alertWarning("Vui lòng nhập mã loại hợp đồng");
        return;
    }

    var name = $("#txtName").val();
    if (name == null || name === "" || name === "undefined") {
        alertWarning("Vui lòng nhập tên loại hợp đồng");
        return;
    }

    var note = $("#txtNote").val();

    var settings = {
        "url": api_url + "create-contract-type",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "Code": code,
            "Name": name,
            "Note": note
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            $("#loader").hide();
            $("#modalCreate").modal("hide");
            onLoad();
            alertSuccess(rs.messVN);
            $("#txtCode").val("");
            $("#txtName").val("");
            $("#txtNote").val("");
            console.log(rs);
        } else {
            $("#loader").hide();
            alertError(rs.messVN);
            console.log(rs);
        }
    });
}


/*Contract type details*/
function contractTypeDetails() {

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