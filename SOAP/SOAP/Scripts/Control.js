var Patient = {
    PatientInfo: { Student: {}, Clinician: {} },
    ClinicalFindings: { PriorAnesthesia: {}, AnesthesiaConcerns: [] },
    Bloodwork: {},
    AnestheticPlan: { PreMedications: [], InjectionPlan: {}, InhalantPlan: {} },
    Maintenance: {},
    Monitoring: {}
}

var newPatient = true;

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
        addValue($this.closest('form'), $this.attr('name'), $this.val(), $this.attr('subgroup'));
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
        if (login(sessionStorage.username, sessionStorage.password)) {
            if (sessionStorage.formId) {
                OpenForm(sessionStorage.formId);
            }
        }
    }
    else {
        $("#login").click(function () {
            var username = $.trim($("#username").val());
            var password = $("#password").val();
            var passwordHash = password.hashCode();
            login(username, passwordHash);
        });
    }

    $("#Patient\\.PatientInfo\\.ProcedureDate").datepicker();
    $("#Patient\\.ClinicalFindings\\.Date").datepicker();
    $("#Patient\\.ClinicalFindings\\.AnesthesiaConcerns").multiselect({ header: "Anesthetic Concerns" });
    $("#Patient\\.ClinicalFindings\\.AnesthesiaConcerns").multiselect("uncheckAll");
    $("#Patient\\.Monitoring\\.Monitoring").multiselect({ header: "Monitoring" });
    $("#Patient\\.Monitoring\\.Monitoring").multiselect("uncheckAll");
});

function addValue(section, name, value, subgroup) {
    if (section && name) {
        section = section[0].name;
        if (section != "AnestheticPlan") {
            if (subgroup) {
                Patient[section][subgroup][name] = value
            }
            else {
                Patient[section][name] = value;
            }
        }
    }
}

function buildPatientInfo() {
    var procedure = $('#Patient\\.PatientInfo\\.Procedure').val();
    var otherProcedure = $('#Patient\\.PatientInfo\\.ProcedureOther').val();
    var bodyWeight = $('#Patient\\.PatientInfo\\.BodyWeight').val();
    var ageInMonths = $('#Patient\\.PatientInfo\\.AgeInMonths').val();
    var ageInYears = $('#Patient\\.PatientInfo\\.AgeInYears').val();
    var temperament = $('#Patient\\.PatientInfo\\.Temperament').val();
    var procedureDate = $('#Patient\\.PatientInfo\\.ProcedureDate').val();
    var preOp = $('#').val();
    var postOp = $('#').val();
}

function buildAnestheticPlanPremeds() {
    Patient.AnestheticPlan.PreMedications = [];
    var route = $("#Patient\\.AnestheticPlan\\.PreMedications\\.Route").val();
    var sedative = $("#Patient\\.AnestheticPlan\\.PreMedications\\.SedativeDrug").val();
    var sedativeDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.SedativeDosage").val();
    if (sedative || sedativeDosage) {
        var sedativeObj = { Drug: { Id: sedative }, Route: { Id: route }, Dosage: sedativeDosage };
        Patient.AnestheticPlan.PreMedications.push(sedativeObj);
    }

    var oploid = $("#Patient\\.AnestheticPlan\\.PreMedications\\.OpioidDrug").val();
    var oploidDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.OpioidDosage").val();
    if (oploid || oploidDosage) {
        var oploidObj = { Drug: { Id: oploid }, Route: { Id: route }, Dosage: oploidDosage };
        Patient.AnestheticPlan.PreMedications.push(oploidObj);
    }

    var antichol = $("#Patient\\.AnestheticPlan\\.PreMedications\\.AnticholinergicDrug").val();
    var anticholDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.AnticholinergicDosage").val();
    if (antichol || anticholDosage) {
        var anticholObj = { Drug: { Id: antichol }, Route: { Id: route }, Dosage: anticholDosage };
        Patient.AnestheticPlan.PreMedications.push(anticholObj);
    }

    var ketamineDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.KetamineDosage").val();
    if (ketamineDosage) {
        var ketamineObj = { Drug: { Id: ketamineEnum }, Route: { Id: route }, Dosage: ketamineDosage };
        Patient.AnestheticPlan.PreMedications.push(ketamineObj);
    }
}

