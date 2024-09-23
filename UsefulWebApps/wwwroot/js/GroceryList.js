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