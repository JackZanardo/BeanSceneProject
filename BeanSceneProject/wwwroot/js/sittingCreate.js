
$(document).ready(function () {
    $("#StartDate").attr("min", getDateString(new Date()));
    $("#CloseTime").attr("min", $("#OpenTime").val());

    $(function () {
        $("#StartDate").change(function () {
            if ($("#StartDate").val() == "") {
                $("#OpenTime").prop("disabled", true);
                $("#OpenTime").prop("disabled", true);
            }
            else {
                $("#OpenTime").prop("disabled", false);
                $("#OpenTime").prop("disabled", false);
            }
        });
    });


    $(function () {
        $("#OpenTime").change(function () {
            removeWarings();

            $("#CloseTime").attr("min", $("#OpenTime").val());

            let open = getDateTimeObj($("#OpenTime").val(), $("#StartDate").val());
            let close = getDateTimeObj($("#CloseTime").val(), $("#StartDate").val());
            let diff = (close.getTime() - open.getTime()) / 60000;
            if (diff <= 0) {
                $("#OpenTime").addClass("is-invalid");
                $("#OpenTimeHelp").text("Open time begins before close time");
            }
            else if (diff < 60) {
                $("#OpenTimeWarn").text("Sitting time less than 1 hour");
            }

        });
    });

    $(function () {
        $("#CloseTime").change(function () {
            removeWarings();

            let open = getDateTimeObj($("#OpenTime").val(), $("#StartDate").val());
            let close = getDateTimeObj($("#CloseTime").val(), $("#StartDate").val());
            let diff = (close.getTime() - open.getTime()) / 60000;
            if (diff <= 0) {
                $("#CloseTime").addClass("is-invalid");
                $("#CloseTimeHelp").text("Close time begins before open time");
            }
            else if (diff < 60) {
                $("#CloseTimeWarn").text("Sitting time less than 1 hour");
            }

        });
    });


    function getDateString(date) {
        var ds = date.getFullYear();
        ds += "-" + (date.getMonth() + 1);
        ds += "-" + date.getDate();
        return ds;
    }

    function addMinutes(date, minutes) {
        return new Date(date.getTime() + minutes * 60000);
    }

    function getDateTimeObj(time, date) {
        let dt = date + "T" + time;
        return new Date(dt);
    }

    function removeWarings() {
        $("#CloseTime").removeClass("is-invalid");
        $("#CloseTimeHelp").text("");
        $("#CloseTimeWarn").text("");
        $("#OpenTime").removeClass("is-invalid");
        $("#OpenTimeHelp").text("");
        $("#OpenTimeWarn").text("");
    }
});