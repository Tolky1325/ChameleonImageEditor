function selectImage(element) {
    // 1. Remove 'selected' class from all other items
    const allItems = document.querySelectorAll('.library_item_container');
    allItems.forEach(item => {
        item.classList.remove('selected-photo');
        item.style.border = "none"; // Reset inline style if any
    });

    // 2. Add 'selected' class to the clicked element
    element.classList.add('selected-photo');

    // Optional: Add a direct border style if you haven't updated CSS yet
    element.style.border = "4px solid #ffcc5c";
    element.style.borderRadius = "12px";

    // 3. Get the image tag inside the clicked div
    const img = element.querySelector('img');

    // 4. Draw the histogram using the function from editor.js
    if (img && typeof drawHistogram === "function") {
        // We pass "" as the filter string because library images 
        // usually don't have CSS filters applied directly in the grid.
        drawHistogram(img, "");
    } else {
        console.error("drawHistogram function not found. Make sure editor.js is loaded.");
    }
}