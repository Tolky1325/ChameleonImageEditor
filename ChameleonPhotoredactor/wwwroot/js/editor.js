document.addEventListener("DOMContentLoaded", function () {
    const img = document.getElementById("editable_image");
    if (!img) return;

});

document.querySelectorAll('.slider-group input[type="range"]')
    .forEach(slider => {
        const updateTrack = () => {
            const min = slider.min ? Number(slider.min) : 0;
            const max = slider.max ? Number(slider.max) : 100;
            const value = Number(slider.value);

            const percent = ((value - min) / (max - min)) * 100;

            slider.style.setProperty('--value', `${percent}%`);
        };

        slider.addEventListener('input', updateTrack);
        updateTrack();
    });



function drawHistogram(img, filterString = "") {
    const canvas = document.getElementById("histogramCanvas");
    if (!canvas) return;
    const ctx = canvas.getContext("2d");

    const tempCanvas = document.createElement("canvas");
    const tctx = tempCanvas.getContext("2d");

    tempCanvas.width = img.naturalWidth;
    tempCanvas.height = img.naturalHeight;

    if (filterString) {
        tctx.filter = filterString;
    }

    tctx.drawImage(img, 0, 0);

    let imageData;
    try {
        imageData = tctx.getImageData(0, 0, tempCanvas.width, tempCanvas.height);
    } catch (e) {
        console.error("Cannot get image data (CORS issue?):", e);
        return;
    }

    const data = imageData.data;
    const histogram = new Array(256).fill(0);

    for (let i = 0; i < data.length; i += 4) {
        const alpha = data[i + 3];

        if (alpha < 10) continue;

        const r = data[i];
        const g = data[i + 1];
        const b = data[i + 2];

        const luminance = Math.round(0.299 * r + 0.587 * g + 0.114 * b);

        if (luminance >= 0 && luminance < 256) {
            histogram[luminance]++;
        }
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    const maxValue = Math.max(...histogram);
    const barWidth = canvas.width / 256;

    ctx.fillStyle = "rgba(200, 200, 200, 0.8)";

    for (let i = 0; i < 256; i++) {
        const percent = maxValue > 0 ? (histogram[i] / maxValue) : 0;
        const barHeight = percent * canvas.height;

        if (barHeight > 0) {
            ctx.fillRect(i * barWidth, canvas.height - barHeight, barWidth, barHeight);
        }
    }
}