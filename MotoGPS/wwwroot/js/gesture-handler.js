(() => {
    const SWIPE_THRESHOLD_PX = 80;
    let dotNetRef = null;
    let startY = null;
    let startX = null;
    let tracking = false;

    function isInBottomHalf(y) {
        return y >= window.innerHeight * 0.5;
    }

    function onPointerDown(e) {
        if (!isInBottomHalf(e.clientY)) return;
        startY = e.clientY;
        startX = e.clientX;
        tracking = true;
    }

    function onPointerMove(e) {
        if (!tracking || startY == null) return;
        // Cancel tracking if horizontal movement dominates.
        if (Math.abs(e.clientX - startX) > Math.abs(e.clientY - startY)) {
            tracking = false;
        }
    }

    function onPointerUp(e) {
        if (!tracking || startY == null) {
            tracking = false;
            startY = null;
            return;
        }

        const delta = startY - e.clientY;
        tracking = false;
        startY = null;

        if (delta > SWIPE_THRESHOLD_PX && dotNetRef) {
            dotNetRef.invokeMethodAsync('OnSwipeUp');
        } else if (delta < -SWIPE_THRESHOLD_PX && dotNetRef) {
            dotNetRef.invokeMethodAsync('OnSwipeDown');
        }
    }

    window.MotoGpsGestures = {
        register(reference) {
            dotNetRef = reference;
            window.addEventListener('pointerdown', onPointerDown, { passive: true });
            window.addEventListener('pointermove', onPointerMove, { passive: true });
            window.addEventListener('pointerup', onPointerUp, { passive: true });
            window.addEventListener('pointercancel', onPointerUp, { passive: true });
        },
        unregister() {
            dotNetRef = null;
            window.removeEventListener('pointerdown', onPointerDown);
            window.removeEventListener('pointermove', onPointerMove);
            window.removeEventListener('pointerup', onPointerUp);
            window.removeEventListener('pointercancel', onPointerUp);
        }
    };
})();
