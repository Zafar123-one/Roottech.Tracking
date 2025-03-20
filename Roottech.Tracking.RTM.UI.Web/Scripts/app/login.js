function login() {
    var showLoadingImage = false;
    if ($("#UserId").val() === '' &&  $("#Password").val() === '') {
         $("div.error span").html('Please enter valid user id & password');
                        $("div.error").show();
    }
    else $.ajax({
        url: url+"?userId=" + $("#UserId").val() + "&password=" + $("#Password").val(),
        cache: false,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        beforeSend: function() {
            $("#buttonsrow").hide();
            $("#loadingrow").show();
        },
        complete: function() {
            if (!showLoadingImage) {
                $("#loadingrow").hide();
                $("#buttonsrow").show();
            }
        },
        success: function (data, textStatus, xhr) {
            if (textStatus === "success") {
                //var userName = data[0].User_Name.toString();
                //$("#lblWelcome").attr("class",userName);
                $("div.error span").html("");
                $("div.error").hide();
                if (data.Msg == null){// && data.GroupCount >= 0) {
                    showLoadingImage = true;
                    if (data.AppType === "All")
                        callFancyBox("#appselect", "Choose Type", 0, 0);
                    else
                        window.location.href = urlMap.replace("/Mobile/Map", "/" + data.AppType + "/Map");
                    return;
                }
                var msg = data.Msg.toString().split(';');
                var attemptType = parseInt(msg[0]);
                if (attemptType === 6) // First Login
                {
                    $("#UserName").val(data.User_Name.toString());
                    $("#LoginCode").val($("#UserId").val());
                    $("#Organization").val(data.OrgName);
                    getSecurityQuestion('#crudform',msg[1].toString(),754,450);
                } else {
                    if (msg[1] != null) {
                        $("div.error span").html(msg[1]);
                        $("div.error").show();
                    }
                }
            }
        } 
    });
}
/*
$("form#login").off("submit");
$("form#login").submit(function() {
    login();
    return false;
});*/

function goBack() {
    window.location.href = urlMainPage;
}

function goAboutus() {
    window.open (urlMore);
}

function getOrganization(appType) {
    $.ajax({
        url: urlOrg + "&appType=" + appType,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.length === 1) {
                var objNam = (appType == 1) ? "selOrganizationByUser" : "selOrganizationByUserForMobile";
                var input = $("<input>")
                    .attr("type", "hidden")
                    .attr("name", objNam).val(data[0].Id);//"selOrganizationByUser"
                $("#" + objNam).closest("form").append($(input));
                $("#" + objNam).closest("form").submit();
                return;
            }
            var html = "<option value=''>-- Select Organization --</option>";
            $.each(data, function (index, value) {
                html += "<option value='" + value.Id + "'>" + value.Description + "</option>";
            });

            /*var bindData = { org: data };
            var template = "{{#org}}<option value={{Id}}>{{DSCR}}</option>{{/org}}";
            var html = Mustache.to_html(template, bindData);*/
            if (appType == 1) {
                $('#selOrganizationByUser').html(html);
                $('#selOrganizationByUser').show();
                $('#selOrganizationByUserForMobile').hide();
                $("#orgsel").show();
            }
            else if (appType == 2) {
                $('#selOrganizationByUserForMobile').html(html);
                $('#selOrganizationByUserForMobile').show();
                $('#selOrganizationByUser').hide();
                $("#orgselMob").show();
            }
            $("#appsel").hide();
            //$("#btnGo").show();
            //$($("div#login-box-name")[2]).show();
        }
    });
}

  function getSecurityQuestion(href,title,width,height) {
    $.ajax({
        url :urlSecurityFAQs,
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function (data) {
            var html = "";
            $.each(data, function (index, value) {
                value = value.toString().replace(/_/g, " ");
                html += "<option value='" + value + "?'>" + value + "?</option>";
            });
            $(href + ' #ddlFAQ1').html(html);
            $(href + ' #ddlFAQ2').html(html);
            callFancyBox(href,title,width,height);
        }
    });
}

function firstLogin() {
    if ($("#crudform #LoginCode").val() === '' || $("#OldPass").val() === ''
        || $("#NewPass").val() === '' || $("#ConfrimPass").val() === ''
        || $("#crudform #ddlFAQ1").val() === '' || $("#crudform #Answer1").val() === '') {
        $("#crudform .error span").html("Login code, old password, new password, confirm passwrd, question and answer # 1 are required fields.");
        $("#crudform .error").show();
        return false;
    } else if ($("#NewPass").val() !== $("#ConfrimPass").val()) {
        $("#crudform .error span").html("Old password and new password should be equal.");
        $("#crudform .error").show();
        $("#ConfrimPass").focus();
        return false;
    }
    $.ajax({
        url: urlFirstLogin + "?userId=" + $("#UserId").val() + "&OldPassword=" + $("#OldPass").val() + "&NewPassword=" + $("#NewPass").val() + "&UserFAQ1=" + $("#crudform #ddlFAQ1").val() + "&UserAns1=" + $("#crudform #Answer1").val() + "&UserFAQ2=" + $("#crudform #ddlFAQ2").val() + "&UserAns2=" + $("#crudform #Answer2").val() + "&LoginCode=" + $("#crudform #LoginCode").val(),
         type: "GET",
         contentType: "application/json charset=utf-8",
         dataType: "json",
         success: function(data, textStatus, xhr) {
             if (textStatus == "success") {
                 $("#UserId").val($("#LoginCode").val());
                 $("#Password").val($("#NewPass").val());
                 $("form#login").submit();
                 $.fancybox.close();
                 return true;
             }
         }
     });
}

function getfpSecurityQuestion() {
    getSecurityQuestion('#forgotform', 'Forgot Password', 450, 250);
}

function callFancyBox(href,title,width,height) {
    $.fancybox({
        //padding: 5,
        href: href,
        title: title,
        openEffect: 'elastic',
        openSpeed: 250,
        openOpacity: true,
        closeEffect: 'elastic',
        closeSpeed: 150,
        closeOpacity: true,
        //width: width, // TODO parseInt($("div.tableContainer").css("width").replace("px", "")),
        //autoSize: false,
        //height: height,
        //autoCenter: true,
        type: 'inline',
        modal: true
        //,closeBtn: true
        //beforeShow: function () { viewModel.showForm(true); }
        //helpers: { overlay: null }
    });
}

function forgotPassword(href) {
    $.ajax({
        url: urlForgotPassword + "?Id=" + $(href + ' #LoginCode').val() + "&User_FAQ1=" + $(href + ' #ddlFAQ1').val() + "&User_Ans1=" + $(href + ' #Answer1').val() + "&User_FAQ2=" + $(href + ' #ddlFAQ2').val() + "&User_Ans2=" + $(href + ' #Answer2').val(),
        type: "GET",
        contentType: "application/json charset=utf-8",
        dataType: "json",
        success: function(data, textStatus, xhr) {
            if (textStatus == "success") {
                if (data.Id == 0) {
                    $("div.error span").html(data.Msg);
                    $("div.error").show();
                } else {
                    $("div.error span").html(data.Msg + "<br/>New Password:" + data.Id);
                    $("div.error").show();
                    $("#UserId").val($(href + ' #LoginCode').val());
                    $("#Password").val(data.Id);
                    $.fancybox.close();
                }

            }
        }
    });
}