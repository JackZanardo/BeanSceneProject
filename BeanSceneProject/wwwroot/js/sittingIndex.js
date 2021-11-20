
$(document).ready(function () {

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

function UpdateIsClosed(value, id) {
    $.ajax({
        type: "POST",
        url: "Sitting/UpdateIsClosed",
        data: { "value": value, "id": id },
        success: function (data) {
            if (!data) {
                location.reload(true);
                $("#sittingRadio" + id).text("Status update failed");
            }
            else {
                $("#sittingRadio" + id).text("");
            }
        },
        failure: function () {
            location.reload(true);
            $("#sittingRadio" + id).text("Status update failed");
        },
        error: function () {
            location.reload(true);
            $("#sittingRadio" + id).text("Status update failed");
        }
        
    });
}