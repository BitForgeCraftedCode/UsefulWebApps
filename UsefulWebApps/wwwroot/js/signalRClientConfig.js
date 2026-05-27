const connection = new signalR.HubConnectionBuilder()
    .withUrl("/appHub")
    .build();

connection.on("ReceiveNotification", function (message) {

    toastr.options = {
        "closeButton": true,
        "progressBar": true,
        "positionClass": "toast-top-right"
    };

    toastr.info(message);

    incrementNotificationBadge();
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function incrementNotificationBadge() {
    const badge = document.getElementById("notification-count");

    if (!badge) return;

    let current = parseInt(badge.innerText);

    if (isNaN(current))
        current = 0;

    badge.innerText = current + 1;
}