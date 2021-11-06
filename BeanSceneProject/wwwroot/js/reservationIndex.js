
$(document).ready(function () {


});

function UpdateStatus(value, id) {
    $.ajax({
        type: "POST",
        url: "/Staff/StaffReservation/UpdateStatus",
        data: { "value": value, "id": id }
    });
}