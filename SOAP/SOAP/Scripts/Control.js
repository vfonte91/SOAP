var PatientInfo = new Object();

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
            }
            else {
                alert('failure');
            }
        }
    });

    //Shows saved forms after login
    $("#login").click(function () {

        //Validate user
        validateUser();

        $("#thumbs a.disabled").removeClass("disabled");
        $("#login-div").slideUp(function () {
            $("#saved-forms-div").slideDown();
        });
    });

    $("#dropdownCat").change(function () {
        var values = getValue($(this).val());
    });
});

function getValue() { }
function addValue() { }
function validateUser() {
    var ASFUser = {
        MembershipInfo: {
            Username: $("#username").val(),
            Password: $("#password").val()
        }
    };
    $.post(rootDir + '/Home/DoLogin', ASFUser, function (data) {
        UserInformation = JSON.parse(data);
        if (UserInformation.success)
            return true;
    })
    .error(function() {
        return false;
    });
    return false;
}

function registerUser() {
    var pw1 = $("#password").val();
    var pw2 = $("#password-repeat").val();
    if (pw1 == pw2) {
        var userName = $("#username").val()
        var ASFUser = {
            Username: userName,
            FullName: 'Needs Implemented',
            EmailAddress: $("#email").val(),
            MembershipInfo: {
                Username: userName,
                Password: pw1
            }
        };
        console.log(ASFUser);
        $.post(rootDir + '/Home/RegisterUser', ASFUser, function (data) {
            console.log(returnVal) = JSON.parse(data);
            if (returnVal.success)
                return true;
        }, 'json')
        .error(function () {
            return false;
        });
    }
    return false;
}
