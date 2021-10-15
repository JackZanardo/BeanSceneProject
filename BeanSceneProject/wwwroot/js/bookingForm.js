//const { error } = require("jquery");


$(document).ready(function () {
    var availableDates = $("#StartDates").val();

    $(function () {
        $("#StartDate").change(function () {
            $("#StartDate").removeClass("is-invalid")
            $("#SittingSelect").prop('disabled', true);
            $("#SittingSelect").find('option:not(:first)').remove();
            $("#StartTime").find('option:not(:first)').remove();
            $("#StartTime").prop('disabled', true);
            $.ajax({
                type: "POST",
                url: "GetSittings",
                data: { "jsonDate": $("#StartDate").val() },
                success: function (sessions) {
                    $.each(JSON.parse(sessions), function (i, session) {
                        var o = new Option(session.InfoText, session.Id);
                        $(o).html(session.InfoText);
                        $("#SittingSelect").append(o);
                        $("#SittingSelect").prop('disabled', false);
                    });
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
            if (!isNaN($("#SittingSelect").val())) {
                $.ajax({
                    type: "POST",
                    url: "GetStartTimes",
                    data: { "sittingId": $("#SittingSelect").val() },
                    success: function (sessions) {
                        $.each(JSON.parse(sessions), function (i, session) {
                            var o = new Option(session.InfoText, session.Start);
                            $(o).html(session.InfoText);
                            $("#StartTime").append(o);
                            $("#StartTime").prop('disabled', false);
                        });
                    }
                });
            }
        });
    });


});