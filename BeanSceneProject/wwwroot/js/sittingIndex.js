
$(document).ready(function () {


});

function UpdateIsClosed(value, id) {
    $.ajax({
        type: "POST",
        url: "Sitting/UpdateIsClosed",
        data: { "value": value, "id": id },
        success: function (data) {
            let IsSucceeded = (data === 'true');
            if (!IsSucceeded) {
                $("#sittingRadio" + id).text("Status update failed");
                if (value) {
                    $("#sitting" + id + "radioOpen").attr("checked", true);
                    $("#sitting" + id + "radioClose").attr("checked", false);
                }
                else {
                    $("#sitting" + id + "radioOpen").attr("checked", false);
                    $("#sitting" + id + "radioClose").attr("checked", true);
                }
            }
            else {
                $("#sittingRadio" + id).text("");
            }
        },
        failure: function () {
            $("#sittingRadio" + id).text("Status update failed");
            if (value) {
                $("#sitting" + id + "radioOpen").attr("checked", true);
                $("#sitting" + id + "radioClose").attr("checked", false);
            }
            else {
                $("#sitting" + id + "radioOpen").attr("checked", false);
                $("#sitting" + id + "radioClose").attr("checked", true);
            }
        },
        error: function () {
            $("#sittingRadio" + id).text("Status update failed");
            if (value) {
                $("#sitting" + id + "radioOpen").attr("checked", true);
                $("#sitting" + id + "radioClose").attr("checked", false);
            }
            else {
                $("#sitting" + id + "radioOpen").attr("checked", false);
                $("#sitting" + id + "radioClose").attr("checked", true);
            }
        }
        
    });
}