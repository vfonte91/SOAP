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
        //addValue($this.attr('name'), $this.attr('id'), $this.val());
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
            var result = registerUser()
            if (result == "success") {
                alert('Registration was succesful');
                $("#register-div").slideUp('slow');
            }
            else {
                alert(result);
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

function OpenForm(formId) {
    var pat = { PatientId: formId };
    ajax('Post', '/Home/GetPatient', JSON.stringify(pat), false)
    .done(function (data) {
        if (data.success) {
            console.log(data);
        }
        else {
        }
    })
    .fail(function (data) {

    });
}

function GetUserForms() {
    ajax('Post', '/Home/GetUserForms', JSON.stringify(UserInformation), false)
    .done(function (data) {
        if (data.success) {
            var forms = $('#saved-forms');
            forms.append('<option value="0"> - Select One - </option>');
            for (var i = 0; i < data.Forms.length; i++) {
                var date = new Date(parseInt(data.Forms[i].PatientInfo.DateSeenOn.substr(6)));
                //date = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear() + ', ' + date.getHours() + ':' + date.getMinutes();
                var e = '<option value="' + data.Forms[i].PatientId + '">' + date.toLocaleString() + '</option>';
                forms.append(e);
            }
        }
        else {
        }
    })
    .fail(function (data) {

    });
}

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

        DropdownCategories = GetAllDropdownCategories();

        //Hides Admin tab if not admin

        if (!UserInformation.IsAdmin) {
            $("#thumbs a.admin").addClass("disabled");
        }
        else {
            getUsers();
            PopulateAdminCategories();
        }
        //Stores username and password
        sessionStorage.username = username;
        sessionStorage.password = password;

        populateAll();
        GetUserForms();
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
    $("#forgot-password").dialog({

        width: 600,
        height: 400,
        modal: true,
        draggable: false,
        buttons: [ { text: "Ok", click: forgetClicked } ],
        open: function (event, ui) {
            var textarea = $('<textarea style="height: 276px;">');
            // getter


            // getter


            //$(this).html(textarea);

            //$(textarea).redactor({ autoresize: false });

        }
    });
}

function forgetClicked() {
    var forgotUser = $("#usernameForgot").val();
    var emailForgot = $("#emailForgot").val();
    var ASFUser1 = {
        Username: forgotUser,
        EmailAddress: emailForgot
    };
    ajax('Post', '/Home/CheckForgotPassword', JSON.stringify(ASFUser1), false)
    .done(function (data) {

        if (data.success) {
         $("#forgot-password").dialog("close");
               $("#change-password").dialog({
                    width:600,
                    height: 400,
                    modal: true,
                    draggable: false,
                    buttons:[{text: "Submit", click: function(){
                            ChangePassword(ASFUser1);
                        }
                        }],
                });
        }
        else {
        }
    })
    .fail(function (data) {

    });
}

function ChangePassword(user) {
    var pw1 = $("#newPassword").val();
    var pw2 = $("#newPasswordAgain").val();
    if (pw1 == pw2) {
        ASFUser1 = {
            Username: user.Username,
            Member: {
                Username: user.Username,
                Password: pw1.hashCode()
            }
        };
        ajax('Post', '/Home/ChangeForgottenPassword', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success) {
                $("#change-password").dialog("close");
            }
            else {
            }
        })
        .fail(function (data) {

        });
    }
    else {
        alert('Passwords do not match');
    }
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
            if (data.success) {
                var values = data.DropdownValues;
                $("#dropdown-body").empty();

                for (var i = 0; i < values.length; i++) {
                    var id = values[i].Id;
                    var label = values[i].Label;
                    var desc = values[i].Description;
                    var labelInput = "<input type='text' id='" + id + "-label' value='" + label + "'/>";
                    var descInput = "<input type='text' id='" + id + "-desc' value='" + desc + "'/>";
                    var deleteButton = "<input type='button' class='submit' onclick='removeDropdownValue(" + id + ")' value='Delete'/>";
                    var editButton = "<input type='button' class='submit' onclick='editDropdownValue(" + id + ", $('#" + id + "-label').val(), $('#" + id + "-desc').val())' value='Edit'/>";
                    var row = "<tr><td>" + labelInput + "</td><td>" + descInput + "</td><td>" + deleteButton + "</td><td>" + editButton + "</td></tr>";
                    $("#dropdown-body").append(row);
                }
            }
            else
                alert("Clould not get drop down values");
        })
        .fail(function (data) {
            alert("Clould not get drop down values");
        });
    }
}

function editDropdownValue(id, label, desc) {
}

function deleteDropdownValue(id) {
}

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
    var fullName = $("#full-name").val();
    var email = $("#email").val()

    if (!userName) {
        returned = "Must enter username";
    }
    else if (pw1 != pw2 || !pw1) {
        returned = "Passwords do not match";
    }
    else if (!fullName) {
        returned = "Must enter full name";
    }
    else if (!email) {
        returned = "Must enter email address";
    }
    else {
        var pwHash = pw1.hashCode();
        var ASFUser1 = {
            "Username": userName,
            "FullName": fullName,
            "EmailAddress": email,
            "Member": {
                "Username": userName,
                "Password": pwHash
            }
        };
        ajax('Post', '/Home/RegisterUser', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success) {
                returned = "success";
                $("#password").val("");
                $("#password-repeat").val("");
                $("#email").val("");
                $("#full-name").val("");
                sessionStorage.username = userName;
                sessionStorage.password = pwHash;
            }
            else {
                returned = "User could not be registered";
            }
        })
        .fail(function (data) {
            returned = "User could not be registered";
        });
    }
    return returned;
}

function getUsers() {
    var users;

    $("#users").empty();

    ajax('Post', '/Home/GetUsers', '', false)
    .done(function (data) {
        if (data.succes) {
            users = data.users;

            for (var i = 0; i < users.length; i++) {
                var name = users[i].FullName;
                var username = users[i].Username;
                $("#users").append("<option value='" + username + "'>" + name + "</option>");
            }
        }
        else
            returned = false;
    })
    .fail(function (data) {
        returned = false;
    });
}

function deleteUser(users) {
    var returned = '';
    for (var i = 0; i < users.length; i++) {
        var ASFUser1 = {
            "Username": users[i]
        }
        ajax('Post', '/Home/DeleteUser', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success)
                returned += users[i] + " deleted. ";
            else
                returned += "Error: " + users[i] + " could not be deleted. ";
        })
        .fail(function (data) {
            returned += "Error: " + users[i] + " could not be deleted. ";
        });
    }
    getUsers();
    alert(returned);
}

function promoteUser(users) {
    var returned = '';
    for (var i = 0; i < users.length; i++) {
        var ASFUser1 = {
            "Username": users[i]
        }
        ajax('Post', '/Home/PromoteUser', JSON.stringify(ASFUser1), false)
        .done(function (data) {
            if (data.success) 
                returned += users[i] + " promoted. ";
            else
                returned += "Error: " + users[i] + " could not be promoted. ";
        })
        .fail(function (data) {
            returned += "Error: " + users[i] + " could not be promoted. ";
        });
    }
    alert(returned);
}

//Logs user out of session
function logOut() {
    //Clears session storage
    sessionStorage.clear();

    //Reloads page
    location.reload();
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
