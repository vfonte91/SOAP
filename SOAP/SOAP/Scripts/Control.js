var Patient = {
    PatientInfo: {},
    ClinicalFindings: {},
    Bloodwork: {},
    AnestheticPlan: { PreMedications: {}, InjectionPlan: {}, InhalantPlan: {} },
    Maintenance: {},
    Monitoring: []
}

var newPatient = true;

var UserInformation = new Object();

var DropdownCategories = new Object();

$(document).ready(function () {

    $("#Patient\\.ClinicalFindings\\.AnesthesiaConcerns").multiselect({ header: "Anesthetic Concerns" });
    $("#Patient\\.ClinicalFindings\\.AnesthesiaConcerns").multiselect("uncheckAll");
    $("#Patient\\.Monitoring\\.Monitoring").multiselect({ header: "Monitoring" });
    $("#Patient\\.Monitoring\\.Monitoring").multiselect("uncheckAll");

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

    //Button to open Edit Profile drop down
    $("#edit-profile-button").click(function () {
        $("#profile-menu").slideToggle("slow")
        $("#edit-profile-button").toggleClass("menu-open");
    });

    //When register is clicked, register inputs are shown
    $("#register").click(function () {
        if (!$("#register-div").is(":visible")) {
            $("#register-div").slideDown('slow');
            document.getElementById("cancelRegister").style.visibility = "visible";
        }
        else {
            //Register user
            var result = registerUser()
        }
    });


    $("#cancelRegister").click(function () {
        $("#register-div").slideUp('slow');
        document.getElementById("cancelRegister").style.visibility = "hidden";
    });

    $("#login").click(function () {
        var username = $.trim($("#username").val());
        var password = $("#password").val();
        var passwordHash = password.hashCode();
        login(username, passwordHash);
    });

    if (sessionStorage.username && sessionStorage.password) {
        if (login(sessionStorage.username, sessionStorage.password)) {
            if (sessionStorage.formId) {
                OpenForm(sessionStorage.formId);
            }
        }
    }

    $("#Patient\\.PatientInfo\\.ProcedureDate").datepicker();
    $("#Patient\\.ClinicalFindings\\.Date").datepicker();

    $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Checked').click(function () {
        toggleInputs($(this), $(this).attr('show').split(","), $(this).attr('hide').split(","));
    });
    $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.Checked').click(function () {
        toggleInputs($(this), $(this).attr('show').split(","), $(this).attr('hide').split(","));
    });
    $("#Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.Checked").click(function () {
        toggleInputs($(this), $(this).attr('show').split(","), $(this).attr('hide').split(","));
    });
    $("#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.Checked").click(function () {
        toggleInputs($(this), $(this).attr('show').split(","), $(this).attr('hide').split(","));
    });

});

function ExportToPDF() {
    ajax('Post', 'Export', JSON.stringify(Patient), true);
}

function buildPatientInfo() {
    var procedure = $('#Patient\\.PatientInfo\\.Procedure').val();
    var otherProcedure = $('#Patient\\.PatientInfo\\.ProcedureOther').val();
    var bodyWeight = $('#Patient\\.PatientInfo\\.BodyWeight').val();
    var ageInMonths = $('#Patient\\.PatientInfo\\.AgeInMonths').val();
    var ageInYears = $('#Patient\\.PatientInfo\\.AgeInYears').val();
    var temperament = $('#Patient\\.PatientInfo\\.Temperament').val();
    var procedureDate = $('#Patient\\.PatientInfo\\.ProcedureDate').val();
    var preOp = $('#Patient\\.PatientInfo\\.PreOperationPainAssessment').val();
    var postOp = $('#Patient\\.PatientInfo\\.PostOperationPainAssessment').val();
    var student = UserInformation.Username;
    var clinician = UserInformation.Username;
    Patient.PatientInfo = { Procedure: { Id: procedure }, ProcedureOther: otherProcedure, BodyWeight: bodyWeight, AgeInMonths: ageInMonths, AgeInYears: ageInYears,
        Temperament: { Id: temperament }, ProcedureDate: procedureDate, PreOperationPainAssessment: { Id: preOp }, PostOperationPainAssessment: { Id: postOp },
        Student: { Username: student }, Clinician: { Username: student }
    }
}

