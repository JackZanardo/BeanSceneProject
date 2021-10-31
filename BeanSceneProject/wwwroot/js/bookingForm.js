//const { error } = require("jquery");


$(document).ready(function () {
    var availableDates = $("#StartDates").val();
    var sessions = "";

    $(function () {
        $("#StartDate").change(function () {
            $("#StartDate").removeClass("is-invalid");
            $("#StartDateHelp").empty();
            $("#SittingSelect").prop('disabled', true);
            $("#SittingSelect").find('option:not(:first)').remove();
            $("#StartTime").find('option:not(:first)').remove();
            $("#StartTime").prop('disabled', true);
            $("#Duration").find('option:not(:first)').remove();
            $("#Duration").prop('disabled', true);
            $("#CustomerNumHelp").text("");
            $("#CustomerNum").removeClass("is-invalid");
            sessions = "";
            if ($("#StartDate").val()) {
                $.ajax({
                    type: "POST",
                    url: "GetSittings",
                    data: { "jsonDate": $("#StartDate").val() },
                    success: function (data) {
                        sessions = JSON.parse(data);
                        $.each(sessions, function (i, session) {
                            var o = new Option(session.InfoText, session.Id);
                            $(o).html(session.InfoText);
                            $("#SittingSelect").append(o);
                        });
                        $("#SittingSelect").prop('disabled', false);
                    },
                    failure: function () {
                        $("#StartDateHelp").text("No Sessions available");
                        $("#StartDate").addClass("is-invalid");
                    },
                    error: function () {
                        $("#StartDateHelp").text("No Sessions available");
                        $("#StartDate").addClass("is-invalid");
                    }
                });
            }
        });
    });

    $(function () {
        $('#StartDate').datepicker({
            beforeShowDay: isAvailable,
            changeMonth: true,
            changeYear: false,
            dateFormat: "dd/mm/yy"
        });
    });

    function isAvailable(date) {
        var dt =  date.getFullYear();
        dt += "-" + (date.getMonth() + 1);
        dt += "-" + date.getDate();
        dt += "T00:00:00";
        if (availableDates.indexOf(dt) != -1) {
            return [true, "", ""];
        } else {
            return [false, "", ""];
        }
    }

    $(function () {
        $("#SittingSelect").change(function () {
            $("#StartTime").find('option:not(:first)').remove();
            $("#StartTime").prop('disabled', true);
            $("#Duration").find('option:not(:first)').remove();
            $("#Duration").prop('disabled', true);
            $("#CustomerNumHelp").text("");
            $("#CustomerNum").removeClass("is-invalid");
            if (!isNaN($("#SittingSelect").val())) {
                let sittingId = $("#SittingSelect").val();
                let session = sessions.find(s => s.Id === parseInt(sittingId));
                let interval = new Date(session.SittingOpen);
                while (interval < Date.parse(session.SittingClose)) {
                    var o = new Option(formatAMPM(interval), interval.toISOString());
                    $(o).html(formatAMPM(interval));
                    $("#StartTime").append(o);
                    interval = addMinutes(interval, 15);
                }
                $("#StartTime").prop('disabled', false);
                if (!validateCustomerNum(session.Available)) {
                    $("#CustomerNumHelp").text("Not enough free seats");
                    $("#CustomerNum").addClass("is-invalid");
                }
            }
        });
    });

    $(function () {
        $("#CustomerNum").change(function () {
            $("#CustomerNumHelp").text("");
            $("#CustomerNum").removeClass("is-invalid");
            if (!isNaN($("#SittingSelect").val())) {
                let sittingId = $("#SittingSelect").val();
                let session = sessions.find(s => s.Id === parseInt(sittingId));
                if (!validateCustomerNum(session.Available)) {
                    $("#CustomerNumHelp").text("Not enough free seats");
                    $("#CustomerNum").addClass("is-invalid");
                }
            }

        });
    });

    $(function () {
        $("#StartTime").change(function () {
            $("#Duration").find('option:not(:first)').remove();
            $("#Duration").prop('disabled', true);
            if ($("#StartTime").val() != "" && !isNaN($("#SittingSelect").val())) {
                let sittingId = $("#SittingSelect").val();
                let session = sessions.find(s => s.Id === parseInt(sittingId));
                let interval = new Date($("#StartTime").val());
                let duration = 15;
                while (interval < Date.parse(session.SittingClose) && duration <= 90) {
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
            if ($("#Duration").val() != "" && $("#StartTime").val() != "" && !isNaN($("#SittingSelect").val())) {
                $("#Submit").prop('disabled', false);
            }
            else {
                $("#Submit").prop('disabled', true);
            }
        })
    });

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

    function validateCustomerNum(available) {
        if ($("#CustomerNum").val() <= available) {
            return true;
        }
        return false;
    }
});