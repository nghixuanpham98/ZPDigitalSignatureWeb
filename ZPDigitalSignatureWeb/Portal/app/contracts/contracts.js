/*Define Parameters*/
var posX = 0;
var posY = 0;
var widthBlockSign = 220;
var heightBlockSign = 120;


/*WebSocket*/
const wsUri = "ws://127.0.0.1:8098/echo";
var websocket;


/*User*/
var user = JSON.parse(storageGetItem("localStorageList"));
var role = user.data.Role;
var userID = user.data.CustomerID;

var url_string = window.location.href;
var url = new URL(url_string);
var idContract = url.searchParams.get("id1");
var idFile = url.searchParams.get("id2");
var nameContract = url.searchParams.get("name");
var isFirstTime = url.searchParams.get("isFTime");
var statusContract = url.searchParams.get("status");


$(document).ready(function () {
    if (role == 2) {
        $("#btnCreateNew").hide();
        $("#btnDelete").hide();
    } else {
        $("#btnCreateNew").show();
        $("#btnDelete").show();
    }

    onLoad();
    getContractTypes();
    getContractCustomers();

    $('.ckAll').on("change", function () {
        var checked = $(this).prop('checked');
        $('.ckRow').prop('checked', checked);
    });

    $('.signInput').bind('keydown', function (event) {
        switch (event.keyCode) {
            case 8:  // Backspace
            case 9:  // Tab
            case 13: // Enter
            case 37: // Left
            case 38: // Up
            case 39: // Right
            case 40: // Down
                break;
            default:
                var regex = new RegExp("^[a-zA-Z0-9.,/ $@()]+$");
                var key = event.key;
                if (!regex.test(key)) {
                    event.preventDefault();
                    return false;
                }
                break;
        }
    });
});