function buildInduction() {
    if ($('#Injectable').is(':checked')) {
        Patient.AnestheticPlan.InhalantPlan = {};
        Patient.AnestheticPlan.InjectionPlan = { Drug: {}, Route: {}, IVFluidType: {} };
        Patient.AnestheticPlan.InjectionPlan.Drug.Id = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Drug').val();
        Patient.AnestheticPlan.InjectionPlan.Route.Id = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Route').val();
        Patient.AnestheticPlan.InjectionPlan.Dosage = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Dosage').val();
        Patient.AnestheticPlan.InjectionPlan.IVFluidType.Id = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.IVFluidType').val();
    }
    else {
        Patient.AnestheticPlan.InjectionPlan = {};
        Patient.AnestheticPlan.InhalantPlan = { Drug: {} };
        Patient.AnestheticPlan.InhalantPlan.Drug.Id = $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.Drug').val();
        Patient.AnestheticPlan.InhalantPlan.Percentage = $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.Percentage').val();
        Patient.AnestheticPlan.InhalantPlan.FlowRate = $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.FlowRate').val();
    }
}

function buildMaintenance() {
    if ($('#MaintenanceInject').is(':checked')) {
        Patient.Maintenance.MaintenanceInhalantDrug = {};
        Patient.Maintenance.MaintenanceInjectionDrug = { Drug: {}, RouteOfAdministration: {}, IntroaperativeAnalgesia: {} }
        Patient.Maintenance.MaintenanceInjectionDrug.Drug.Id = $('#Patient\\.MaintenanceInjectionDrug\\.Drug').val();
        Patient.Maintenance.MaintenanceInjectionDrug.RouteOfAdministration.Id = $('#Patient\\.MaintenanceInjectionDrug\\.RouteOfAdministration').val();
        Patient.Maintenance.MaintenanceInjectionDrug.Dosage = $('#Patient\\.MaintenanceInjectionDrug\\.Dosage').val();
        Patient.Maintenance.MaintenanceInjectionDrug.OtherAnestheticDrug = $('#Patient\\.OtherAnestheticDrug\\.Drug').val();
        Patient.Maintenance.MaintenanceInjectionDrug.IntroaperativeAnalgesia.Id = $('#Patient\\.OtherAnestheticDrug\\.IntraoperativeAnalgesia').val();
    }
    else {
        Patient.Maintenance.MaintenanceInjectionDrug = {};
        Patient.Maintenance.MaintenanceInhalantDrug = { Drug: {}, BreathingSystem: {}, BreathingBagSize: {}, IntraoperativeAnalgesia: {} };
        Patient.Maintenance.MaintenanceInhalantDrug.Drug.Id = $('#Patient\\.MaintenanceInhalentDrug\\.Drug').val();
        Patient.Maintenance.MaintenanceInhalantDrug.InductionPercentage = $('#Patient\\.MaintenanceInhalentDrug\\.InductionPercentage').val();
        Patient.Maintenance.MaintenanceInhalantDrug.InductionOxygenFlowRate = $('#Patient\\.MaintenanceInhalentDrug\\.InductionOxygenFlowRate').val();
        Patient.Maintenance.MaintenanceInhalantDrug.MaintenancePercentage = $('#Patient\\.MaintenanceInhalentDrug\\.MaintenancePercentage').val();
        Patient.Maintenance.MaintenanceInhalantDrug.MaintenanceOxygenFlowRate = $('#Patient\\.MaintenanceInhalentDrug\\.MaintenanceOxygenFlowRate').val();
        Patient.Maintenance.MaintenanceInhalantDrug.BreathingSystem.Id = $('#Patient\\.MaintenanceInhalentDrug\\.BreathingSystem').val();
        Patient.Maintenance.MaintenanceInhalantDrug.BreathingBagSize.Id = $('#Patient\\.MaintenanceInhalentDrug\\.BreathingBagSize').val();
        Patient.Maintenance.MaintenanceInhalantDrug.OtherAnestheticDrug = $('#Patient\\.OtherAnestheticDrug\\.Drug').val();
        Patient.Maintenance.MaintenanceInhalantDrug.IntraoperativeAnalgesia.Id = $('#Patient\\.OtherAnestheticDrug\\.IntraoperativeAnalgesia').val();
    }
}

function dropdownSelected(domObject) {
    var section = $(domObject).closest("form")[0].name;
    var name = domObject.name;
    var value = domObject.value;
    if (section && name) {
        Patient[section][name] = {};
        Patient[section][name].Id = value;
    }
}

