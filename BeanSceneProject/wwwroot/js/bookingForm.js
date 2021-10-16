//const { error } = require("jquery");


$(document).ready(function () {
    var availableDates = $("#StartDates").val();
    var sessions = "";

    $(function () {
        $("#StartDate").change(function () {
            $("#StartDate").removeClass("is-invalid")
            $("#SittingSelect").prop('disabled', true);
            $("#SittingSelect").find('option:not(:first)').remove();
            $("#StartTime").find('option:not(:first)').remove();
            $("#StartTime").prop('disabled', true);
            sessions = ""
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
            sittingId = "";
            if (!isNaN($("#SittingSelect").val())) {
                let sittingId = $("#SittingSelect").val();
                let session = sessions.find(s => s.Id === sittingId);
                let interval = Date.parse(session.SittingOpen);
                while (interval < Date.parse(sitting.SittingClose)) {
                    var o = new Option(formatAMPM(interval), interval);
                    $(o).html(formatAMPM(interval));
                    $("#StartTime").append(o);
                    interval = addMinutes(interval, 15);
                }
                $("#StartTime").prop('disabled', false);
                $.ajax({
                    type: "POST",
                    url: "GetStartTimes",
                    data: { "sittingId": $("#SittingSelect").val() },
                    success: function (sessions) {
                        $.each(JSON.parse(sessions), function (i, session) {
                            var o = new Option(session.InfoText, session.Start);
                            $(o).html(session.InfoText);
                            $("#StartTime").append(o);    
                        });
                    }
                });
            }
        });
    });

    $(function () {
        $("#StartTime").change(function () {
            $("#Duration").find('option:not(:first)').remove();
            $("#Duration").prop('disabled', true);
            if ($("#StartTime").val() != "") {
                let sittingId = $("#SittingSelect").val();
                let session = sessions.find(s => s.Id === sittingId);
                let interval = $("#StartTime").val();

            }
        });
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
});