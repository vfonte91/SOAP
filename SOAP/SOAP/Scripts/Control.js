﻿var PatientInfo = new Object();

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
        $.ajax({
            type: 'Post',
            dataType: 'json',
            url: rootDir + '/Home/EditProfile',
            data: JSON.stringify(foobarredUser),
            contentType: 'application/json; charset=utf-8',
            async: true,
            success: function (data) {
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
            },
            error: function (data) {
                alert('Error updating User Info');
            }
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
        var values = getValue($(this).val());
    });
});

function setProfileInfo() {
    $("#Patient\\.Profile\\.FullName").val(UserInformation.FullName);
    $("#Patient\\.Profile\\.Email").val(UserInformation.EmailAddress);
}

function populateAll() {
    populate(1, "Patient.PatientInfo.Procedure");
    populate(2, "Patient.PatientInfo.Temperament");
    populate(3, "Patient.PatientInfo.PreoperativePainAssesment");
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

function GetAllDropdownCategories() {
    var dCats;
    $.ajax({
        type: 'Post',
        dataType: 'json',
        url: rootDir + '/Home/GetAllDropdownCategories',
        contentType: 'application/json; charset=utf-8',
        async: false,
        success: function (data) {
            if (data.success) {
                dCats = data.DropdownCategories;
            }
            else {
            }
        },
        error: function (data) {
        }
    });
    return dCats;
}

function getValue() { }
function addValue() { }
function validateUser() {
    var returned = false;
    var member = $.trim($("#username").val());
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
                    UserInformation = JSON.parse(data.returnUser);
                    setProfileInfo();
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
    var userName = $.trim($("#username").val());
    if (pw1 == pw2 && pw1 && userName) {
        var fullName = $("#full-name").val();
        var ASFUser1 = {
            "Username": userName,
            "FullName": fullName,
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
                    $("#password").val("");
                    $("#password-repeat").val("");
                    $("#email").val("");
                    $("#full-name").val("");
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
