$(document).ready(function () {
    $.validator.addMethod("validDropDownList", function (value, element) {
        return (value != "0");
    }, "");

    $("#SearchResultsForm").validate({
        rules: {
            ManufacturerId: "validDropDownList",
            Model: "validDropDownList",
            Year: "validDropDownList",
            CustomerName: "required",
            CustomerPhone: {
                required: true,
                number: true
            }
        },
        messages: {
            ManufacturerId: "Chọn hãng xe",
            Model: "Chọn dòng xe",
            Year: "Chọn năm",
            CustomerName: "Nhập tên của bạn",
            CustomerPhone: {
                required: "Nhập số điện thoại",
                number: "Vui lòng nhập số"
            }
        },
        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#pu-manufacturer").change(function () {
        loadModelByManufacturer($(this).val());
        $("#ManufacturerName").val($(this).find("option:selected").text());
    });

    $("#pu-model").change(function () {
        loadYearByModel($(this).val());
        $("#ModelName").val($(this).find("option:selected").text());
    });

    $("#pu-year").change(function () {
        $("#YearName").val($(this).find("option:selected").text());
    });

    $(".btnAlert").trigger("click");
});

function showAlert() {
    var msg = $("#hidMessage").val();
    if (msg.length > 0 || msg != "")
        $.notify(msg, "success");
        //$.notify(msg, { align: "center", delay: 5000, icon: "bell", type: "success" });
        //window.notificationSuccess(msg);
}

function loadModelByManufacturer(manufacturerId) {
    $.get("/Service/Service/LoadModelByManufacturer", { manufacturerId: manufacturerId }, function (response) {
        var selectHtml = "<option value='0'>Chọn dòng xe</option>";
        if (response.data != "") {
            $.each(response.data, function (index, d) {
                selectHtml += "<option value='" + d.Id + "'>" + d.Name + "</option>";
            });
        }
        $("#pu-model").html(selectHtml);
    });
}

function loadYearByModel(modelId) {
    $.get("/Service/Service/LoadYearByModel", { modelId: modelId }, function (response) {
        var selectHtml = "<option value='0'>Chọn năm</option>";
        if (response.data != "") {
            $.each(response.data, function (index, d) {
                selectHtml += "<option value='" + d.Id + "'>" + d.Name + "</option>";
            });
        }
        $("#pu-year").html(selectHtml);
    });
}