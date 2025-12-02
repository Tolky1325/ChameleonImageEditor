function selectImage(element) {
    const allItems = document.querySelectorAll('.library_item_container');
    allItems.forEach(item => {
        item.classList.remove('selected-photo');
        item.style.border = "none";
    });

    element.classList.add('selected-photo');
    element.style.border = "4px solid #ffcc5c";
    element.style.borderRadius = "16px";

    const imageId = element.getAttribute('data-id');
    console.log("Selected Image ID:", imageId);
    //^^no need but for timebeing it`ll be here

    const editBtn = document.getElementById('library-edit-btn');
    const exportBtn = document.getElementById('library-export-btn');
    const deleteBtn = document.getElementById('library-delete-btn');

    if (editBtn && imageId) {
        editBtn.href = '/FullEditor/FullEditor/' + imageId;
    }

    if (exportBtn && imageId) {
        exportBtn.href = '/Export/Export/' + imageId;
    }
    /*
    if (deleteBtn && imageId) {
        deleteBtn.href = '/FullEditor/DeleteImage/' + imageId;
    }
    */
   //^^implement smth later
    const img = element.querySelector('img');
    if (img && typeof drawHistogram === "function") {
        drawHistogram(img, "");
    }
}