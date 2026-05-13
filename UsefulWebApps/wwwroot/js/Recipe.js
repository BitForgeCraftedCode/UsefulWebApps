$("#add-ingredient-form").validate({
    rules: {
        "AddIngredientToGrocery.GroceryItem": {
            required: true,
            minlength: 3,
            maxlength: 100
        },
        "AddIngredientToGrocery.Category": {
            required: true
        },
        "AddIngredientToGrocery.GroceryListId": {
            required: true
        }
    },
    submitHandler: function (form) {

        var formData = $(form).serialize();

        $.ajax({
            url: "/MyRecipes/AddIngredientToGroceryList",
            type: "POST",
            headers: {
                RequestVerificationToken:
                    $("#RequestVerificationToken").val()
            },
            data: formData,
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);

                    // clear inputs
                    $("#AddIngredientToGrocery_GroceryItem").val("");
                    //$("#AddIngredientToGrocery_Category").val("");
                }
                else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error("Error adding ingredient. Please try again.");
            }
        });
    }
});