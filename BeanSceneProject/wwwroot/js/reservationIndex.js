
$(document).ready(function () {


});

function UpdateStatus(value, id) {
    $.ajax({
        type: "POST",
        url: "/Staff/StaffReservation/UpdateStatus",
        data: { "value": value, "id": id },
        success: function (data) {
            if (!data) {
                location.reload(true);
                $("#warning" + id).text("Status update failed");
            }
            else {
                $("#warning" + id).text("");
            }
        },
        failure: function () {
            $("#warning" + id).text("Status update failed");
        },
        error: function () {
            $("#warning" + id).text("Status update failed");
        }
    });
}