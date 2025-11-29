document.addEventListener("DOMContentLoaded", function () {
    const img = document.getElementById("editable_image");
    if (!img) return;

    if (img.complete) drawHistogram(img);
    else img.onload = () => drawHistogram(img);
});

function drawHistogram(img) {
    const canvas = document.getElementById("histogramCanvas");
    if (!canvas) return;
    const ctx = canvas.getContext("2d");

    const tempCanvas = document.createElement("canvas");
    const tctx = tempCanvas.getContext("2d");
    tempCanvas.width = img.naturalWidth;
    tempCanvas.height = img.naturalHeight;
    tctx.drawImage(img, 0, 0);

    let imageData;
    try { imageData = tctx.getImageData(0, 0, tempCanvas.width, tempCanvas.height); }
    catch (e) { console.error(e); return; }

    const data = imageData.data;
    const histogramR = new Array(256).fill(0);
    const histogramG = new Array(256).fill(0);
    const histogramB = new Array(256).fill(0);

    for (let i = 0; i < data.length; i += 4) {
        histogramR[data[i]]++;
        histogramG[data[i + 1]]++;
        histogramB[data[i + 2]]++;
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    const maxValue = Math.max(...histogramR, ...histogramG, ...histogramB);
    const barWidth = Math.ceil(canvas.width / 256);

    for (let i = 0; i < 256; i++) {
        const rH = (histogramR[i] / maxValue) * canvas.height;
        const gH = (histogramG[i] / maxValue) * canvas.height;
        const bH = (histogramB[i] / maxValue) * canvas.height;

        ctx.fillStyle = "rgba(255,0,0,0.7)";
        ctx.fillRect(i * barWidth, canvas.height - rH, barWidth, rH);
        ctx.fillStyle = "rgba(0,255,0,0.7)";
        ctx.fillRect(i * barWidth, canvas.height - gH, barWidth, gH);
        ctx.fillStyle = "rgba(0,0,255,0.7)";
        ctx.fillRect(i * barWidth, canvas.height - bH, barWidth, bH);
    }
}