/*Onload*/
function onLoad(key) {
    var keySearch = $("#txtMainSearch").val();

    /*Paging*/
    var pageNumber = $("#pageIndex").val();
    var pageSize = $("#pageSize").val();

    if (key) {
        pageIndex = 1;
        $("#pageIndex").val(1);
    }

    var settings = {
        "url": api_url + "contracts",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "PageNumber": pageNumber,
            "PageSize": pageSize,
            "Role": role,
            "KeySearch": keySearch,
            "CustomerID": userID
        }),
    };

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

                if (data[i].FullName == null || data[i].FullName === "undefined" || data[i].FullName === "") {
                    data[i].FullName = "-";
                }

                if (data[i].FileName == null || data[i].FileName === "undefined" || data[i].FileName === "") {
                    data[i].FileName = "-";
                }

                if (data[i].ContractType == null || data[i].ContractType === "undefined" || data[i].ContractType === "") {
                    data[i].ContractType = "-";
                }

                if (data[i].Note == null || data[i].Note === "undefined" || data[i].Note === "") {
                    data[i].Note = "-";
                }

                if (data[i].RejectNote == null || data[i].RejectNote === "undefined" || data[i].RejectNote === "") {
                    data[i].RejectNote = "-";
                }

                if (data[i].CreatedOn == null || data[i].CreatedOn === "undefined" || data[i].CreatedOn === "") {
                    data[i].CreatedOn = "-";
                } else {
                    data[i].CreatedOn = formatDate(new Date(data[i].CreatedOn), "mm-dd-yyyy");
                }

                html += `<tr>`;
                html += `<td class="text-center"><input value="${data[i].ID}&${data[i].FileContractID}" type="checkbox" class="ckRow ckContract" /></td>`;

                if (role == 2) {
                    html += `<td><span class="ivg-view-detail">${data[i].SAPCode}</span></td>`;
                } else {
                    html += `<td><span title="Cập nhật" onclick="contractDetails(\`` + data[i].ID + `\`)" class="ivg-view-detail">${data[i].SAPCode}</span></td>`;
                }

                html += `<td><span>${data[i].FullName}</span></td>`;
                html += `<td><span>${data[i].ContractName}</span></td>`;
                html += `<td><span>${data[i].ContractTypeName}</span></td>`;
                html += `<td><span>${data[i].Note}</span></td>`;
                html += `<td><span>${data[i].CreatedOn}</span></td>`;

                var status = data[i].Status;
                if (status == 0) {
                    status = "Đang xử lý";
                    html += `<td><span class="color-status__blue fw-bold">${status}</span></td>`;
                } else if (status == 1) {
                    status = "Đã ký";
                    html += `<td><span class="color-status__green fw-bold">${status}</span></td>`;
                } else if (status == 2) {
                    status = "Nháp";
                    html += `<td><span class="color-status__gray fw-bold">${status}</span></td>`;
                } else if (status == 3) {
                    status = "Từ chối";
                    html += `<td><span class="color-status__red fw-bold">${status}</span></td>`;
                } else {
                    status = "Hoàn thành";
                    html += `<td><span class="color-status__yellow fw-bold">${status}</span></td>`;
                }

                html += `<td><span>${data[i].RejectNote}</span></td>`;

                if (data[i].Status == 0) {
                    html += `<td class="text-center">`;
                    html += `<span>`;
                    html += `<i onclick="signContract(\`` + data[i].ID + `\`, \`` + data[i].FileContractID + `\`, \`` + data[i].ContractName + `\`, \`` + false + `\`, 0)" class="font-size__1 ivg-view-detail fa-solid fa-pencil me-3"></i>`;
                    html += `<i onclick="openModalReject(\`` + data[i].ID + `\`)" title="Từ chối ký" class="font-size__1 ivg-view-detail fa-solid fa-x"></i>`;
                    html += `</span></td >`;
                } else if (data[i].Status == 1 && role == 1) {
                    html += `<td class="text-center">`;
                    html += `<span>`;
                    html += `<i onclick="signContract(\`` + data[i].ID + `\`, \`` + data[i].FileContractID + `\`, \`` + data[i].ContractName + `\`, \`` + false + `\`, 1)" class="font-size__1 ivg-view-detail fa-solid fa-pencil me-3"></i>`;
                    html += `<i title="Từ chối ký" class="font-size__1 table-icon__disabled ivg-view-detail fa-solid fa-x"></i>`;
                    html += `</span></td >`;
                } else if (data[i].Status == 1 && role == 2) {
                    html += `<td class="text-center">`;
                    html += `<span>`;
                    html += `<i onclick="signContract(\`` + data[i].ID + `\`, \`` + data[i].FileContractID + `\`, \`` + data[i].ContractName + `\`, \`` + true + `\`, 0)" class="font-size__1 ivg-view-detail fa-solid fa-pencil me-3"></i>`;
                    html += `<i title="Từ chối ký" class="font-size__1 table-icon__disabled ivg-view-detail fa-solid fa-x"></i>`;
                    html += `</span></td >`;
                } else if (data[i].Status == 2) {
                    html += `<td class="text-center">`;
                    html += `<span>`;
                    html += `<i onclick="signContract(\`` + data[i].ID + `\`, \`` + data[i].FileContractID + `\`, \`` + data[i].ContractName + `\`, \`` + false + `\`, 0)" class="font-size__1 ivg-view-detail fa-solid fa-pencil me-3"></i>`;
                    html += `<i title="Từ chối ký" class="font-size__1 table-icon__disabled ivg-view-detail fa-solid fa-x"></i>`;
                    html += `</span></td >`;
                } else if (data[i].Status == 3 || data[i].Status == 4) {
                    html += `<td class="text-center">`;
                    html += `<span>`;
                    html += `<i onclick="signContract(\`` + data[i].ID + `\`, \`` + data[i].FileContractID + `\`, \`` + data[i].ContractName + `\`, \`` + true + `\`, 0)" class="font-size__1 ivg-view-detail fa-solid fa-pencil me-3"></i>`;
                    html += `<i title="Từ chối ký" class="font-size__1 table-icon__disabled ivg-view-detail fa-solid fa-x"></i>`;
                    html += `</span></td >`;
                }

                html += `</tr>`;
            }

            pageIndexLoad(rs.paging.PageIndex, rs.paging.TotalPages, rs.paging.TotalRecords);

            if (idContract && isFirstTime === "true" && statusContract == 3) {
                signContract(idContract, idFile, nameContract, `true`, statusContract);

                isFirstTime = "false";
            } else if (idContract && isFirstTime === "true") {
                signContract(idContract, idFile, nameContract, `false`, statusContract);

                isFirstTime = "false";
            }
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