function dateSelected(domObject) {
    var section = $(domObject).closest("form")[0].name;
    var name = domObject.name;
    var value = domObject.value;
    var subgroup = $(domObject).attr('subgroup');
    if (section && name) {
        if (subgroup)
            Patient[section][subgroup][name] = value;
        else
            Patient[section][name] = value;
    }
}

function SaveForm() {
    Patient.PatientInfo.FormCompleted = 'N';
    Patient.PatientInfo.Student.Username = UserInformation.Username;
    Patient.PatientInfo.Clinician.Username = UserInformation.Username;
    var anesthesiaValues = $("#Patient\\.ClinicalFindings\\.AnesthesiaConcerns").multiselect("getChecked");
    for (var i = 0; i < anesthesiaValues.length; i++) {
        var idOfVal = anesthesiaValues[i].getAttribute("value");
        var val = { Concern: { Id: idOfVal} };
        Patient.ClinicalFindings.AnesthesiaConcerns.push(val);
    }
    buildAnestheticPlanPremeds();
    buildInduction();
    buildMaintenance();
    var url = "";
    if (newPatient) {
        url = 'CreateForm'
    }
    else {
        url = 'SaveForm'
    }
    ajax('Post', url, JSON.stringify(Patient), true)
    .done(function (data) {
        if (data.success) {
            //Reload user form dropdown
            alert("Form saved");
            GetUserForms();
        }
        else {
            alert("Error: Form could not be saved");
        }
    })
    .fail(function (jqXHR, textStatus) {
        alert("Error: Form could not be saved");
    });
}

