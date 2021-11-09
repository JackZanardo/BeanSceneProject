
$(document).ready(function () {

    $("#StartDate").attr("min", getDateString(new Date()));
    $("#CloseTime").attr("min", $("#OpenTime").val());


    $(function () {
        $("#SittingTypeSelect").change(function () {   
            let selectVal = $("#SittingTypeSelect").val();
            if (!isNaN(selectVal)) {
                removeTimeWarings();
                let defaultOpenTime = $(".DefaultTimes input[data-id=" + selectVal + "]").attr("data-open");
                let defaultCloseTime = $(".DefaultTimes input[data-id=" + selectVal + "]").attr("data-close");
                $("#OpenTime").val(defaultOpenTime);
                $("#CloseTime").val(defaultCloseTime);
                $("#CloseTime").attr("min", defaultOpenTime);
            }
            validateSittingTypeSelect();
        });
    });

    $(function () {
        $("#SittingCreate").submit(function (event) {
            let success = true;
            success = validateDate() && success;
            success = validateRestaurantSelect() && success;
            success = validateSittingTypeSelect() && success;
            success = validateTime() && success;
            success = validateCapacity() && success;
            if (!success) {
                event.preventDefault();
                event.stopPropagation();
            }
        });
    });

    $(function () {
        $("#RestuarantSelect").change(function () {
            validateRestaurantSelect();
        });
    });

    $(function () {
        $("#StartDate").change(function () {
            validateDate();
        });
    });


    $(function () {
        $("#OpenTime").change(function () {
            $("#CloseTime").attr("min", $("#OpenTime").val());
            validateTime();
        });
    });

    $(function () {
        $("#CloseTime").change(function () {
            validateTime();
        });
    });

    $(function () {
        $("#Capacity").change(function () {
            validateCapacity();
        });
    });



    function getDateString(date) {
        var ds = date.getFullYear();
        ds += "-" + (date.getMonth() + 1);
        let day = date.getDate();
        if (day <= 9) {
            ds += "-0" + day;
        }
        else {
            ds += "-" + date.getDate();
        }
        return ds;
    }

    function addMinutes(date, minutes) {
        return new Date(date.getTime() + minutes * 60000);
    }

    function getDateTimeObj(time, date) {
        let dt = date + "T" + time;
        return new Date(dt);
    }


    function validateTime() {
        removeTimeWarings();

        let open = getDateTimeObj($("#OpenTime").val(), $("#StartDate").val());
        let close = getDateTimeObj($("#CloseTime").val(), $("#StartDate").val());
        let diff = (close.getTime() - open.getTime()) / 60000;
        if (diff <= 0) {
            $("#CloseTime").addClass("is-invalid");
            $("#CloseTimeHelp").text("Close time begins before open time");
            $("#OpenTime").addClass("is-invalid");
            $("#OpenTimeHelp").text("Open time begins after close time");
            return false;
        }
        if ($("#CloseTime").val() == "" && $("#OpenTime").val() == "") {
            $("#CloseTime").addClass("is-invalid");
            $("#CloseTimeHelp").text("Close time required");
            $("#OpenTime").addClass("is-invalid");
            $("#OpenTimeHelp").text("Open time required");
            return false;
        }
        if ($("#CloseTime").val() == "") {
            $("#CloseTime").addClass("is-invalid");
            $("#CloseTimeHelp").text("Close time required");
            return false;
        }
        if ($("#OpenTime").val() == "") {
            $("#OpenTime").addClass("is-invalid");
            $("#OpenTimeHelp").text("Open time required");
            return false;
        }
        if (diff < 60) {
            $("#OpenTimeWarn").text("Sitting time less than 1 hour");
            $("#CloseTimeWarn").text("Sitting time less than 1 hour");
            return true;
        }
        return true;
    }

    function validateDate() {
        if ($("#StartDate").val() == "") {
            $("#OpenTime").prop("disabled", true);
            $("#CloseTime").prop("disabled", true);
            $("#StartDateHelp").text("Date must be selected");
            $("#StartDate").addClass("is-invalid");
            return false;
        }
        else {
            $("#OpenTime").prop("disabled", false);
            $("#CloseTime").prop("disabled", false);
            $("#StartDateHelp").text("");
            $("#StartDate").removeClass("is-invalid");
            return true;
        }
    }

    function validateRestaurantSelect() {
        $("#RestuarantSelectHelp").text("");
        $("#RestuarantSelect").removeClass("is-invalid");
        if ($("#RestuarantSelect").val() == "") {
            $("#RestuarantSelectHelp").text("Must select a restaurant");
            $("#RestuarantSelect").addClass("is-invalid");
            return false;
        }
        return true;
    }

    function validateSittingTypeSelect() {
        $("#SittingTypeSelectHelp").text("");
        $("#SittingTypeSelect").removeClass("is-invalid");
        if ($("#SittingTypeSelect").val() == "") {
            $("#SittingTypeSelectHelp").text("Must select a sitting type");
            $("#SittingTypeSelect").addClass("is-invalid");
            return false;
        }
        return true;
    }

    function validateCapacity() {
        $("#CapacityWarn").text("");
        $("#CapacityHelp").text("");
        $("#Capacity").removeClass("is-invalid");
        let capacity = parseInt($("#Capacity").val());
        if (capacity === 0) {
            $("#CapacityWarn").text("Capacity of zero entered");
            return true;
        }
        if (isNaN(capacity)) {
            $("#CapacityHelp").text("Capacity required");
            $("#Capacity").addClass("is-invalid");
            return false;
        }
        return true;
    }

    function removeTimeWarings() {
        $("#CloseTime").removeClass("is-invalid");
        $("#CloseTimeHelp").text("");
        $("#CloseTimeWarn").text("");
        $("#OpenTime").removeClass("is-invalid");
        $("#OpenTimeHelp").text("");
        $("#OpenTimeWarn").text("");
    }
});