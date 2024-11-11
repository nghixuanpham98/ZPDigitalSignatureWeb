var user = JSON.parse(storageGetItem("localStorageList"));
var guidEmpty = "00000000-0000-0000-0000-000000000000";


$(document).ready(function () {
    if (user != null) {
        $("#loader").hide();
        var userName = user.data.DisplayName;
        var soldCode = user.data.SoldCode;
        $("#txtUserChange").val(soldCode);
        $(".nav-user-name").text(userName);

        checkNewNotification();

        setInterval(function () {
            checkNewNotification();
        }, 5000);
    }
});


/*Check new Notification*/
function checkNewNotification() {
    var settings = {};
    var role = user.data.Role;
    var userID = user.data.CustomerID;

    if (role == 2) {
        settings = {
            "url": api_url + "contract-news?id=" + userID + "&isCus=true",
            "method": "GET",
            "timeout": 0,
        };
    } else {
        settings = {
            "url": api_url + "contract-news?id=" + guidEmpty + "&isCus=false",
            "method": "GET",
            "timeout": 0,
        };
    }

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            var html = "";
            $(".totalNews").text(rs.totalData);

            if (rs.totalData > 0) {
                for (var i = 0; i < rs.totalData; i++) {
                    let timestampNow = Math.floor(Date.now() / 1000);
                    var timeContract = Math.floor(new Date(rs.data[i].ModifiedOn) / 1000);
                    var timeResult = timeSince(timestampNow - timeContract);

                    if (rs.data[i].Status == 0) {
                        html += `<a onclick="onLoadPage('contracts', 0, \`` + rs.data[i].ID + `\`, \`` + rs.data[i].FileContractID + `\`, \`` + rs.data[i].ContractName + `\`, 0)" href="javascript:void(0);" class="dropdown-item py-3">`;
                        html += `<small class="float-end text-muted ps-2">${timeResult}</small>`;
                        html += `<div class="media">`;
                        html += `<div class="avatar-md bg-soft-primary">`;
                        html += `<img src="assets/images/ivg.png" class="w-100 h-100" />`
                        html += `</div>`;
                        html += `<div class="media-body align-self-center ms-2 text-truncate">`;
                        html += `<h6 class="my-0 fw-normal text-dark">${rs.data[i].ContractName}</h6>`;
                        html += `<small class="color-status__blue text-muted mb-0">Nhấn vào để phê duyệt</small>`;
                        html += `</div></div></a>`;
                    } else if (rs.data[i].Status == 1) {
                        html += `<a onclick="onLoadPage('contracts', 0, \`` + rs.data[i].ID + `\`, \`` + rs.data[i].FileContractID + `\`, \`` + rs.data[i].ContractName + `\`, 1)" href="javascript:void(0);" class="dropdown-item py-3">`;
                        html += `<small class="float-end text-muted ps-2">${timeResult}</small>`;
                        html += `<div class="media">`;
                        html += `<div class="avatar-md bg-soft-primary">`;
                        html += `<img src="assets/images/ivg.png" class="w-100 h-100" />`
                        html += `</div>`;
                        html += `<div class="media-body align-self-center ms-2 text-truncate">`;
                        html += `<h6 class="my-0 fw-normal text-dark">${rs.data[i].ContractName}</h6>`;
                        html += `<small class="color-status__green text-muted mb-0">Đã ký phê duyệt</small>`;
                        html += `</div></div></a>`;
                    } else if (rs.data[i].Status == 3) {
                        html += `<a onclick="onLoadPage('contracts', 0, \`` + rs.data[i].ID + `\`, \`` + rs.data[i].FileContractID + `\`, \`` + rs.data[i].ContractName + `\`, 3)" href="javascript:void(0);" class="dropdown-item py-3">`;
                        html += `<small class="float-end text-muted ps-2">${timeResult}</small>`;
                        html += `<div class="media">`;
                        html += `<div class="avatar-md bg-soft-primary">`;
                        html += `<img src="assets/images/ivg.png" class="w-100 h-100" />`
                        html += `</div>`;
                        html += `<div class="media-body align-self-center ms-2 text-truncate">`;
                        html += `<h6 class="my-0 fw-normal text-dark">${rs.data[i].ContractName}</h6>`;
                        html += `<small class="color-status__red text-muted mb-0">Từ chối ký phê duyệt</small>`;
                        html += `</div></div></a>`;
                    }
                }
            } else {
                html += `<div class="text-center p-2">Không có thông báo mới</div>`;
            }
        }

        $(".notification-menu").html(html);
    });
}


/*Change Password*/
function changePW() {
    var soldCode = $("#txtUserChange").val();
    if (soldCode == null || soldCode === "" || soldCode === "undefined") {
        alertWarning("Vui lòng nhập Sold code");
        return;
    }

    var passwordPresent = $("#txtPWPresent").val();
    if (soldCode == null || soldCode === "" || soldCode === "undefined") {
        alertWarning("Vui lòng nhập mật khẩu hiện tại");
        return;
    }

    var password = $("#txtPWNew").val();
    if (password == null || password === "" || password === "undefined") {
        alertWarning("Vui lòng nhập mật khẩu mới");
        return;
    }

    var passwordAgain = $("#txtPWNewAgain").val();
    if (passwordAgain == null || passwordAgain === "" || passwordAgain === "undefined") {
        alertWarning("Vui lòng nhập lại Password");
        return;
    }

    if (password != passwordAgain) {
        alertWarning("Mật khẩu nhập lại không đúng vui lòng kiểm tra lại");
        return;
    }

    var settings = {
        "url": api_url + "change-password",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "SoldCode": soldCode,
            "Password": passwordPresent,
            "PasswordNew": passwordAgain
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);
            alertSuccess(rs.messVN);
            $("#txtPWPresent").val("");
            $("#txtPWNew").val("");
            $("#txtPWNewAgain").val("");
            $("#modalChangePassword").modal("hide");
        } else {
            $("#loader").hide();
            alertError(rs.messVN);
            console.log(rs);
        }
    });
}