function OpenForm(formId) {
    var pat = { PatientId: formId };
    ajax('Post', 'GetPatient', JSON.stringify(pat), false)
    .done(function (data) {
        if (data.success) {
            var patient = data.Patient;
            for (var i in patient) {
                if (patient.hasOwnProperty(i)) {
                    var section = patient[i];
                    for (var j in section) {
                        if (section.hasOwnProperty(j)) {
                            var input = section[j];
                            var $input = $('#Patient\\.' + i + '\\.' + j);
                            if ($input.length) {
                                var value = section[j];
                                if (value && value.length && typeof value != 'string') {
                                    var valArray = new Array();
                                    for (var k = 0; k < value.length; k++) {
                                        var temp1 = value[k];
                                        for (var r in temp1) {
                                            if (temp1[r] && temp1[r].hasOwnProperty('Id'))
                                                valArray.push(temp1[r].Id);
                                        }
                                    }
                                    $input.val(valArray);
                                }
                                else {
                                    if (value && value.hasOwnProperty('Id'))
                                        $input.val(value.Id);
                                    else if (typeof value == 'string' && value.search(/date/i) != -1) {
                                        var date = new Date(parseInt(value.substr(6)));
                                        $input.val(date.toLocaleDateString());
                                    }
                                    else if (value != -1)
                                        $input.val(value);
                                }
                            }
                            //Needed for AnesthesiaPlan
                            else {
                                for (var q in input) {
                                    if (input.hasOwnProperty(q)) {
                                        var input2 = input[q];
                                        var $input2 = $('#Patient\\.' + i + '\\.' + j + '\\.' + q);
                                        if ($input2.length) {
                                            if (input2 && input2.hasOwnProperty('Id'))
                                                $input2.val(input2.Id);
                                            else if (input2 != -1)
                                                $input2.val(input2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else {
            alert("Could not open form");
        }
    })
    .fail(function (data) {
        alert("Could not open form");
    });
}

function GetUserForms() {
    ajax('Post', 'GetUserForms', JSON.stringify(UserInformation), true)
    .done(function (data) {
        if (data.success) {
            var forms = $('#saved-forms');
            forms.empty();
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
        return true;
    }
    else {
        alert('Validate User Failed');
        return false;
    }
}

function editUserInformation() {
    var foobarredUser = UserInformation;
    foobarredUser.FullName = $("#Patient\\.Profile\\.FullName").val();
    foobarredUser.EmailAddress = $("#Patient\\.Profile\\.Email").val();
    ajax('Post', 'EditProfile', JSON.stringify(foobarredUser), true)
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
}

function setProfileInfo() {
    $("#Patient\\.Profile\\.FullName").val(UserInformation.FullName);
    $("#Patient\\.Profile\\.Email").val(UserInformation.EmailAddress);
}

function populateAll() {
    populate(1, "Patient\\.PatientInfo\\.Procedure");
    populate(2, "Patient\\.PatientInfo\\.Temperament");
    populate(4, "Patient\\.PatientInfo\\.PreOperationPainAssessment");
    populate(5, "Patient\\.PatientInfo\\.PostOperationPainAssessment");
    populate(6, "Patient\\.ClinicalFindings\\.CardiacAuscultation");
    populate(7, "Patient\\.ClinicalFindings\\.PulseQuality");
    populate(21, "Patient\\.ClinicalFindings\\.CapillaryRefillTime");
    populate(8, "Patient\\.ClinicalFindings\\.RespiratoryAuscultation");
    populate(10, "Patient\\.ClinicalFindings\\.PhysicalStatusClassification");
    populate(26, "Patient\\.ClinicalFindings\\.MucousMembraneColor");
    populate(11, "Patient\\.ClinicalFindings\\.AnesthesiaConcerns");
    populate(14, "Patient\\.AnestheticPlan\\.PreMedications\\.Route");
    populate(22, "Patient\\.AnestheticPlan\\.PreMedications\\.SedativeDrug");
    populate(23, "Patient\\.AnestheticPlan\\.PreMedications\\.OpioidDrug");
    populate(24, "Patient\\.AnestheticPlan\\.PreMedications\\.AnticholinergicDrug");
    populate(15, "Patient\\.AnestheticPlan\\.InjectionPlan\\.Drug");
    populate(14, "Patient\\.AnestheticPlan\\.InjectionPlan\\.Route");
    populate(25, "Patient\\.AnestheticPlan\\.InjectionPlan\\.IVFluidType");
    populate(16, "Patient\\.AnestheticPlan\\.InhalantPlan\\.Drug");
    populate(15, "Patient\\.MaintenanceInjectionDrug\\.Drug");
    populate(14, "Patient\\.MaintenanceInjectionDrug\\.RouteOfAdministration");
    populate(16, "Patient\\.MaintenanceInhalentDrug\\.Drug");
    populate(17, "Patient\\.MaintenanceInhalentDrug\\.BreathingSystem");
    populate(18, "Patient\\.MaintenanceInhalentDrug\\.BreathingBagSize");
    populate(19, "Patient\\.OtherAnestheticDrug\\.IntraoperativeAnalgesia");
    populate(20, "Patient\\.Monitoring\\.Monitoring");

}

function populate(id, name) {

    var num = parseInt(id);
    var values;
    var dCatName;
    for (var i = 0; i < DropdownCategories.length; i++) {
        if (DropdownCategories[i].Id == num) {
            values = DropdownCategories[i].DropdownValues;
            dCatName = DropdownCategories[i].ShortName;
            break;
        }
    }
    var x = $('#' + name);
    if (dCatName != "Anesthesia Concerns" && dCatName != "Monitoring") 
        x.append('<option value="-1"> - Select One - </option>');
    for (var i = 0; i < values.length; i++) {
        var option = '<option value="' + values[i].Id + '">' + values[i].Label + '</option>';
        x.append(option);
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
    ajax('Post', 'CheckForgotPassword', JSON.stringify(ASFUser1), false)
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
                        }]
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
        ajax('Post', 'ChangeForgottenPassword', JSON.stringify(ASFUser1), false)
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

function showPriorAnesthesia() {
    //$('#Patient.ClinicalFindings.Date').css('visibility','visible');
    //$('#Patient.ClinicalFindings.Problems').css('visibility', 'visible');
    $('#Patient.ClinicalFindings.Date').show();
    $('#Patient.ClinicalFindings.Problems').show();
}

function hidePriorAnesthesia() {
    $('#Patient.ClinicalFindings.Date').hide();
    $('#Patient.ClinicalFindings.Problems').hide();
}

function GetAllDropdownCategories() {
    var dCats;
    ajax('Post', 'GetAllDropdownCategories', '', false)
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

//Populates dropdown category select box on the Admin page
function PopulateAdminCategories() {
    var cat = $('#dropdownCat');
    cat.append('<option value="0"> - Select One - </option>');
    for (var i = 0; i < DropdownCategories.length; i++) {
        var e = '<option value="' + DropdownCategories[i].Id + '">' + DropdownCategories[i].ShortName + '</option>';
        cat.append(e);
    }
}

//Creates table for editing dropdown values on the Admin page
function PopulateAdminDropdownValues(idOfCat) {
    if (idOfCat != 0) {
        var obj = { Id: idOfCat };
        ajax('Post', 'GetDropdownValues', JSON.stringify(obj), false)
        .done(function (data) {
            if (data.success) {
                var values = data.DropdownValues;
                $("#dropdown-body").empty();

                //Create a table row for each value
                for (var i = 0; i < values.length; i++) {
                    var id = values[i].Id;
                    var label = values[i].Label;
                    var desc = values[i].Description;
                    var labelInput = "<input type='text' id='" + id + "-label' value='" + label + "'/>";
                    var descInput = "<input type='text' id='" + id + "-desc' value='" + desc + "'/>";
                    var deleteButton = "<input type='button' class='submit' onclick='removeDropdownValue(" + id + ")' value='Delete'/>";
                    var editButton = "<input type='button' class='submit' onclick=\"editDropdownValue(" + id + ", $('#" + id + "-label').val(), $('#" + id + "-desc').val())\" value='Edit'/>";
                    var row = "<tr><td>" + labelInput + "</td><td>" + descInput + "</td><td>" + deleteButton + "</td><td>" + editButton + "</td></tr>";
                    $("#dropdown-body").append(row);
                }
                //Row for adding new values
                var newLabelInput = "<input type='text' id='new-label'/>";
                var newDescInput = "<input type='text' id='new-desc'/>";
                var addButton = "<input type='button' class='submit' onclick='addDropdownValue(" + idOfCat + ", $(\"#new-label\").val(),$(\"#new-desc\").val())' value='Add'/>";
                var newRow = "<tr><td>" + newLabelInput + "</td><td>" + newDescInput + "</td><td>" + addButton + "</td><td></td></tr>";
                $("#dropdown-body").append(newRow);
            }
            else
                alert("Clould not get drop down values");
        })
        .fail(function (data) {
            alert("Clould not get drop down values");
        });
    }
}

// edits a dropdown value's label and/or description
function editDropdownValue(id, label, desc) {
    var returned;
    var dropdown = {
        Id: id,
        Label: label,
        Description: desc + " "
        }

        ajax('Post', 'EditDropdownValue', JSON.stringify(dropdown), true)
        .done(function(data) {
            if(data.success) {
                alert("Value edited successfully");
            }
            else {
                alert("Error: value could not be edited");
            }
        })
        .fail(function(data) {
            alert("Error: value could not be edited");
        });
}

function deleteDropdownValue(id) { 
}

function addDropDownValue(idOfCat, value, desc) {}

function validateUser(member, password) {
    var returned = false;
    if (member && password) {
        var memberInfo = {
            Username: member,
            Password: password
        };
        ajax('Post', "DoLogin", JSON.stringify(memberInfo), false)
        .fail(function (jqXHR, textStatus) {
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
        ajax('Post', 'RegisterUser', JSON.stringify(ASFUser1), false)
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

    ajax('Post', 'GetUsers', '', false)
    .done(function (data) {
        if (data.succes) {
            users = data.users;

            for (var i = 0; i < users.length; i++) {
                var name = users[i].FullName;
                var username = users[i].Username;
                $("#users").append("<option value='" + username + "'>" + name + " - " + username + "</option>");
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
        };
        ajax('Post', 'DeleteUser', JSON.stringify(ASFUser1), true)
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
        }/// <reference path="http://localhost/VSOAP/Scripts/" />
        ajax('Post', 'PromoteUser', JSON.stringify(ASFUser1), true)
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

//toggle visibilty for elements
function showInputs(ids) {
    for (var i = 0; i < ids.length; i++) {
        $('#' + ids[i]).show('slow');
    }
}
function hideInputs(ids) {
    for (var i = 0; i < ids.length; i++) {
        $('#' + ids[i]).hide('slow');
    }
}

String.prototype.hashCode = function(){
	var hash = 0;
	if (this.length == 0) return hash;
	for (i = 0; i < this.length; i++) {
		var charac = this.charCodeAt(i);
		hash = ((hash<<5)-hash)+charac;
		hash = hash & hash; // Convert to 32bit integer
	}
	return hash.toString();
}
function toolTipGenerate(id, name) {
    var num = parseInt(id);
    num = num - 1;
    var cats = DropdownCategories;
    var values = cats[num].DropdownValues;
    var e = document.getElementById(name);
    var current = e.options[e.selectedIndex].text;
    var description;
    for (var i = 0; i < values.length; i++) {
        if (values[i].Label == current) {
            description = values[i].Description;
        }
    }
    if(description != null) {
        alert(description);
    } else {
        alert("No description avaliable");
    }
}
function calculateDosages() {

    var weight = document.getElementById("Patient.PatientInfo.BodyWeight").value;

    var dose;
    var dosage;
    var cats = DropdownCategories;
    if (weight != "") {
        //dosage = weight * dose;
        weight = parseFloat(weight);

        var sed = specificCalculations(21, "Patient.AnestheticPlan.PreMedications.SedativeDrug", "Patient.AnestheticPlan.PreMedications.SedativeDosage");
        if (sed == null) {
            document.getElementById("Premed-Sedative-Dosage").innerHTML = "Unable to Calculate";
        } else if (sed == "dose") {
            document.getElementById("Premed-Sedative-Dosage").innerHTML = "Enter Dosage";
        } else {
            document.getElementById("Premed-Sedative-Dosage").innerHTML = sed+ "mL";
        }
        var opi = specificCalculations(22, "Patient.AnestheticPlan.PreMedications.OpioidDrug", "Patient.AnestheticPlan.PreMedications.OpioidDosage");
        if (opi == null) {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = "Unable to Calculate";
        } else if (opi == "dose") {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = "Enter Dosage";
        } else {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = opi + "mL";
        }

        var anti = specificCalculations(23, "Patient.AnestheticPlan.PreMedications.AnticholinergicDrug", "Patient.AnestheticPlan.PreMedications.AnticholinergicDosage");
        if (anti == null) {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = "Unable to Calculate";
        } else if (anti == "dose") {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = "Enter Dosage";
        } else {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = anti + "mL";
        }

        var induc = specificCalculations(14, "Patient.AnestheticPlan.InjectionPlan.Drug", "Patient.AnestheticPlan.InjectionPlan.Dosage");
        if (induc == null) {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = "Unable to Calculate";
        } else if (induc == "dose") {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = "Enter Dosage";
        } else {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = induc + "mL";
        }

        var main = specificCalculations(14, "Patient.MaintenanceInjectionDrug.Drug", "Patient.MaintenanceInjectionDrug.Dosage");
        if (main == null) {
            document.getElementById("Maintenance-Injectable-Dosage").innerHTML = "Unable to Calculate";
        } else if (main == "dose") {
            document.getElementById("Maintenance-Injectable-Dosage").innerHTML = "Enter Dosage";
        } else {
            document.getElementById("Maintenance-Injectable-Dosage").innerHTML = main + "mL";
        }

        var epi = .1 * weight;
        document.getElementById("Emergency-Epinephrine").innerHTML = epi + "mL";

        var atro = .1 * weight;
        document.getElementById("Emergency-Atropine").innerHTML = atro + "mL";
    } else {
        document.getElementById("Premed-Sedative-Dosage").innerHTML = "Enter Body Weight";
        document.getElementById("Premed-Opioid-Dosage").innerHTML = "Enter Body Weight"; 
        document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = "Enter Body Weight";
        document.getElementById("Induction-Injectable-Dosage").innerHTML = "Enter Body Weight";
        document.getElementById("Maintenance-Injectable-Dosage").innerHTML = "Enter Body Weight";
        document.getElementById("Emergency-Epinephrine").innerHTML = "Enter Body Weight";
        document.getElementById("Emergency-Atropine").innerHTML = "Enter Body Weight";
    }
}

function specificCalculations(id, name, dosage) {
    var dosageVal = document.getElementById(dosage).value;
    if (dosageVal == "") {
        return "dose";
    }
    dosageVal = parseFloat(dosageVal);
    var weight = document.getElementById("Patient.PatientInfo.BodyWeight").value;
    weight = parseFloat(weight);
    var num = parseInt(id);
    num = num - 1;
    var cats = DropdownCategories;
    var values = cats[num].DropdownValues;
    var e = document.getElementById(name);
    var current = e.options[e.selectedIndex].text;
    var concentraion;
    var maxDose;
    for (var i = 0; i < values.length; i++) {
        if (values[i].Label == current) {
            concentraion = values[i].Concentration;
            maxDose = values[i].MaxDosage;
        }

    }
    if (concentraion == null) {
        return null;
    }
    if (maxDose != 0) {
        if (maxDose < dosageVal) {
            alert("Alert: Dosage greater than max for " + current +", automatically altered");
            dosageVal = maxDose;
        }
    }
    var dose = dosageVal * weight;
    var mL = dose / concentraion;
    mL = mL.toFixed(2);
    return mL;

}
