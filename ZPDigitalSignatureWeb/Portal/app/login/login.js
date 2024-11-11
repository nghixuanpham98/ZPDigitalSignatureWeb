
$(document).ready(function () {
    $("#loader").hide();
});

function registerAcc() {
    var soldCode = $("#txtUserNew").val();
    if (soldCode == null || soldCode === "" || soldCode === "undefined") {
        alertWarning("Vui lòng nhập Sold code");
        return;
    }

    var password = $("#txtPWNew").val();
    if (password == null || password === "" || password === "undefined") {
        alertWarning("Vui lòng nhập Password");
        return;
    }

    var passwordAgain = $("#txtPWNewAgain").val();
    if (passwordAgain == null || passwordAgain === "" || passwordAgain === "undefined") {
        alertWarning("Vui lòng nhập lại Password");
        return;
    }

    if (password != passwordAgain) {
        alertWarning("Password nhập lại không đúng vui lòng kiểm tra lại");
        return;
    }

    var settings = {
        "url": api_url + "register",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "SoldCode": soldCode,
            "Password": password
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);
            alertSuccess(rs.messVN);
            $("#modalRegister").modal("hide");
        } else {
            $("#loader").hide();
            alertError(rs.messVN);
            console.log(rs);
        }
    });
}

function loginAcc() {
    var soldCode = $("#txtUser").val();
    if (soldCode == null || soldCode === "" || soldCode === "undefined") {
        alertWarning("Vui lòng nhập Sold code");
        return;
    }

    var password = $("#txtPassword").val();
    if (password == null || password === "" || password === "undefined") {
        alertWarning("Vui lòng nhập Password");
        return;
    }

    var settings = {
        "url": api_url + "login",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "SoldCode": soldCode,
            "Password": password
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            $("#loader").hide();
            console.log(rs);

            window.localStorage.setItem("localStorageList", JSON.stringify(rs));

            var full = window.location.protocol
                + '//'
                + window.location.hostname
                + (window.location.port ? ':' + window.location.port : '');

            var first = $(location).attr('pathname');
            first.indexOf(1);
            first.toLowerCase();
            first = first.split("/")[1];

            parent.window.location.href = full + "/" + first;
        } else {
            $("#loader").hide();
            alertError(rs.messVN);
            console.log(rs);
        }
    });
}


/*Alert*/
function alertError(message) {
    try {
        toastr.error("", message, { timeOut: 3000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}

function alertSuccess(message) {
    try {
        toastr.success("", message, { timeOut: 3000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}

function alertSuccessSpecial(message) {
    try {
        toastr.success("", message, { timeOut: 5000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}

function alertWarning(message) {
    try {
        toastr.warning("", message, { timeOut: 3000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}