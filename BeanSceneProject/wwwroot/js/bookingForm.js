

$(document).ready(function () {
    var availableDates = $("#StartDates").val();

    $(function () {
        $("#StartDate").change(function () {
            $("#SittingSelect").children('option:not(:first)').remove();
            $.ajax({
                type: "POST",
                url: "GetSittings",
                data: { "jsonDate": $("#StartDate").val() },
                success: function (sessions) {
                    $.each(JSON.parse(sessions), function (i, session) {
                        var o = new Option(session.InfoText, session.Id);
                        $(o).html(session.InfoText);
                        $("#SittingSelect").append(o);
                    });
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


});