(() => {
    const state = {
        map: null,
        marker: null,
        routeLayer: null,
        tileLayer: null
    };

    const carIcon = L.divIcon({
        className: 'motogps-car-icon',
        html: '<svg viewBox="0 0 24 24" width="32" height="32">'
            + '<path d="M12 2 L20 20 L12 16 L4 20 Z" fill="#4aa3ff" stroke="#0a0a0a" stroke-width="1.5" stroke-linejoin="round"/>'
            + '</svg>',
        iconSize: [32, 32],
        iconAnchor: [16, 16]
    });

    window.MotoGpsMap = {
        init(elementId, lat, lon) {
            if (state.map) {
                state.map.remove();
                state.map = null;
            }

            const map = L.map(elementId, {
                center: [lat, lon],
                zoom: 16,
                zoomControl: false,
                attributionControl: false
            });

            state.tileLayer = L.tileLayer(
                'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png',
                { maxZoom: 19, subdomains: 'abcd' }
            ).addTo(map);

            state.marker = L.marker([lat, lon], { icon: carIcon, rotationAngle: 0 }).addTo(map);
            state.map = map;
        },

        updatePosition(lat, lon, heading) {
            if (!state.map || !state.marker) return;

            state.marker.setLatLng([lat, lon]);

            const el = state.marker.getElement();
            if (el && Number.isFinite(heading)) {
                el.style.transformOrigin = 'center';
                el.style.setProperty('--motogps-rotation', heading + 'deg');
                const svg = el.querySelector('svg');
                if (svg) svg.style.transform = 'rotate(' + heading + 'deg)';
            }

            state.map.panTo([lat, lon], { animate: true, duration: 0.4 });
        },

        drawRoute(coords) {
            if (!state.map) return;
            this.clearRoute();
            if (!coords || coords.length === 0) return;

            const latlngs = coords.map(c => [c[1], c[0]]); // GeoJSON: [lon, lat]
            state.routeLayer = L.polyline(latlngs, {
                color: '#4aa3ff',
                weight: 5,
                opacity: 0.9,
                lineCap: 'round',
                lineJoin: 'round'
            }).addTo(state.map);

            state.map.fitBounds(state.routeLayer.getBounds(), { padding: [40, 40] });
        },

        clearRoute() {
            if (state.routeLayer && state.map) {
                state.map.removeLayer(state.routeLayer);
            }
            state.routeLayer = null;
        },

        dispose() {
            if (state.map) {
                state.map.remove();
                state.map = null;
                state.marker = null;
                state.routeLayer = null;
            }
        }
    };
})();