function buildClinicalFindings() {
    var temperature = $('#Patient\\.ClinicalFindings\\.Temperature').val();
    var pulseRate = $('#Patient\\.ClinicalFindings\\.PulseRate').val();
    var respiratoryRate = $('#Patient\\.ClinicalFindings\\.RespiratoryRate').val();
    var cardiacAusc = $('#Patient\\.ClinicalFindings\\.CardiacAuscultation').val();
    var pulseQuality = $('#Patient\\.ClinicalFindings\\.PulseQuality').val();
    var capRefill = $('#Patient\\.ClinicalFindings\\.CapillaryRefillTime').val();
    var respiratoryAusc = $('#Patient\\.ClinicalFindings\\.RespiratoryAuscultation').val();
    var physicalStatus = $('#Patient\\.ClinicalFindings\\.PhysicalStatusClassification').val();
    var classification = $('#Patient\\.ClinicalFindings\\.ReasonForClassification').val();
    var mucous = $('#Patient\\.ClinicalFindings\\.MucousMembraneColor').val();
    var priorAnesDate = $('#Patient\\.ClinicalFindings\\.Date').val();
    var priorAnesProb = $('#Patient\\.ClinicalFindings\\.Problems').val();
    var currentMeds = $('#Patient\\.ClinicalFindings\\.CurrentMedications').val();
    var otherConc = $('#Patient\\.ClinicalFindings\\.OtherAnestheticConcerns').val();
    var anesthesiaValues = $("#Patient\\.ClinicalFindings\\.AnesthesiaConcerns").multiselect("getChecked");
    Patient.ClinicalFindings = { Temperature: temperature, PulseRate: pulseRate, RespiratoryRate: respiratoryRate, CardiacAuscultation: { Id: cardiacAusc },
        CapillaryRefillTime: { Id: capRefill }, PulseQuality: { Id: pulseQuality }, RespiratoryAuscultation: { Id: respiratoryAusc },
        PhysicalStatusClassification: { Id: physicalStatus }, ReasonForClassification: classification, MucousMembraneColor: { Id: mucous }, CurrentMedications: currentMeds,
        AnesthesiaConcerns: [], OtherAnestheticConcerns: otherConc, PriorAnesthesia: {}
    };
    for (var i = 0; i < anesthesiaValues.length; i++) {
        var idOfVal = anesthesiaValues[i].getAttribute("value");
        var val = { Concern: { Id: idOfVal} };
        Patient.ClinicalFindings.AnesthesiaConcerns.push(val);
    }
    if ($('#Patient\\.ClinicalFindings\\.PriorAnesthesia\\.Yes').is(':checked')) {
        Patient.ClinicalFindings.PriorAnesthesia = { DateOfProblem: priorAnesDate, Problem: priorAnesProb };
    }
}

function buildBloodwork() {
    var pcv = $('#Patient\\.Bloodwork\\.PCV').val();
    var tp = $('#Patient\\.Bloodwork\\.TP').val();
    var albumin = $('#Patient\\.Bloodwork\\.Albumin').val();
    var globulin = $('#Patient\\.Bloodwork\\.Globulin').val();
    var wbc = $('#Patient\\.Bloodwork\\.WBC').val();
    var na = $('#Patient\\.Bloodwork\\.NA').val();
    var k = $('#Patient\\.Bloodwork\\.K').val();
    var cl = $('#Patient\\.Bloodwork\\.Cl').val();
    var ca = $('#Patient\\.Bloodwork\\.Ca').val();
    var ica = $('#Patient\\.Bloodwork\\.iCa').val();
    var glucose = $('#Patient\\.Bloodwork\\.Glucose').val();
    var alt = $('#Patient\\.Bloodwork\\.ALT').val();
    var alp = $('#Patient\\.Bloodwork\\.ALP').val();
    var bun = $('#Patient\\.Bloodwork\\.BUN').val();
    var creat = $('#Patient\\.Bloodwork\\.CREAT').val();
    var usg = $('#Patient\\.Bloodwork\\.USG').val();
    var otherType = $('#Patient\\.Bloodwork\\.OtherType').val();
    var otherResult = $('#Patient\\.Bloodwork\\.OtherValue').val();
    Patient.Bloodwork = { PCV: pcv, TP: tp, Albumin: albumin, Globulin: globulin, WBC: wbc, NA: na, K: k, Cl: cl,
        Ca: ca, iCa: ica, Glucose: glucose, ALT: alt, ALP: alp, BUN: bun, CREAT: creat, USG: usg, OtherType: otherType, OtherValue: otherResult
    };
}