/*Create Place Signature*/
function createPlaceSignature() {
    $("#btnPlace").hide();
    $("#btnClear").show();
    $("#btnSign").show();
    $("#btnReject").hide();

    var html = "";
    html += `<div class="block-resize__item"></div>`;

    $("#blockResize").append(html);
    $(".block-resize__item").draggable({
        containment: $('#blockResize'),
        stop: function () {
            var position = $(this).position();
            var x = 612;
            var y = 792;

            posX = position.left;
            posY = (792 - heightBlockSign) - (position.top);

            console.log("x = " + posX, "y = " + posY);
            console.log("width = " + widthBlockSign, "height = " + heightBlockSign);
        }
    }).resizable({
        containment: $('#blockResize'),
        stop: function (event, ui) {
            widthBlockSign = ui.size.width;
            heightBlockSign = ui.size.height;
        }
    });
}


/*Clear Place Signature*/
function clearPlaceSignature() {
    $("#btnPlace").show();
    $("#btnClear").hide();
    $("#btnSign").hide();
    $("#btnReject").show();

    $(".block-resize__item").remove();
    posX = 0;
    posY = 0;
    widthBlockSign = 220;
    heightBlockSign = 120;
}


/*Upload Image Sign*/
function uploadImage() {
    var filesSelected = document.getElementById("txtImageFile").files;
    var fileToLoad = filesSelected[0];

    var fileReader = new FileReader();

    fileReader.onload = function (fileLoadedEvent) {
        var srcData = fileLoadedEvent.target.result;
        var base64result = srcData.split(',')[1];

        var settings = {
            "url": api_url + "upload-file",
            "method": "POST",
            "timeout": 0,
            "headers": {
                "Content-Type": "application/json"
            },
            "data": JSON.stringify({
                "FileName": fileToLoad.name,
                "FileType": fileToLoad.type,
                "FileBase64": base64result
            }),
        };

        $("#loader").show();

        $.ajax(settings).done(function (rs) {
            if (rs.code == 200) {
                $("#loader").hide();
                console.log(rs);
                var fileImageID = rs.fileID;

                sendMessage(fileImageID);
            } else {
                $("#loader").hide();
                console.log(rs);
            }
        });
    }

    fileReader.readAsDataURL(fileToLoad);
}


/*Submit Sign*/
function submitSign() {
    websocket = new WebSocket(wsUri);
    handleEventWS();
}


/*Handle event from WS*/
function handleEventWS() {
    websocket.onopen = (e) => {
        if (e.type == "open") {
            var maxOfFile = 2000000;
            var filesSelected = document.getElementById("txtImageFile").files;

            if (filesSelected != null && filesSelected.length > 0) {
                var fileToLoad = filesSelected[0];

                if (fileToLoad.size >= maxOfFile) {
                    alertWarning("Dung lượng tệp không được vượt quá 2MB ");
                    return;
                } else {
                    uploadImage();
                }
            } else {
                setTimeout(function () {
                    sendMessage("");
                }, 1000);
            }
        }
    };

    websocket.onclose = (e) => {
        console.log(e);
    };

    websocket.onmessage = (e) => {
        var data = JSON.parse(e.data);

        if (data.code == 200) {
            $("#loader").hide();
            $("#modalSign").modal("hide");
            $("#txtReason").val("");
            $("#txtContact").val("");
            $("#txtLocation").val("");
            $("#txtImageFile").val("");
            alertSuccess(data.messVN);
            signContract();
            clearPlaceSignature();
            onLoad();
        } else {
            $("#loader").hide();
            alertError(data.messVN);
            console.log(data);
        }
    };

    websocket.onerror = (e) => {
        console.log(e);
    };
}


/*Send message to Websocket*/
function sendMessage(fileImageID) {
    var msg = {};
    var pageSign = $("#pageNumber").text();
    var reason = $("#txtReason").val();
    var location = $("#txtLocation").val();
    var contact = $("#txtContact").val();
    var fileSignID = $("#fileSignID").val();
    var signID = $("#contractID").val();
    var status = $("#contractStatus").val();

    if (status == 1) {
        msg = {
            type: "sign",
            data: {
                SignID: signID,
                FileSignID: fileSignID,
                FileImageID: fileImageID,
                PageSign: pageSign,
                PosX: posX,
                PosY: posY,
                WidthPlace: widthBlockSign,
                HeightPlace: heightBlockSign,
                Reason: reason,
                Contact: contact,
                Location: location,
                IsCompleted: 4
            }
        };
    } else {
        msg = {
            type: "sign",
            data: {
                SignID: signID,
                FileSignID: fileSignID,
                FileImageID: fileImageID,
                PageSign: pageSign,
                PosX: posX,
                PosY: posY,
                WidthPlace: widthBlockSign,
                HeightPlace: heightBlockSign,
                Reason: reason,
                Contact: contact,
                Location: location,
                IsCompleted: 1
            }
        };
    }

    $("#loader").show();

    websocket.send(JSON.stringify(msg));
}


