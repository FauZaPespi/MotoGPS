(() => {
    const state = {
        map: null,
        marker: null,
        routeLayer: null,
        tileLayer: null,
        radarMarkers: []
    };

    const carIcon = L.divIcon({
        className: 'motogps-car-icon',
        html: '<svg viewBox="0 0 24 24" width="36" height="36">'
            + '<path d="M12 2 L20 20 L12 16 L4 20 Z" fill="#4aa3ff" stroke="#ffffff" stroke-width="1.5" stroke-linejoin="round"/>'
            + '</svg>',
        iconSize: [36, 36],
        iconAnchor: [18, 18]
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
                attributionControl: true
            });

            state.tileLayer = L.tileLayer(
                'https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png',
                {
                    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> &copy; <a href="https://carto.com/">CARTO</a>',
                    subdomains: 'abcd',
                    maxZoom: 20
                }
            ).addTo(map);

            state.marker = L.marker([lat, lon], { icon: carIcon }).addTo(map);
            state.map = map;
        },

        updatePosition(lat, lon, heading) {
            if (!state.map || !state.marker) return;

            state.marker.setLatLng([lat, lon]);

            const el = state.marker.getElement();
            if (el && Number.isFinite(heading)) {
                const svg = el.querySelector('svg');
                if (svg) svg.style.transform = 'rotate(' + heading + 'deg)';
            }

            state.map.panTo([lat, lon], { animate: true, duration: 0.4 });
        },

        updateRadars(positions) {
            if (!state.map) return;

            state.radarMarkers.forEach(m => state.map.removeLayer(m));
            state.radarMarkers = [];

            if (!positions || positions.length === 0) return;

            positions.forEach(p => {
                const m = L.circleMarker([p.latitude, p.longitude], {
                    radius: 9,
                    fillColor: '#ff3333',
                    color: '#ffffff',
                    weight: 2,
                    opacity: 1,
                    fillOpacity: 0.85
                });
                m.bindTooltip('Radar', { permanent: false, direction: 'top' });
                m.addTo(state.map);
                state.radarMarkers.push(m);
            });
        },

        drawRoute(coords) {
            if (!state.map) return;
            this.clearRoute();
            if (!coords || coords.length === 0) return;

            const latlngs = coords.map(c => [c[1], c[0]]);
            state.routeLayer = L.polyline(latlngs, {
                color: '#4aa3ff',
                weight: 6,
                opacity: 0.95,
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
                state.radarMarkers = [];
            }
        }
    };
})();