function buildAnestheticPlanPremeds() {
    Patient.AnestheticPlan.PreMedications = { SedativeDrug: {}, OpioidDrug: {},  AnticholinergicDrug: {}};
    var route = $("#Patient\\.AnestheticPlan\\.PreMedications\\.Route").val();
    Patient.AnestheticPlan.PreMedications.Route = { Id: route };

    var sedative = $("#Patient\\.AnestheticPlan\\.PreMedications\\.SedativeDrug").val();
    var sedativeDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.SedativeDosage").val();
    Patient.AnestheticPlan.PreMedications.SedativeDrug = {Id: sedative};
    Patient.AnestheticPlan.PreMedications.SedativeDosage = sedativeDosage;

    var oploid = $("#Patient\\.AnestheticPlan\\.PreMedications\\.OpioidDrug").val();
    var oploidDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.OpioidDosage").val();
    Patient.AnestheticPlan.PreMedications.OpioidDrug = { Id: oploid};
    Patient.AnestheticPlan.PreMedications.OpioidDosage = oploidDosage;

    var antichol = $("#Patient\\.AnestheticPlan\\.PreMedications\\.AnticholinergicDrug").val();
    var anticholDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.AnticholinergicDosage").val();
    Patient.AnestheticPlan.PreMedications.AnticholinergicDrug = {Id: antichol};
    Patient.AnestheticPlan.PreMedications.AnticholinergicDosage = anticholDosage;

    var ketamineDosage = $("#Patient\\.AnestheticPlan\\.PreMedications\\.KetamineDosage").val();
    Patient.AnestheticPlan.PreMedications.KetamineDosage = ketamineDosage;
}

function buildInduction() {
    if ($('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Checked').is(':checked')) {
        Patient.AnestheticPlan.InhalantPlan = {};
        Patient.AnestheticPlan.InjectionPlan = { Drug: {}, Route: {}, IVFluidType: {} };
        Patient.AnestheticPlan.InjectionPlan.Drug.Id = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Drug').val();
        Patient.AnestheticPlan.InjectionPlan.Route.Id = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Route').val();
        Patient.AnestheticPlan.InjectionPlan.Dosage = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.Dosage').val();
        Patient.AnestheticPlan.InjectionPlan.IVFluidType.Id = $('#Patient\\.AnestheticPlan\\.InjectionPlan\\.IVFluidType').val();
        Patient.AnestheticPlan.InjectionPlan.Checked = true;
    }
    else {
        Patient.AnestheticPlan.InjectionPlan = {};
        Patient.AnestheticPlan.InhalantPlan = { Drug: {} };
        Patient.AnestheticPlan.InhalantPlan.Drug.Id = $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.Drug').val();
        Patient.AnestheticPlan.InhalantPlan.Percentage = $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.Percentage').val();
        Patient.AnestheticPlan.InhalantPlan.FlowRate = $('#Patient\\.AnestheticPlan\\.InhalantPlan\\.FlowRate').val();
        Patient.AnestheticPlan.InhalantPlan.Checked = true;
    }
}

function buildMaintenance() {
    if ($('#Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.Checked').is(':checked')) {
        Patient.Maintenance.MaintenanceInhalantDrug = {};
        Patient.Maintenance.MaintenanceInjectionDrug = { Drug: {}, RouteOfAdministration: {} };
        Patient.Maintenance.MaintenanceInjectionDrug.Drug.Id = $('#Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.Drug').val();
        Patient.Maintenance.MaintenanceInjectionDrug.RouteOfAdministration.Id = $('#Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.RouteOfAdministration').val();
        Patient.Maintenance.MaintenanceInjectionDrug.Dosage = $('#Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.Dosage').val();
        Patient.Maintenance.MaintenanceInjectionDrug.Checked = true;
    }
    else {
        Patient.Maintenance.MaintenanceInjectionDrug = {};
        Patient.Maintenance.MaintenanceInhalantDrug = { Drug: {}, BreathingSystem: {}, BreathingBagSize: {} };
        Patient.Maintenance.MaintenanceInhalantDrug.Drug.Id = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.Drug').val();
        Patient.Maintenance.MaintenanceInhalantDrug.InductionPercentage = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.InductionPercentage').val();
        Patient.Maintenance.MaintenanceInhalantDrug.InductionOxygenFlowRate = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.InductionOxygenFlowRate').val();
        Patient.Maintenance.MaintenanceInhalantDrug.MaintenancePercentage = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.MaintenancePercentage').val();
        Patient.Maintenance.MaintenanceInhalantDrug.MaintenanceOxygenFlowRate = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.MaintenanceOxygenFlowRate').val();
        Patient.Maintenance.MaintenanceInhalantDrug.BreathingSystem.Id = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.BreathingSystem').val();
        Patient.Maintenance.MaintenanceInhalantDrug.BreathingBagSize.Id = $('#Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.BreathingBagSize').val();
        Patient.Maintenance.MaintenanceInhalantDrug.Checked = true;
    }
    Patient.Maintenance.MaintenanceOther = { IntraoperativeAnalgesia: {}, IVFluidType: {} };
    Patient.Maintenance.MaintenanceOther.OtherAnestheticDrug = $('#Patient\\.Maintenance\\.MaintenanceOther\\.OtherAnestheticDrug').val();
    Patient.Maintenance.MaintenanceOther.IntraoperativeAnalgesia.Id = $('#Patient\\.Maintenance\\.MaintenanceOther\\.IntraoperativeAnalgesia').val();
    Patient.Maintenance.MaintenanceOther.IVFluidType.Id = $('#Patient\\.Maintenance\\.MaintenanceOther\\.IVFluidType').val();
}

