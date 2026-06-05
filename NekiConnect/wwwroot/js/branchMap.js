// wwwroot/js/branchMap.js

window.branchMap = (() => {
    const maps = {};   // keyed by elementId so multiple maps can coexist

    function init(elementId, lat, lng, popupText) {
        // Destroy old instance if re-initialising
        if (maps[elementId]) {
            maps[elementId].remove();
            delete maps[elementId];
        }

        const map = L.map(elementId, { zoomControl: true, scrollWheelZoom: false })
            .setView([lat, lng], 14);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
            maxZoom: 19
        }).addTo(map);

        if (popupText) {
            L.marker([lat, lng])
                .addTo(map)
                .bindPopup(popupText)
                .openPopup();
        }

        maps[elementId] = map;
    }

    // Move existing map to new coords without full re-init
    function update(elementId, lat, lng, popupText) {
        const map = maps[elementId];
        if (!map) { init(elementId, lat, lng, popupText); return; }

        map.setView([lat, lng], 14);
        // Remove old markers
        map.eachLayer(l => { if (l instanceof L.Marker) map.removeLayer(l); });
        if (popupText) {
            L.marker([lat, lng]).addTo(map).bindPopup(popupText).openPopup();
        }
    }

    // Show all NGO branches on one overview map
    function initOverview(elementId, branches) {
        if (maps[elementId]) {
            maps[elementId].remove();
            delete maps[elementId];
        }

        // Default centre on Pakistan
        const map = L.map(elementId, { scrollWheelZoom: true })
            .setView([30.3753, 69.3451], 5);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
            maxZoom: 19
        }).addTo(map);

        if (!branches || branches.length === 0) return;

        const group = L.featureGroup();
        branches.forEach(b => {
            L.marker([b.latitude, b.longitude])
                .addTo(map)
                .bindPopup(`<strong>${b.branchName}</strong><br>${b.address || ''}, ${b.city}`)
                .addTo(group);
        });
        group.addTo(map);

        // Zoom map to fit all markers
        map.fitBounds(group.getBounds().pad(0.3));
        maps[elementId] = map;
    }

    function destroy(elementId) {
        if (maps[elementId]) {
            maps[elementId].remove();
            delete maps[elementId];
        }
    }

    return { init, update, initOverview, destroy };
})();