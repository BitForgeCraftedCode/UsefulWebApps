$("#delete-all-to-do").validate({
    submitHandler: function (form) {
        //ask to confirm delete
        toastConfirm().then(deleteConfirm => {
            if (deleteConfirm == true) {
                form.submit();
            }
            else {
                toastr.success("List has NOT been deleted");
            }
        });
    }
});

$("#add-to-do-item").validate({
    rules: {
        //ToDoItem.ToDoItem is just the name of the html element
        "ToDoItem.ToDoItem": {
            required: true,
            minlength: 3,
            maxlength: 100,
            normalizer: function (value) {
                // Trim the value of the `field` element before
                // validating. this trims only the value passed
                // to the attached validators, not the value of
                // the element itself.
                return value.trim();
            }
        }
    },
    messages: {
        "ToDoItem.ToDoItem": {
            required: "To Do Item Is Required.",
            minlength: "Please Enter At Least 3 Characters.",
            maxlength: "No More Than 100 Characters."
        }
    },
    errorPlacement: function (error, element) {
        error.appendTo("#new-to-do-error");
    },
    errorElement: "span",
    submitHandler: function (form) {
        var formData = $(form).serialize();
        $.ajax({
            url: "/ListBuddy/ToDoListAddItem",
            type: "POST",
            data: formData,
            dataType: "html",
            success: function (response, status, xhr) {
                $("#to-do-list-container").empty();
                $("#to-do-list-container").append(response);
                $("#ToDoItem_ToDoItem").val("");

                // Sync the list version from the refreshed partial back to the add form
                var newVersion = $("#to-do-list-container").find("#version-in-partial").val();
                $("#version-add-form").val(newVersion);

                if (xhr.getResponseHeader("X-Concurrency-Conflict") === "true") {
                    toastr.warning("The list was updated by someone else. Your view has been refreshed — please try again.");
                }
            },
            error: function (request, status, error) {
                console.log(request.responseText);
                toastr.error("Add To Do Item Error. Please Try Again.");
            }
        });
    }
});
function validateDeleteToDo(deleteId) {
    //ask to confirm delete
    toastConfirm().then(deleteConfirm => {
        if (deleteConfirm == true) {
            deleteToDo(deleteId);
        }
        else {
            toastr.success("Item has NOT been deleted");
        }
    });
}

function deleteToDo(deleteId) {
    var formData = {
        id: deleteId
    };
    $.ajax({
        url: "/ListBuddy/ToDoListDeleteItem",
        type: "POST",
        headers: {
            RequestVerificationToken:
                $("#RequestVerificationToken")[0].value
        },
        data: formData,
        dataType: "json",
        success: function (response) {
            var obj = JSON.parse(response);
            $("#to-do-li-" + obj.deleteId).remove();
        },
        error: function (request, status, error) {
            console.log(request.responseText);
            toastr.error("Delete To Do Item Error. Please Try Again.");
        }
    });
}
function toggleComplete(id, listId) {
    var formData = {
        id: id,
        listId: listId
    };
    $.ajax({
        url: "/ListBuddy/ToDoListToggleComplete",
        type: "POST",
        headers: {
            RequestVerificationToken:
                $("#RequestVerificationToken")[0].value
        },
        data: formData,
        dataType: "html",
        success: function (response) {
            $("#to-do-list-container").empty();
            $("#to-do-list-container").append(response);
        },
        error: function (request, status, error) {
            console.log(request.responseText);
            toastr.error("Toggle Complete Error. Please Try Again.");
        }
    });
}

var loading = $('#spinner').hide();
$(document).on("ajaxStart", function () {
    //$("#spinner").show();
    loading.show();
});
$(document).on("ajaxStop", function () {
    //$("#spinner").hide();
    loading.hide();
});