function buildMonitoring() {
    var monitorValues = $("#Patient\\.Monitoring\\.Monitoring").multiselect("getChecked");
    for (var i = 0; i < monitorValues.length; i++) {
        var idOfVal = monitorValues[i].getAttribute("value");
        var val = { Equipment: { Id: idOfVal} };
        Patient.Monitoring.push(val);
    }
    var otherValue = $('#Patient\\.Monitoring\\.OtherMonitoring').val();
    if (otherValue) {
        var val = { OtherEquipment: otherValue };
        Patient.Monitoring.push(val);
    }
}

function SaveForm() {
    if (errorCheckAll()) {
            Patient.PatientInfo.FormCompleted = 'N';
            buildAnestheticPlanPremeds();
            buildInduction();
            buildMaintenance();
            buildPatientInfo();
            buildClinicalFindings();
            buildBloodwork();
            buildMonitoring();
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
                //alert("Form saved");
                popupBox("Form saved");
                GetUserForms();
            }
            else {
                //alert("Error: Form could not be saved");
                popupBox("Error: Form could not be saved");
            }
        })
        .fail(function (jqXHR, textStatus) {
            popupBox("Error: Form could not be saved");
            //alert("Error: Form could not be saved");
        });
    } 
}

function OpenForm(formId) {
    var pat = { PatientId: formId };
    ajax('Post', 'GetPatient', JSON.stringify(pat), true)
    .done(function (data) {
        if (data.success) {
            Patient = data.Patient;
            newPatient = false;
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
                                            else if (input2 && input2.Checked == true)
                                                toggleInputs(input2, $input2.attr('show'), $input2.attr('hide'));
                                            else if (input2 && input2.Checked == false)
                                                $input2.attr('checked', '');
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
            popupBox('Form Successfully Loaded');
            //alert('Form Successfully Loaded');
        }
        else {
            //alert("Could not open form");
            popupBox("Could not open form");
        }
    })
    .fail(function (data) {
        //alert("Could not open form");
        popupBox("Could not open form");
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

        GetAllDropdownCategories();

        //Hides Admin tab if not admin
        if (!UserInformation.IsAdmin) {
            $("#thumbs a.admin").addClass("disabled");
        }
        else {
            getUsers();
        }
        //Stores username and password
        sessionStorage.username = username;
        sessionStorage.password = password;

        GetUserForms();
        return true;
    }
    else {
        popupBox('Validate User Failed');
        return false;
    }
}

function editUserInformation() {
    var foobarredUser = UserInformation;
    var currentPass = $('#Patient\\.Profile\\.CurrentPassword').val();
    var newPass1 = $('#Patient\\.Profile\\.NewPassword').val();
    var newPass2 = $('#Patient\\.Profile\\.NewPasswordTwo').val();
    foobarredUser.FullName = $("#Patient\\.Profile\\.FullName").val();
    foobarredUser.EmailAddress = $("#Patient\\.Profile\\.Email").val();
    ajax('Post', 'EditProfile', JSON.stringify(foobarredUser), true)
        .done(function (data) {
            if (data.success) {
                UserInformation.FullName = foobarredUser.FullName;
                UserInformation.EmailAddress = foobarredUser.EmailAddress;
                $("#profile-menu").slideToggle("slow")
                $("#edit-profile-button").toggleClass("menu-close");
            }
            else {
                //alert('Error updating User Info');
                popupBox('Error updating User Info');
            }
        })
        .fail(function (data) {
            //alert('Error updating User Info');
            popupBox('Error updating User Info');
        });
    if (currentPass && newPass1 && newPass2) {
        if (newPass1 == newPass2) {
            foobarredUser.Member.OldPassword = currentPass.hashCode();
            foobarredUser.Member.Password = newPass1.hashCode();
            ajax('Post', 'ChangePassword', JSON.stringify(foobarredUser), true)
            .done(function (data) {
                if (data.success) {
                    $("#profile-menu").slideToggle("slow")
                    $("#edit-profile-button").toggleClass("menu-close");
                    //alert('User Info Updated');
                    popupBox('User Info Updated');
                }
                else {
                    //alert('Error updating User Info');
                    popupBox('Current password is not correct');
                }
            })
            .fail(function (data) {
                //alert('Error updating User Info');
                popupBox('Error updating User Info');
            });
        }
        else {
            popupBox('New passwords do not match');
        }
    }
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
    populate(15, "Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.Drug");
    populate(14, "Patient\\.Maintenance\\.MaintenanceInjectionDrug\\.RouteOfAdministration");
    populate(16, "Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.Drug");
    populate(17, "Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.BreathingSystem");
    populate(18, "Patient\\.Maintenance\\.MaintenanceInhalantDrug\\.BreathingBagSize");
    populate(19, "Patient\\.Maintenance\\.MaintenanceOther\\.IntraoperativeAnalgesia");
    populate(19, "Patient\\.Maintenance\\.MaintenanceOther\\.IVFluidType");
    populate(20, "Patient\\.Monitoring\\.Monitoring");
    $('#Patient\\.ClinicalFindings\\.AnesthesiaConcerns').multiselect("refresh");
    $('#Patient\\.Monitoring\\.Monitoring').multiselect("refresh");
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
        buttons: [{ text: "Ok", click: securityCheck}],
        open: function (event, ui) {
            $("#usernameForgot").val("");
        }
    });
}

