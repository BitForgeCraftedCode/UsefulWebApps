// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//https://stackoverflow.com/questions/61499672/return-true-or-false-values-according-to-buttons-pressed-toastr-js-no-an
function toastConfirm() {
    return new Promise((resolve, reject) => {
        toastr.warning("Are you sure you want to delete?<br/><br/><button class='btn' id='deleteYes'>Yes</button><br/><br/><button class='btn' id='deleteNo'>No</button>", 'Delete Confirm', {
            closeButton: false,
            allowHtml: true,
            timeOut: 0,
            extendedTimeOut: 0,
            onShown: function (toast) {
                $("#deleteYes").on("click", function () {
                    resolve(true)
                });
                $("#deleteNo").on("click", function () {
                    resolve(false)
                });
            }
        });
    });
}

function toastConfirmUseSavedList() {
    return new Promise((resolve, reject) => {
        toastr.warning("Are you sure you want to use a saved list?<br/><br/><button class='btn' id='useSavedYes'>Yes</button><br/><br/><button class='btn' id='useSavedNo'>No</button>", 'Use Saved List Confirm', {
            closeButton: false,
            allowHtml: true,
            timeOut: 0,
            extendedTimeOut: 0,
            onShown: function (toast) {
                $("#useSavedYes").on("click", function () {
                    resolve(true)
                });
                $("#useSavedNo").on("click", function () {
                    resolve(false)
                });
            }
        });
    });
}