/*Contract details*/
function contractDetails(id) {
    if (id) {
        $("#contractID").val(id);
        
        var settings = {
            "url": api_url + "contracts/" + id,
            "method": "GET",
            "timeout": 0,
        };

        $("#loader").show();

        $.ajax(settings).done(function (rs) {
            if (rs.code == 200) {
                console.log(rs);
                $("#loader").hide();
                var data = rs.data;

                $("#contractName").val(data.ContractName);
                $('#contractType').data('selectize').setValue(data.ContractTypeID);
                $('#contractCustomer').data('selectize').setValue(data.CustomerID);

                if (data.Status == 0) {
                    $("#ckContractStatus0").prop("checked", true);
                    $("#btnUpdateContract").show();
                } else if (data.Status == 1) {
                    $("#ckContractStatus0").prop("checked", true);
                    $("#btnUpdateContract").hide();
                } else if (data.Status == 2) {
                    $("#ckContractStatus2").prop("checked", true);
                    $("#btnUpdateContract").show();
                } else if (data.Status == 3) {
                    $("#ckContractStatus0").prop("checked", true);
                    $("#btnUpdateContract").hide();
                }

                $("#contractNote").val(data.Note);

                $("#modalUpdate").modal("show");
            } else {
                console.log(rs);
                $("#loader").hide();
            }
        });
    }
}


/*Update contract*/
function updateContract() {
    var contractID = $("#contractID").val();

    var contractName = $("#contractName").val();
    if (contractName == null || contractName === "" || contractName === "undefined") {
        alertWarning("Vui lòng nhập tên hợp đồng");
        return;
    }
    
    var contractTypeID = $("#contractType").val();
    if (contractTypeID == null || contractTypeID === "" || contractTypeID === "undefined" || contractTypeID == 0) {
        alertWarning("Vui lòng chọn loại hợp đồng");
        return;
    }

    var customerID = $("#contractCustomer").val();
    if (customerID == null || customerID === "" || customerID === "undefined" || customerID == 0) {
        alertWarning("Vui lòng chọn bên ký kết hợp đồng");
        return;
    }

    var contractStatus = $('input[name="ckContractStatus"]:checked').val();
    var note = $("#contractNote").val();

    var settings = {
        "url": api_url + "update-contract",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "ID": contractID,
            "CustomerID": customerID,
            "ContractTypeID": contractTypeID,
            "ContractName": contractName,
            "Status": contractStatus,
            "Note": note
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            $("#loader").show();
            console.log(rs);
            alertSuccess(rs.messVN);
            $("#modalUpdate").modal("hide");
            onLoad(1);
        } else {
            $("#loader").show();
            console.log(rs);
            alertError(rs.messVN);
        }
    });
}


/*Delete contract*/
function deleteContract() {
    swal({
        title: "Thông báo",
        text: "Bạn muốn xóa dữ liệu?",
        icon: "warning",
        buttons: true,
        buttons: ["Hủy", "Đồng ý"],
        dangerMode: true,
    })
        .then((doit) => {
            if (doit) {
                var data = [];

                $('.ckContract').each(function () {
                    if ($(this).prop('checked') == true) {
                        var id = $(this).val().split('&')[0];

                        data.push(id);
                    }
                });

                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var settings = {
                            "url": api_url + "contracts/" + data[i],
                            "method": "DELETE",
                            "timeout": 0,
                        };

                        $("#loader").show();

                        $.ajax(settings).done(function (rs) {
                            if (rs.code == 200) {
                                $("#loader").hide();
                                console.log(rs);
                                alertSuccess(rs.messVN);
                                onLoad();
                            } else {
                                $("#loader").hide();
                                console.log(rs);
                                alertError(rs.messVN);
                            }
                        });
                    }
                } else {
                    alertWarning("Vui lòng chọn thông tin muốn xóa");
                }
            }
        });
}