function securityCheck() {
    var forgotUser = $("#usernameForgot").val();
    var foobarredUser = {
        Username: forgotUser
    };
    ajax('Post', 'GetSecurityQuestion', JSON.stringify(foobarredUser), true)
        .done(function (data) {
            if (data.success) {
                $("#securityQuestionCheck").val(data.securityQuestion);
                $("#forgot-password").dialog("close");
                $("#security-Check").dialog({
                    width: 600,
                    height: 400,
                    modal: true,
                    draggable: false,
                    buttons: [{ text: "Submit", click: function () {
                        forgetClicked();
                    }
                    }]
                });
            }
        })
        .fail(function (data) {

        });

}
    

function forgetClicked() {
    var forgotUser = $("#usernameForgot").val();
    var securityAnswer = $("#securityAnswerCheck").val();
    var foobarUser = {
        Username: forgotUser,
        Member: {
            SecurityAnswer: securityAnswer
        }
    };
    ajax('Post', 'CheckSecurityAnswer', JSON.stringify(foobarUser), false)
    .done(function (data) {

        if (data.success) {
         $("#security-Check").dialog("close");
               $("#change-password").dialog({
                    width:600,
                    height: 400,
                    modal: true,
                    draggable: false,
                    buttons:[{text: "Submit", click: function(){
                        ChangePassword(foobarUser);
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
                popupBox('Password successfuly changed!');
            }
            else {
            }
        })
        .fail(function (data) {

        });
    }
    else {
        //alert('Passwords do not match');
        popupBox('Passwords do not match');
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
    ajax('Post', 'GetAllDropdownCategories', '', true)
    .done(function (data) {
        if (data.success) {
            DropdownCategories = data.DropdownCategories;
            populateAll();
            if (UserInformation.IsAdmin)
                PopulateAdminCategories();
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
        ajax('Post', 'GetDropdownValues', JSON.stringify(obj), true)
        .done(function (data) {
            if (data.success) {
                var values = data.DropdownValues;
                $("#dropdown-body").empty();

                //Create a table row for each value
                for (var i = 0; i < values.length; i++) {
                    var id = values[i].Id;
                    var label = values[i].Label;
                    var desc = values[i].Description;
                    var conc = values[i].Concentration;
                    var dos = values[i].MaxDosage;
                    var labelInput = "<input type='text' id='" + id + "-label' value='" + label + "'/>";
                    var descInput = "<input type='text' id='" + id + "-desc' value='" + desc + "'/>";
                    var concInput = "<input type='text' id='" + id + "-conc' value='" + conc + "'/>";
                    var dosageInput = "<input type='text' id='" + id + "-dosage' value='" + dos + "'/>";
                    var deleteButton = "<input type='button' class='submit' onclick='removeDropdownValue(" + id + ")' value='Delete'/>";
                    var editButton = "<input type='button' class='submit' onclick=\"editDropdownValue(" + id + ", $('#" + id + "-label').val(), $('#" + id + "-desc').val(), $('#" + id + "-conc').val(), $('#" + id + "-dosage').val())\" value='Edit'/>";
                    var row = "<tr><td>" + labelInput + "</td><td>" + descInput + "</td><td>" + concInput + "</td><td>" + dosageInput + "</td><td>" + deleteButton + "</td><td>" + editButton + "</td></tr>";
                    $("#dropdown-body").append(row);
                }
                //Row for adding new values
                var newLabelInput = "<input type='text' id='new-label'/>";
                var newDescInput = "<input type='text' id='new-desc'/>";
                var newConcInput = "<input type='text' id='new-conc'/>";
                var newDosageInput = "<input type='text' id='new-dosage'/>";
                var addButton = "<input type='button' class='submit' onclick='addDropdownValue(" + idOfCat + ", $(\"#new-label\").val(),$(\"#new-desc\").val(),$(\"#new-conc\").val(),$(\"#new-dosage\").val())' value='Add'/>";
                var newRow = "<tr><td>" + newLabelInput + "</td><td>" + newDescInput + "</td><td>" + newConcInput + "</td><td>" + newDosageInput + "</td><td>" + addButton + "</td><td></td></tr>";
                $("#dropdown-body").append(newRow);
            }
            else
            popupBox("Could not get drop down values");
        })
        .fail(function (data) {
            popupBox("Could not get drop down values");
        });
    }
}

// edits a dropdown value's label and/or description
function editDropdownValue(id, label, desc, conc, dosage) {
    var returned;
    var dropdown = {
        Id: id,
        Label: label,
        Description: desc + " ",
        Concentration: conc,
        MaxDosage: dosage
        }

        ajax('Post', 'EditDropdownValue', JSON.stringify(dropdown), true)
        .done(function (data) {
            if (data.success) 
                popupBox("Value edited successfully");
            else
                popupBox("Error: value could not be edited");
        })
        .fail(function (data) {
            popupBox("Error: value could not be edited");
        });
}

function deleteDropdownValue(id) { 
}

function addDropdownValue(id, label, desc, conc, dosage) {
    var dropdown = {
        Category: { Id: id },
        Label: label,
        Description: desc + " ",
        Concentration: conc,
        MaxDosage: dosage,
        OtherFlag: "N"
    }

    ajax('Post', 'AddDropdownValue', JSON.stringify(dropdown), true)
        .done(function (data) {
            if (data.success)
                popupBox("Value add successfully");
            else
                popupBox("Error: value could not be added");
        })
        .fail(function (data) {
            popupBox("Error: value could not be added");
        });
}

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
        ajax('Post', 'RegisterUser', JSON.stringify(ASFUser1), true)
        .done(function (data) {
            if (data.success) {
                returned = "success";
                $("#password").val("");
                $("#password-repeat").val("");
                $("#email").val("");
                $("#full-name").val("");
                sessionStorage.username = userName;
                sessionStorage.password = pwHash;
                popupBox('Registration was succesful');
                $("#register-div").slideUp('slow');
            }
            else {
                popupBox("User could not be registered");
            }
        })
        .fail(function (data) {
            popupBox("User could not be registered");
        });
    }
}

function getUsers() {
    var users;

    $("#users").empty();

    ajax('Post', 'GetUsers', '', true)
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
    //alert(returned);
    popupBox(returned);
}

function promoteUser(users) {
    var returned = '';
    for (var i = 0; i < users.length; i++) {
        var ASFUser1 = {
            "Username": users[i]
        }
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
    //alert(returned);
    popupBox(returned);
}

//Logs user out of session
function logOut() {
    //Clears session storage
    sessionStorage.clear();

    //Reloads page
    location.reload();
}
//Toggle visibility of elements
function toggleInputs(radioId, showIds, hideIds) {
    //Check selected radio button
    radioId.attr('checked', 'checked');
    showInputs(showIds);
    hideInputs(hideIds);
}
//Show array of elements
function showInputs(ids) {
    for (var i = 0; i < ids.length; i++) {
        $('#' + ids[i]).show('slow');
    }
}
//Hide array of elements
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
    if(description != "") {
        //alert(description);
        popupBox(description);
    } else {
        //alert("No description avaliable");

        popupBox("No Description Avaliable");
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
        } else if (sed == -1) {
            document.getElementById("Premed-Sedative-Dosage").innerHTML = "Pre-set mL Amounts";
        }
        else {
            document.getElementById("Premed-Sedative-Dosage").innerHTML = sed + "mL";
        }
        var opi = specificCalculations(22, "Patient.AnestheticPlan.PreMedications.OpioidDrug", "Patient.AnestheticPlan.PreMedications.OpioidDosage");
        if (opi == null) {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = "Unable to Calculate";
        } else if (opi == "dose") {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = "Enter Dosage";
        } else if (opi == -1) {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = "Pre-set mL Amounts";
        }
         else {
            document.getElementById("Premed-Opioid-Dosage").innerHTML = opi + "mL";
        }

        var anti = specificCalculations(23, "Patient.AnestheticPlan.PreMedications.AnticholinergicDrug", "Patient.AnestheticPlan.PreMedications.AnticholinergicDosage");
        if (anti == null) {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = "Unable to Calculate";
        } else if (anti == "dose") {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = "Enter Dosage";
        } else if (anti == -1) {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = "Pre-set mL Amounts";
        }
         else {
            document.getElementById("Premed-Anticholinergic-Dosage").innerHTML = anti + "mL";
        }

        var induc = specificCalculations(14, "Patient.AnestheticPlan.InjectionPlan.Drug", "Patient.AnestheticPlan.InjectionPlan.Dosage");
        if (induc == null) {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = "Unable to Calculate";
        } else if (induc == "dose") {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = "Enter Dosage";
        } else if (induc == -1) {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = "Pre-set mL Amounts";
        }
         else {
            document.getElementById("Induction-Injectable-Dosage").innerHTML = induc + "mL";
        }

        var main = specificCalculations(14, "Patient.Maintenance.MaintenanceInjectionDrug.Drug", "Patient.Maintenance.MaintenanceInjectionDrug.Dosage");
        if (main == null) {
            document.getElementById("Maintenance-Injectable-Dosage").innerHTML = "Unable to Calculate";
        } else if (main == "dose") {
            document.getElementById("Maintenance-Injectable-Dosage").innerHTML = "Enter Dosage";
        } else if (main == -1) {
            document.getElementById("Maintenance-Injectable-Dosage").innerHTML = "Pre-set mL Amounts";
        }
         else {
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
 
    var weight = document.getElementById("Patient.PatientInfo.BodyWeight").value;
    weight = parseFloat(weight);
    var num = parseInt(id);
    num = num - 1;
    var cats = DropdownCategories;
    var values = cats[num].DropdownValues;
    var e = document.getElementById(name);
    var current = e.options[e.selectedIndex].text;
    var concentration;
    var maxDose;
    for (var i = 0; i < values.length; i++) {
        if (values[i].Label == current) {
            concentration = values[i].Concentration;
            maxDose = values[i].MaxDosage;
        }

    }
    if (concentration == null) {
        return null;
    } else if (concentration == "-1") {
         return -1;
     }
     if (dosageVal == "") {
         return "dose";
     }
    if (maxDose != 0) {
        if (maxDose < dosageVal) {
            //alert("Alert: Dosage greater than max for " + current +", automatically altered");
            popupBox("Dosage more than maximum allowed detected. Automatically reduced.");
            dosageVal = maxDose;
        }
    }
    dosageVal = parseFloat(dosageVal);
    var dose = dosageVal * weight;
    var mL = dose / concentration;
    mL = mL.toFixed(2);
    return mL;

}
function popupBox(text) {
    $("#dialog-modal").dialog(
    {

        width: 350,
        height: 200,
        title: "Notification",

        open: function (event, ui) {

        }

    });
    $(".ui-dialog .ui-widget-content").css("background-color", "White");
    $(".ui-dialog .ui-dialog-titlebar").css("background-color", "Red");
    $("#dialog-modal").text(text);

    //$("dialog-modal").dialog("open");
    //alert("here");
}
function errorCheckAll() {
    var retVal = true;
    var check = errorCheckPatientInfo();
    if (!check) {
        retVal = false;
    }

    check = errorCheckClinicalFindings();
    if (!check) {
        retVal = false;
    }

    check = errorCheckBloodwork();
    if (!check) {
        retVal = false;
    }

    check = errorCheckAnetheticPlan();
    if (!check) {
        retVal = false;
    }

    check = errorCheckInduction();
    if (!check) {
        retVal = false;
    }

    check = errorCheckMaintenance();
    if (!check) {
        retVal = false;
    }

    check = errorCheckMonitoring();
    if (!check) {
        retVal = false;
    }
    return retVal;
}
function errorCheckPatientInfo() {
    var retVal = true;
    var check = document.getElementById("Patient.PatientInfo.ProcedureOther").value;
    if (check.length > 256) {
        popupBox("The string in Procedure Other is too long");
        retVal = false;
    }

    check = document.getElementById("Patient.PatientInfo.BodyWeight").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Body Weight is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.PatientInfo.AgeInYears").value;
    if (parseInt(check) > 127 || parseInt(check) < 0) {
        popupBox("The value in Age in Years is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.PatientInfo.AgeInMonths").value;
    if (parseInt(check) > 127 || parseInt(check) < 0) {
        popupBox("The value in Age in Months is incorrectly formatted");
        retVal = false;
    }
    return retVal;
}

function errorCheckClinicalFindings() {
    var retVal = true;

    var check = document.getElementById("Patient.ClinicalFindings.Temperature").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Temperature is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.ClinicalFindings.PulseRate").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Pulse Rate is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.ClinicalFindings.RespiratoryRate").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Respiratory Rate is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.ClinicalFindings.ReasonForClassification").value;
    if (check.length > 250) {
        popupBox("The string in Reason For Classification is too long");
        retVal = false;
    }

    check = document.getElementById("Patient.ClinicalFindings.OtherAnestheticConcerns").value;
    if (check.length > 250) {
        popupBox("The string in Other Anesthetic Concerns is too long");
        retVal = false;
    }

    check = document.getElementById("Patient.ClinicalFindings.CurrentMedications").value;
    if (check.length > 250) {
        popupBox("The string in Current Medications is too long");
        retVal = false;
    }

    return retVal;

}

function errorCheckBloodwork() {
    var retVal = true;

    var check = document.getElementById("Patient.Bloodwork.PCV").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in PCV is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.TP").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in TP is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.Albumin").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Albumin is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.Globulin").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Globulin is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.WBC").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in WBC is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.NA").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in NA is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.K").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in K is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.Cl").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Cl is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.Ca").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Ca is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.iCa").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in iCa is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.Glucose").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Glucose is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.ALT").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in ALT is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.ALP").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in ALP is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.BUN").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in BUN is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.CREAT").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in CREAT is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.USG").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in USG is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.OtherType").value;
    if (check.length > 24) {
        popupBox("The string in Other Bloodwork is too long");
        retVal = false;
    }

    check = document.getElementById("Patient.Bloodwork.OtherValue").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 2)) || check.length > 5) {
        popupBox("The value in Other Bloodwork Results is incorrectly formatted");
        retVal = false;
    }

    return retVal;
}

