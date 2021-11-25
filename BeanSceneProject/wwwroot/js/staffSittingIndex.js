$(document).ready(function () {
    $("#date").attr("min", getDateString(new Date()));
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