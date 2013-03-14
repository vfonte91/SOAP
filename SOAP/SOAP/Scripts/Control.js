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
        ajax('Post', '/Home/EditProfile', JSON.stringify(foobarredUser), true)
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
    //Shows saved forms after login
    $("#login").click(function () {

        //Validate user
        if (validateUser()) {
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
            DropdownCategories = GetAllDropdownCategories();
            populateAll();
        }
        else {
            alert('Validate User Failed');
        }
    });

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
        PopulateAdminCategories();
        populateAll();
    }
    else {
        alert('Validate User Failed');
    }
}



function setProfileInfo() {
    $("#Patient\\.Profile\\.FullName").val(UserInformation.FullName);
    $("#Patient\\.Profile\\.Email").val(UserInformation.EmailAddress);
}

function populateAll() {
    populate(1, "Patient.PatientInfo.Procedure");
    populate(2, "Patient.PatientInfo.Temperament");
    populate(4, "Patient.PatientInfo.PreoperativePainAssesment");
    populate(4, "Patient.PatientInfo.PostperativePainAssesment");
    populate(5, "Patient.ClinicalFindings.CardiacAuscultation");
    populate(6, "Patient.ClinicalFindings.PulseQuality");
    populate(20, "Patient.ClinicalFindings.CapillaryRefillTime");
    populate(7, "Patient.ClinicalFindings.RespiratoryAuscultationId");
    populate(9, "Patient.ClinicalFindings.PhysicalStatusClass");
    populate(25, "Patient.ClinicalFindings.MucousMembraneColor");
    populate(13, "Patient.AnestheticPlanPremedication.Route");
    populate(21, "Patient.AnestheticPlanPremedication.SedativeDrug");
    populate(22, "Patient.AnestheticPlanPremedication.OpioidDrug");
    populate(23, "Patient.AnestheticPlanPremedication.AnticholinergicDrug");
    populate(14, "Patient.AnestheticPlanInjection.Drug");
    populate(13, "Patient.AnestheticPlanInjection.Route");
    populate(24, "Patient.AnestheticPlanInjection.IVFluidTypes");
    populate(15, "Patient.AnestheticPlanInhalant.Drug");
    populate(14, "Patient.MaintenanceInjectionDrug.Drug");
    populate(13, "Patient.MaintenanceInjectionDrug.RouteOfAdministration");
    populate(15, "Patient.MaintenanceInhalentDrug.Drug");
    populate(16, "Patient.MaintenanceInhalentDrug.BreathingSystem");
    populate(17, "Patient.MaintenanceInhalentDrug.BreathingBagSize");
    populate(18, "Patient.OtherAnestheticDrug.IntraoperativeAnalgesia");

}

function populate(id, name) {
   
    var num = parseInt(id);
    num = num - 1;
    var cats = DropdownCategories;
    var values = cats[num].DropdownValues;
    for (var i = 0; i < values.length; i++) {
        var x = document.getElementById(name);
        var option = document.createElement("option");
        option.text = values[i].Label;
        x.add(option, null);
    }
}

function forgotPass() {
    $("#forgotPass").dialog({

        width: 600,
        height: 400,
        modal: true,
        draggable: false,
        buttons: [ { text: "Ok", click: function() { $( this ).dialog( "close" ); } } ],
        open: function (event, ui) {
            var textarea = $('<textarea style="height: 276px;">');
            // getter


            // getter


            //$(this).html(textarea);

            //$(textarea).redactor({ autoresize: false });
            //$(textarea).setCode('<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>');
        }
    });
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

function getUsers() {}

function deleteUser(users) {

    for (var i = 0; i < users.length; i++) {
        var ASFUser1 = {
            "Username": users[i]
        }
        ajax('Post', '/Home/DeleteUser', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success) {
                returned = true;
                getUsers();
            }
            else
                returned = false;
        })
        .fail(function (data) {
            returned = false;
        });
    }
    return returned;
}

function promoteUser(users) {

    for (var i = 0; i < users.length; i++) {
        var ASFUser1 = {
            "Username": users[i]
        }
        ajax('Post', '/Home/PromoteUser', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success) {
                returned = true;
            }
            else
                returned = false;
        })
        .fail(function (data) {
            returned = false;
        });
    }
    return returned;
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
