// Initialize and add the map
function initMap() {
    // The location of Hanoi
    const hanoi = { lat: 21.026613, lng: 105.833889 };

    // The map, centered at hanoi
    const map = new google.maps.Map(document.getElementById("content"), {
        zoom: 8,
        center: hanoi,
    });
    // The marker, positioned at Uluru
    const marker = new google.maps.Marker({
        position: hanoi,
        map: map,
    });
}