$(document).ready(function () {
    //validate form using http://formvalidation.io/api/
    $("#createServiceForm").formValidation({
        framework: "bootstrap",
        icon: {
            valid: "glyphicon glyphicon-ok",
            invalid: "glyphicon glyphicon-remove",
            validating: "glyphicon glyphicon-refresh"
        },
        fields: {
            Title: {
                validators: {
                    notEmpty: {
                        message: "Nhập tiêu đề!"
                    }
                }
            },
            Description: {
                validators: {
                    notEmpty: {
                        message: "Nhập mô tả!"
                    }
                }
            }
        }
    })
    .on('success.field.fv', function (e, data) {
        if (data.fv.getInvalidFields().length > 0)
            data.fv.disableSubmitButtons(true);
        else
            data.fv.disableSubmitButtons(false);
            
    });

    var editForm = $("#editServiceForm").formValidation({
        framework: "bootstrap",
        icon: {
            valid: "glyphicon glyphicon-ok",
            invalid: "glyphicon glyphicon-remove",
            validating: "glyphicon glyphicon-refresh"
        },
        fields: {
            Title: {
                validators: {
                    notEmpty: {
                        message: "Nhập tiêu đề!"
                    }
                }
            },
            Description: {
                validators: {
                    notEmpty: {
                        message: "Nhập mô tả!"
                    }
                }
            }
        }
    })
    .on('success.field.fv', function (e, data) {
        if (data.fv.getInvalidFields().length > 0)
            data.fv.disableSubmitButtons(true);
        else
            data.fv.disableSubmitButtons(false);
    });    

    $("#btnCreate").click(function () {
        submitValidatedForm(createForm);
    });
    
    $("#btnEdit").click(function () {
        submitValidatedForm(editForm);
    });

    //grid
    $(".form-service").on("ajaxSuccess", function (e, response) {
        $('#grid-service').bootgrid('reload');
    })

    var url = $("#grid-service").data('url');
    var editUrl = $("#grid-service").data('edit-url');
    var deleteUrl = $("#grid-service").data('delete-url');
    var gridService = $("#grid-service").bootgrid({
        ajax: true,
        post: function () {
            return {
                data: "somedatabase"
            };
        },
        url: url,
        formatters: {
            "commands": function (column, row) {
                var generateEdit = "<a href=\"" + editUrl + "/" + row.Id + "\" class=\"\" style=\"margin-right: 20px;\"><span class=\"fa fa-pencil\"></span></a>";
                var generateDelete = "<a href=\"" + deleteUrl + "/" + row.Id + "\" class=\"ajax-post\"><span class=\"fa fa-trash-o\"></span></a>";
                return generateEdit + generateDelete;
            }
        }
    }).on("loaded.rs.jquery.bootgrid", function () {

        var message = gridService.data("delete-message");

        Ajax.handleAjaxLink(gridService, "a.ajax-post", message);

        $("a.ajax-post").on("ajaxSuccess", function (e, response) {
            if (response.success)
                gridService.bootgrid("reload");
        });
    });
});

function submitValidatedForm(form) {
    var serviceFormValidation = form.data("formValidation"), // FormValidation instance

    // Validate the container
    $container = $(".form-container");
    serviceFormValidation.validateContainer($container);

    //Check if container is valid
    var isValid = serviceFormValidation.isValidContainer($container);

    if (isValid === false || isValid === null)
        return false;

    form.submit();
    return true;
}

