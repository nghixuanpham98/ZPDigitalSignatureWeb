
$(document).ready(function () {
    $("#loader").hide();
    getContractTypes();
    getContractCustomers();
});


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


/*Upload file Contract*/
function uploadFile() {
    var maxOfFile = 10000000;
    var filesSelected = document.getElementById("txtFile").files;

    if (filesSelected != null && filesSelected.length > 0) {
        var fileToLoad = filesSelected[0];

        if (fileToLoad.size >= maxOfFile) {
            alertWarning("Dung lượng tệp không được vượt quá 10MB");
            return;
        }

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

                    $("#fileID").val(rs.fileID);
                } else {
                    $("#loader").hide();
                    console.log(rs);
                    alertError(rs.messVN);
                }
            });
        }

        fileReader.readAsDataURL(fileToLoad);
    }
}


/*Create contract info*/
function createContractInfo() {
    var fileID = $("#fileID").val();
    if (fileID == null || fileID === "" || fileID === "undefined") {
        alertWarning("Vui lòng chọn tệp *.pdf để lập hợp đồng");
        return;
    }

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

    var contractNote = $("#contractNote").val();

    var contractStatus = $('input[name="ckContractStatus"]:checked').val();

    var settings = {
        "url": api_url + "create-contract",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "CustomerID": customerID,
            "FileContractID": fileID,
            "ContractTypeID": contractTypeID,
            "ContractName": contractName,
            "Status": contractStatus,
            "Note": contractNote
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);
            alertSuccess(rs.messVN);

            /*Clear field*/
            $("#txtFile").val("");
            $("#contractName").val("");
            $("#contractNote").val("");
            $("#ckContractStatus1").prop("checked", true);
            $('#contractType').data('selectize').setValue(0);
            $('#contractCustomer').data('selectize').setValue(0);

            onLoadPage('contracts', 1);
        } else {
            $("#loader").hide();
            console.log(rs);
            alertError(rs.messVN);
        }
    });
}