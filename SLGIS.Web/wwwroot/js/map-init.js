let map;
let markers = [];
var plantMarkers = [];
var substationMarkers = [];
var damsMarkers = [];
var hydropowers = [];
var substation = [];

function initMap() {
    const hanoi = { lat: 21.026613, lng: 105.833889 };

    map = new google.maps.Map(document.getElementById("content"), {
        zoom: 8,
        center: hanoi,
    });
    $.get("/api/hydropower/map", function (data) {
        hydropowers = data;
        $.get("/api/substation/map", function (data) {
            substation = data;
            plantMarkers = makeMarkers(hydropowers, "hydropower-plant");

            showMarkers("hydropower-plant")
        });
    });
}
function makeMarkers(items, type) {
    var markers = [];
    for (var i in items) {
        var item = type === "hydropower-plant" ? items[i].hydropowerPlant : type === "hydropower-dams" ?
            items[i].hydropowerPlant.hydropowerDams : items[i].substation;

        const marker = new google.maps.Marker({
            position: new google.maps.LatLng(parseFloat(item.location.lat),
                parseFloat(item.location.lng)),
            map: null,
        });

        marker.addListener("click", () => {
            showInfo(items[i], type);
            map.panTo(marker.getPosition());
            map.setZoom(10);
        });
        markers.push(marker);
        return markers;
    }
}
function showMarkers() {
    clearMarkers();
    var selection = $("input[name=layerSelect]:checked").map(
        function () { return this.value; }).get().join(",");

    if (selection.includes("hydropower-plant")) {
        for (var i in plantMarkers) {
            plantMarkers[i].setMap(map);
        }
    }
    if (selection.includes("hydropower-dams")) {
        for (var i in damsMarkers) {
            damsMarkers[i].setMap(map);
        }
    }
    if (selection.includes("substation")) {
        for (var i in substationMarkers) {
            substationMarkers[i].setMap(map);
        }
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    for (var i in plantMarkers) {
        plantMarkers[i].setMap(null);
    }
    for (var i in damsMarkers) {
        damsMarkers[i].setMap(null);
    }
    for (var i in substationMarkers) {
        substationMarkers[i].setMap(null);
    }
}

function showInfo(data, type) {
    if (data.canManage)
        $('.manage-section').removeClass('d-none')
    else
        $('.manage-section').addClass('d-none')

    $('.info').each(function () { $(this).addClass('d-none') });
    $('.' + type).removeClass('d-none');

    if (type === "hydropower-plant") {
        $('#plant-name').text(data.hydropowerPlant.name);
        $('#plant-owner-name').text("Chủ đầu tư: " + data.hydropowerPlant.ownerName);
        $('#plant-owner-address').text('Địa chỉ: ' + data.hydropowerPlant.address);
        $('#plant-owner-person-name').text('Người đại diện: ' + data.hydropowerPlant.personOwnername);
        $('#plant-owner-phone').text(data.hydropowerPlant.phone);
        $('#plant-owner-email').text(data.hydropowerPlant.email);
        $('#plant-wattage').text('Công suất lắp máy: ' + data.hydropowerPlant.wattage);
        $('#plant-level').text('Cấp công trình: ' + data.hydropowerPlant.level);
        $('#plant-build-address').text('Địa điểm xây dựng: ' + data.hydropowerPlant.buildAddress);
        $('#plant-area').text('Diện tích đất: ' + data.hydropowerPlant.area);
        $('#plant-start-build').text('Khởi công/ Hoàn thành: ' + data.hydropowerPlant.startBuild + ' - ' + data.hydropowerPlant.endBuild);
        $('#plant-total').text('Tổng mức đầu tư: ' + data.hydropowerPlant.totalInvestment);
    }

    if (type === "hydropower-dams") {
        var dams = data.hydropowerPlant.hydropowerDams;

        $('#dams-name').text(dams.name);
        $('#dams-address').text('Địa điểm xây dựng: ' + dams.address);
        $('#dams-missions').text('Nhiệm vụ chính: ' + dams.missions.join());
        $('#dams-start-build').text('Khởi công/ Hoàn thành: ' + dams.startBuild + ' - ' + dams.endBuild);
        $('#dams-nguon-von').text('Nguồn vốn: ' + dams.nguonVon);

        $('#dams-owner-name').text('Chủ sở hữu: ' + dams.ownerName);
        $('#dams-owner-address').text(dams.ownerAddress);
        $('#dams-owner-phone').text(dams.ownerPhone);
        $('#dams-owner-fax').text(dams.ownerFax);
        $('#dams-owner-email').text(dams.ownerEmail);
        $('#dams-owner-website').text(dams.ownerWebsite);

        $('#dams-exploit-name').text('Khai thác: ' + dams.exploitName);
        $('#dams-exploit-address').text('Địa chỉ: ' + dams.exploitAddress);
        $('#dams-exploit-fax').text(dams.exploitPhone);
        $('#dams-exploit-email').text(dams.exploitFax);
        $('#dams-exploit-email').text(dams.exploitEmail);
    }
}