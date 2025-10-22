document.addEventListener("DOMContentLoaded", function () {
    const uploadBthn = document.getElementById("uploadBtn");
    const fileInput = document.getElementById("fileInput");

    if (uploadBthn && fileInput) {
        uploadBthn.addEventListener("click", () => fileInput.click());
        fileInput.addEventListener("change", () => {
            if (fileInput.files.length > 0) {
                fileInput.closest("form").submit();
            }
        });
    }
});