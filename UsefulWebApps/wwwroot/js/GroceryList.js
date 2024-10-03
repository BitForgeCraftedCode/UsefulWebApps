$("#delete-all-grocery-list").validate({
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

function validateDeleteGroceryItem(deleteId, category) {
    //ask to confirm delete
    toastConfirm().then(deleteConfirm => {
        if (deleteConfirm == true) {
            deleteGroceryItem(deleteId, category);
        }
        else {
            toastr.success("Item has NOT been deleted");
        }
    });
}

function deleteGroceryItem(deleteId, category) {
    var formData = {
        id: deleteId
    };
    $.ajax({
        url: "/ListBuddy/GroceryListDeleteItem",
        type: "POST",
        data: formData,
        dataType: "json",
        success: function (response) {
            var obj = JSON.parse(response);
            //remove the list item and if no more item in list remove the Category section
            $("#grocery-list-item-li-" + obj.deleteId).remove();
            if ($("#ul-" + category).children().length == 0) {
                $("#section-" + category).remove();
            }
        },
        error: function (request, status, error) {
            console.log(request.responseText);
            toastr.error("Delete To Do Item Error. Please Try Again.");
        }
    });
}

function toggleComplete(toggleId, userID) {
    var formData = {
        id: toggleId,
        userId: userID
    };
    $.ajax({
        url: "/ListBuddy/GroceryListToggleComplete",
        type: "POST",
        data: formData,
        dataType: "html",
        success: function (response) {
            $("#grocery-list-container").empty();
            $("#grocery-list-container").append(response);
        },
        error: function (request, status, error) {
            console.log(request.responseText);
            toastr.error("Toggle Complete Error. Please Try Again.");
        }
    });
}

$("#add-grocery-list-item").validate({
    rules: {
        //GroceryList.GroceryItem is just the name of the html element
        "GroceryList.GroceryItem": {
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
        "GroceryList.GroceryItem": {
            required: "Grocery Item Is Required.",
            minlength: "Please Enter At Least 3 Characters.",
            maxlength: "No More Than 100 Characters."
        }
    },
    errorPlacement: function (error, element) {
        error.appendTo("#new-grocery-item-error");
    },
    errorElement: "span",
    submitHandler: function (form) {
        var formData = $(form).serialize();
        $.ajax({
            url: "/ListBuddy/GroceryListCreate",
            type: "POST",
            data: formData,
            dataType: "html",
            success: function (response) {
                $("#grocery-list-container").empty();
                $("#grocery-list-container").append(response);
                $("#GroceryList_GroceryItem").val("");
            },
            error: function (request, status, error) {
                console.log(request.responseText);
                toastr.error("Add Grocery List Item Error. Please Try Again.");
            }
        });
    }
});