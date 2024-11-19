$("#delete-all-to-do").validate({
    submitHandler: function (form) {
        //ask to confirm delete
        toastConfirm().then(deleteConfirm => {
            if (deleteConfirm == true) {
                form.submit();
            }
            else {
                toastr.success("Item has NOT been deleted");
            }
        });
    }
});

$("#add-to-do-item").validate({
    rules: {
        //ToDoList.ToDoItem is just the name of the html element
        "ToDoList.ToDoItem": {
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
        "ToDoList.ToDoItem": {
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
            url: "/ListBuddy/ToDoListCreate",
            type: "POST",
            data: formData,
            dataType: "html",
            success: function (response) {
                $("#to-do-list-container").empty();
                $("#to-do-list-container").append(response);
                $("#ToDoList_ToDoItem").val("");
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
function toggleComplete(toggleId) {
    var formData = {
        id: toggleId
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