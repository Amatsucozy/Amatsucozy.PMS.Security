// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function createOverlay() {
    const overlay = document.createElement("div");
    overlay.classList.add("overlay");
    overlay.id = "overlay";

    return overlay;
}

function getOverlay() {
    let overlay = document.getElementById("overlay");

    if (!overlay) {
        overlay = createOverlay()
        document.body.appendChild(overlay);
    }

    return overlay;
}

function createDialog(title, message) {
    const dialog = document.createElement("div");
    dialog.classList.add("dialog-container");

    const titleElement = document.createElement("h2");
    titleElement.innerText = title;

    const messageElement = document.createElement("p");
    messageElement.innerText = message;

    const footerElement = document.createElement("div");
    footerElement.classList.add("dialog-footer");

    const confirmButton = document.createElement("button");
    confirmButton.classList.add("button-primary");
    confirmButton.classList.add("dialog-confirm-button");
    confirmButton.value = "Ok";
    confirmButton.innerText = "OK";
    confirmButton.addEventListener("click", (event) => {
        const dialog = event.target.closest(".dialog-container");
        dialog?.remove();

        const overlay = getOverlay();

        if (!overlay.hasChildNodes()) {
            overlay.remove();
        }
    });

    footerElement.appendChild(confirmButton);
    dialog.appendChild(titleElement);
    dialog.appendChild(messageElement);
    dialog.appendChild(footerElement);

    return dialog;
}

document.addEventListener("open-dialog", (event) => {
    const overlay = getOverlay();
    const dialog = createDialog(event.detail.title, event.detail.message);
    overlay.appendChild(dialog);
});

function openInformationDialog(title, message) {
    const event = new CustomEvent("open-dialog", {
        detail: {
            title: title,
            message: message
        }
    });

    document.dispatchEvent(event);
}
