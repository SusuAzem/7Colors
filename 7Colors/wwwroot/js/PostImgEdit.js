$(function () {
    var ids = [];
    $('form input.btn').on('click', function () {
        var checkbox_value = "";
        $(":checkbox").each(function () {
            var ischecked = $(this).is(":checked");
            if (ischecked) {
                checkbox_value = $(this).val();
                ids.push(checkbox_value);
            }
        });
        // var obj = JSON.stringify(allVals);
        alert(ids);
        $.ajax({
            type: "POST",
            url: "/Admin/Posts/EditPostImg",
            data: ids,
            contentType: "application/json; charset=utf-8",
            dataType: 'JSON',
            traditional: true,
            success: function (response) {
                alert("OK! Data [" + allVals + "] Sent with Response:" + response);
                console.log("OK! Data [" + allVals + "] Sent with Response:" + response);
            },
            error: function (e) {
                alert("OH NOES! Data[" + allVals + "] Not sent with Error:" + e);
                console.error("OH NOES! Data[" + allVals + "] Not sent with Error:" + e);
            }
        });
    });
});