/*Get contract types*/
function getContractTypes() {
    var settings = {
        "url": api_url + "contract-types?isGetAll=true",
        "method": "GET",
        "timeout": 0,
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        var optionHtml = "";
        optionHtml += `<option value="0">Chọn loại hợp đồng</option>`;

        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);
            var data = rs.data;
            var dataLength = data.length;

            for (var i = 0; i < dataLength; i++) {
                optionHtml += `<option value="${data[i].ID}">${data[i].Name}</option>`;
            }

        } else {
            $("#loader").hide();
            console.log(rs);
            alertError(rs.messVN);
        }

        $("#contractType").html(optionHtml);

        $("#contractType").selectize({
            create: true
        });
    });
}


/*Get contract customers*/
function getContractCustomers() {
    var settings = {
        "url": api_url + "customers?isGetAll=true",
        "method": "GET",
        "timeout": 0,
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        var optionHtml = "";
        optionHtml += `<option value="0">Chọn bên ký kết</option>`;

        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);
            var data = rs.data;
            var dataLength = data.length;

            for (var i = 0; i < dataLength; i++) {
                optionHtml += `<option value="${data[i].ID}">${data[i].FullName}</option>`;
            }
        } else {
            $("#loader").hide();
            console.log(rs);
            alertError(rs.messVN);
        }

        $("#contractCustomer").html(optionHtml);

        $("#contractCustomer").selectize({
            create: true
        });
    });
}



/*Reject contract*/
function openModalReject(id) {
    if (id) {
        $("#rejectID").val(id);
    }

    $("#modalReject").modal("show");
}


function rejectContract() {
    swal({
        title: "Thông báo",
        text: "Bạn muốn từ chối ký hợp đồng này?",
        icon: "warning",
        buttons: true,
        buttons: ["Hủy", "Đồng ý"],
        dangerMode: true,
    })
        .then((doit) => {
            if (doit) {
                var id = $("#rejectID").val();
                var rejectNote = $("#txtReject").val();

                var settings = {
                    "url": api_url + "reject-contract",
                    "method": "POST",
                    "timeout": 0,
                    "headers": {
                        "Content-Type": "application/json"
                    },
                    "data": JSON.stringify({
                        "ID": id,
                        "RejectNote": rejectNote
                    }),
                };

                $("#loader").show();

                $.ajax(settings).done(function (rs) {
                    if (rs.code == 200) {
                        console.log(rs);
                        $("#loader").hide();
                        alertSuccess(rs.messVN);
                        $("#modalReject").modal("hide");
                        signContract();
                        onLoad(1);
                    } else {
                        console.log(rs);
                        $("#loader").hide();
                        alertError(rs.messVN);
                    }
                });
            }
        });
}


/*Download file*/
function downloadFile() {
    swal({
        title: "Thông báo",
        text: "Bạn tải tệp xuống?",
        icon: "warning",
        buttons: true,
        buttons: ["Hủy", "Đồng ý"],
        dangerMode: true,
    })
        .then((doit) => {
            if (doit) {
                var dataDownload = [];

                $('.ckContract').each(function () {
                    if ($(this).prop('checked') == true) {
                        var id = $(this).val().split('&')[1];

                        dataDownload.push(id);
                    }
                });

                if (dataDownload.length > 0) {
                    for (var i = 0; i < dataDownload.length; i++) {
                        var settings = {
                            "url": api_url + "file/" + dataDownload[i],
                            "method": "GET",
                            "timeout": 0,
                        };

                        $("#loader").show();

                        $.ajax(settings).done(function (rs) {
                            if (rs.code == 200) {
                                $("#loader").hide();
                                console.log(rs);
                                var data = rs.data;

                                var linkSource = `data:application/pdf;base64, ${data.FileBase64}`;

                                $("#downloadFile").attr("href", linkSource);

                                document.getElementById("downloadFile").click();
                            } else {
                                $("#loader").hide();
                                console.log(rs);
                                alertError(rs.messVN);
                            }
                        });
                    }
                } else {
                    alertWarning("Vui lòng chọn tệp muốn tải về");
                }
            }
        });
}