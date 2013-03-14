var PatientInfo = new Object();

var UserInformation = new Object();

var DropdownCategories = new Object();

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

    $("#edit-profile-submit").click(function () {
        var foobarredUser = UserInformation;
        foobarredUser.FullName = $("#Patient\\.Profile\\.FullName").val();
        foobarredUser.EmailAddress = $("#Patient\\.Profile\\.Email").val();
        ajax('Post', '', JSON.stringify(foobarredUser), true)
        .done(function (data) {
            if (data.success) {
                UserInformation.FullName = foobarredUser.FullName;
                UserInformation.EmailAddress = foobarredUser.EmailAddress;
                $("#profile-menu").slideToggle("slow")
                $("#edit-profile-button").toggleClass("menu-close");
                alert('User Info Updated');
            }
            else {
                alert('Error updating User Info');
            }
        })
        .fail(function (data) {
            alert('Error updating User Info');
        });
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

    if (sessionStorage.username && sessionStorage.password) {
        login(sessionStorage.username, sessionStorage.password);
    }
    else {
        $("#login").click(function () {
            var username = $.trim($("#username").val());
            var password = $("#password").val();
            var passwordHash = password.hashCode();
            login(username, passwordHash);
        });
    }

    $("#dropdownCat").change(function () {
        var idOfCat = $(this).val();
        PopulateAdminPropertyValues(idOfCat);
    });
});

function login(username, password) {
    //Validate user
    if (validateUser(username, password)) {
        //Shows saved forms after login
        $("#user-info a.edit-profile").show("slide");
        $("#thumbs a.disabled").show("drop");
        $("#thumbs a.disabled").removeClass("disabled");
        $("#user-info a.edit-profile").removeClass("disabled");
        $("#login-div").slideUp(function () {
            $("#saved-forms-div").slideDown();
        });
        if (!UserInformation.IsAdmin) {
            $("#thumbs a.admin").addClass("disabled");
        }
        sessionStorage.username = username;
        sessionStorage.password = password;
        DropdownCategories = GetAllDropdownCategories();
    }
    else {
        alert('Validate User Failed');
    }
}

function setProfileInfo() {
    $("#Patient\\.Profile\\.FullName").val(UserInformation.FullName);
    $("#Patient\\.Profile\\.Email").val(UserInformation.EmailAddress);
}

function GetAllDropdownCategories() {
    var dCats;
    ajax('Post', '/Home/GetAllDropdownCategories', '', false)
    .done(function (data) {
        if (data.success) {
            dCats = data.DropdownCategories;
        }
        else {
        }
    })
    .fail(function (data) {

    });
    return dCats;
}

function PopulateAdminCategories() {
    var cat = $('#dropdownCat');
    cat.append('<option value="0"> - Select One - </option>');
    for (var i = 0; i < DropdownCategories.length; i++) {
        var e = '<option value="' + DropdownCategories[i].Id + '">' + DropdownCategories[i].ShortName + '</option>';
        cat.append(e);
    }
}

function PopulateAdminPropertyValues(idOfCat) {
    if (idOfCat != 0) {
        var obj = { Id: idOfCat };
        ajax('Post', '/Home/GetDropdownValues', JSON.stringify(obj), false)
        .done(function (data) {
        })
        .fail(function (data) {

        });
    }
}

function getValue() { }
function addValue() { }

function validateUser(member, password) {
    var returned = false;
    if (member && password) {
        var memberInfo = {
            Username: member,
            Password: password
        }
        ajax('Post', '/Home/DoLogin', JSON.stringify(memberInfo), false)
        .fail(function (data) {
            returned = false;
        })
        .done(function (data) {
            if (data.success) {
                var ReturnUser = data.returnUser;
                UserInformation = JSON.parse(ReturnUser);
                setProfileInfo();
                returned = true;
                if (user.IsAdmin) {
                    UserInformation.IsAdmin = true;
                }
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
    var userName = $.trim($("#username").val());
    if (pw1 == pw2 && pw1 && userName) {
        var fullName = $("#full-name").val();
        var pwHash = pw1.hashCode();
        var ASFUser1 = {
            "Username": userName,
            "FullName": fullName,
            "EmailAddress": $("#email").val(),
            "Member": {
                "Username": userName,
                "Password": pwHash
            }
        };
        ajax('Post', '/Home/RegisterUser', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success) {
                returned = true;
                $("#password").val("");
                $("#password-repeat").val("");
                $("#email").val("");
                $("#full-name").val("");
                sessionStorage.username = userName;
                sessionStorage.password = pwHash;
            }
            else {
                returned = false;
            }
        })
        .fail(function (data) {
            returned = false;
        });
    }
    else {
        returned = false;
    }
    return returned;
}


function deleteUser(users) {


}

function promoteUser(users) {


}

function ajax(typeIn, urlIn, dataIn, asyncIn) {

    return $.ajax({
        type: typeIn,
        dataType: 'json',
        url: rootDir + urlIn,
        data: dataIn,
        contentType: 'application/json; charset=utf-8',
        async: asyncIn
    });
}

String.prototype.hashCode = function(){
	var hash = 0;
	if (this.length == 0) return hash;
	for (i = 0; i < this.length; i++) {
		char = this.charCodeAt(i);
		hash = ((hash<<5)-hash)+char;
		hash = hash & hash; // Convert to 32bit integer
	}
	return hash.toString();
}
