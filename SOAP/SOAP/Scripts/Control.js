﻿var PatientInfo = new Object();

var UserInformation = new Object();

$(document).ready(function () {

    // tooltip does not work for <option>
    // $(document).tooltip();

    //Hide all tab content that are not active
    $('#thumbs a.not-active').each(function () {
        var tabContent = $(this).attr('nav');
        $('#' + tabContent).css('display', 'none');
    });

    // When a link is clicked
    $("#thumbs a").click(function () {
        if (!$(this).hasClass('active') && !$(this).hasClass('disabled')) {
            // switch previous tabs off
            var prevContent = $('a.active').attr('nav');
            $("a.active").removeClass("active");
            // switch this tab on
            $(this).addClass("active");
            var currentContent = $('a.active').attr('nav');
            $('#' + prevContent).hide('slide', function () {
                $('#' + currentContent).show('slide');
            });
        }
    });

    //When an input loses focus, grab information
    $("input").blur(function () {
        $this = $(this);
        addValue($this.attr('name'), $this.attr('id'), $this.val());
    });

    //Button to open Edit Profile drop down
    $("#edit-profile-button").click(function () {
        $("#profile-menu").slideToggle("slow")
        $("#edit-profile-button").toggleClass("menu-open");
    });

    //When register is clicked, register inputs are shown
    $("#register").click(function () {
        if (!$("#register-div").is(":visible")) {
            $("#register-div").slideDown('slow');
        }
        else {
            //Register user
            if (registerUser()) {
                alert('success');
                $("#register-div").slideUp('slow');
            }
            else {
                alert('Register User Failed');
            }
        }
    });

    //Shows saved forms after login
    $("#login").click(function () {

        //Validate user
        if (validateUser()) {

        $("#thumbs a.disabled").show("drop");
            $("#thumbs a.disabled").removeClass("disabled");
            $("#login-div").slideUp(function () {
                $("#saved-forms-div").slideDown();
            });
        }
        else {
            alert('Validate User Failed');
        }
    });
});

function getValue() { }
function addValue() { }
function validateUser() {
    var returned = false;
    var member = $("#username").val();
    var pw = $("#password").val();
    if (member && pw) {
        var memberInfo = {
            Username: member,
            Password: pw
        };
        $.ajax({
            type: 'Post',
            dataType: 'json',
            url: rootDir + '/Home/DoLogin',
            data: JSON.stringify(memberInfo),
            contentType: 'application/json; charset=utf-8',
            async: false,
            success: function (data) {
                if (data.success) {
                    returned = true;
                }
                else {
                    returned = false;
                }
            },
            error: function (data) {
                returned = false;
            }
        });
    }
    else {
        returned = false;
    }
    return returned;
}

function registerUser() {
    var returned = false;
    var pw1 = $("#password").val();
    var pw2 = $("#password-repeat").val();
    var userName = $("#username").val();
    if (pw1 == pw2 && pw1 && userName) {
        var ASFUser1 = {
            "Username": userName,
            "FullName": 'Needs Implemented',
            "EmailAddress": $("#email").val(),
            "Member": {
                "Username": userName,
                "Password": pw1
            }
        };
        $.ajax({
            type: 'Post',
            dataType: 'json',
            url: rootDir + '/Home/RegisterUser',
            data: JSON.stringify(ASFUser1),
            contentType: 'application/json; charset=utf-8',
            async: false,
            success: function (data) {
                if (data.success) {
                    returned = true;
                }
                else {
                    returned = false;
                }
            },
            error: function (data) {
                returned = false;
            }
        });
    }
    else {
        returned = false;
    }
    return returned;
}