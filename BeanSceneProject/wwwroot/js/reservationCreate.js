
$(document).ready(function () {
    let sittingOpen = $("#SittingOpen").val();
    let sittingClose = $("#SittingClose").val();
    StartTimes(sittingOpen, sittingClose);



    $(function () {
        $("#StartTime").change(function () {
            $("#Duration").find('option:not(:first)').remove();
            $("#Duration").prop('disabled', true);
            if (validateStartTime()) {
                let interval = new Date($("#StartTime").val());
                let duration = 15;
                while (interval < Date.parse(sittingClose) && duration <= 90) {
                    var o = new Option(duration + " minutes", duration);
                    $(o).html(duration + " minutes");
                    $("#Duration").append(o);
                    duration += 15;
                    interval = addMinutes(interval, 15);
                }
                $("#Duration").prop('disabled', false);
            }
        });
    });

    $(function () {
        $("#Duration").change(function () {
            validateDuration();
        });
    });

    $(function () {
        $("#ReservationCreate").submit(function (event) {
            let success = true;
            success = validateStartTime() && success;
            success = validateStartTime() && success;
            if (!success) {
                event.preventDefault();
                event.stopPropagation();
            }
        });
    });



    function validateStartTime() {
        $("#StartTimeHelp").text("");
        $("#StartTime").removeClass("is-invalid");
        if ($("#StartTime").val() == "") {
            $("#StartTimeHelp").text("Please select a start time");
            $("#StartTime").addClass("is-invalid");
            return false;
        }
        return true;
    }

    function validateDuration() {
        $("#DurationHelp").text("");
        $("#Duration").removeClass("is-invalid");
        if (!$("#Duration").val()) {
            $("#DurationHelp").text("Please select a start time");
            $("#Duration").addClass("is-invalid");
            return false;
        }
        return true;
    }


})

function StartTimes(sittingOpen, sittingClose) {
    let interval = new Date(sittingOpen);
    while (interval < Date.parse(sittingClose)) {
        var o = new Option(formatAMPM(interval), interval.toISOString());
        $(o).html(formatAMPM(interval));
        $("#StartTime").append(o);
        interval = addMinutes(interval, 15);
    }
}

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function addMinutes(date, minutes) {
    return new Date(date.getTime() + minutes * 60000);
}