function errorCheckAnetheticPlan() {
    var retVal = true;

    var check = document.getElementById("Patient.AnestheticPlan.PreMedications.SedativeDosage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Sedative Dosage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.AnestheticPlan.PreMedications.OpioidDosage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Opioid Dosage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.AnestheticPlan.PreMedications.AnticholinergicDosage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Anticholinergic Dosage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.AnestheticPlan.PreMedications.KetamineDosage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Ketamine Dosage is incorrectly formatted");
        retVal = false;
    }

    return retVal;
}
function errorCheckInduction() {
    var retVal = true;

    var check = document.getElementById("Patient.AnestheticPlan.InjectionPlan.Dosage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Induction Injection Dosage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.AnestheticPlan.InhalantPlan.Percentage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Induction Inhalant Percentage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.AnestheticPlan.InhalantPlan.FlowRate").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Induction Inhalant Flow Rate is incorrectly formatted");
        retVal = false;
    }

    return retVal;
}
function errorCheckMaintenance() {
    var retVal = true;

    var check = document.getElementById("Patient.Maintenance.MaintenanceInjectionDrug.Dosage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Maintenance Injection Dosage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Maintenance.MaintenanceInhalantDrug.InductionPercentage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Maintenance Inhalant Starting Percentage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Maintenance.MaintenanceInhalantDrug.InductionOxygenFlowRate").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Maintenance Inhalant Starting Flow Rate is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Maintenance.MaintenanceInhalantDrug.MaintenancePercentage").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Maintenance Inhalant Maintenance Percentage is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Maintenance.MaintenanceInhalantDrug.MaintenanceOxygenFlowRate").value;
    if ((check.indexOf(".") != -1 && check.length > (check.indexOf(".") + 3)) || check.length > 5) {
        popupBox("The value in Maintenance Inhalant Maintenance Flow Rate is incorrectly formatted");
        retVal = false;
    }

    check = document.getElementById("Patient.Maintenance.MaintenanceOther.OtherAnestheticDrug").value;
    if (check.length > 250) {
        popupBox("The string in Other Anesthetic Drug is too long");
        retVal = false;
    }

    return retVal;
}
function errorCheckMonitoring() {
    var retVal = true;

    var check = document.getElementById("Patient.Monitoring.OtherMonitoring").value;
    if (check.length > 256) {
        popupBox("The string in Other Monitoring is too long");
        retVal = false;
    }

    return retVal;
}
