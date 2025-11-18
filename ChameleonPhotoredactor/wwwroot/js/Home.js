document.addEventListener("DOMContentLoaded", function () {
    const uploadBtn = document.getElementById("uploadBtn");
    const fileInput = document.getElementById("fileInput");
    const form = fileInput ? fileInput.closest("form") : null;

    if (uploadBtn && fileInput && form) {
        // When the visible button is clicked...
        uploadBtn.addEventListener("click", function () {
            // ...trigger a click on the hidden file input.
            fileInput.click();
        });

        // When a file is selected in the hidden input...
        fileInput.addEventListener("change", function () {
            // ...automatically submit the form.
            if (fileInput.files.length > 0) {
                form.submit();
            }
        });
    }
});