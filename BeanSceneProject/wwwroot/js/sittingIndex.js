
$(document).ready(function () {


});

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