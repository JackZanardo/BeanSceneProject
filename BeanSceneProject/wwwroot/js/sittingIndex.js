
$(document).ready(function () {


});

function UpdateIsClosed(value, id) {
    $.ajax({
        type: "POST",
        url: "Sitting/UpdateIsClosed",
        data: { "value": value, "id": id }
    });
}