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
//have to loop through each form and submit "this" one that is clicked
$(".grocery-list-delete-item").each(function () {
    $(this).validate({
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
});

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