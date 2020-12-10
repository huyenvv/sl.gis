let map;
var poly = [];
let markers = [];
var plantMarkers = [];
var substationMarkers = [];
var damsMarkers = [];

var hydropowers = [];
var substation = [];
var currentId = null;
var currentType = null;
var editMode = false;
function initMap() {
    $.get("/api/hydropower/map", function (data) {
        hydropowers = data;
        $.get("/api/substation/map", function (data) {
            substations = data;
            plantMarkers = makeMarkers(hydropowers, "hydropower-plant");
            damsMarkers = makeMarkers(hydropowers, "hydropower-dams");

            substationMarkers = makeMarkers(substations, "substation");
            map = new google.maps.Map(document.getElementById("content"), {
                zoom: 10,
                center: substationMarkers[0].getPosition(),
            });

            showMarkers();
        });
    });
}
function makeMarkers(items, type) {
    var markers = [];
    for (var i in items) {
        var item = type === "hydropower-plant" ? items[i].hydropowerPlant : type === "hydropower-dams" ?
            (items[i].hydropowerPlant.hydropowerDams.length > 0 ? items[i].hydropowerPlant.hydropowerDams[0] : null) : items[i].substation;
        var label = '';
        if (type === 'substation') {
            label = item.columnNumber + "";
        }
        //data.push({id: item.id, text: item.name});
        if (item != null) {
            const marker = new google.maps.Marker({
                position: new google.maps.LatLng(parseFloat(item.location.lat),
                    parseFloat(item.location.lng)),
                map: null,
                icon: getMarkerIcon(type)
            });

            google.maps.event.addListener(marker, 'click', (function (data, type) {
                return function () {
                    showInfo(data, type);
                    //map.panTo(marker.getPosition());
                    //map.setZoom(14);
                };
            })(items[i], type));
            markers.push(marker);
        }
    }
    return markers;
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

    var levels = $('.electric-level:checked').map(function () {
        return $(this).val();
    }).get();
    for (var i in substationMarkers) {
        substationMarkers[i].setMap(null);
    }
    substationMarkers = [];
    if (levels.length > 0) {
        var subs = substations.filter(s => {
            return levels.includes(s.substation.electricLevel);
        });
        substationMarkers = makeMarkers(subs, "substation");
        for (var i in substationMarkers) {
            substationMarkers[i].setMap(map);
        }
        var lineNames = [...new Set(substations.map(item => item.substation.lineName))];
        poly = [];
        for (var i = 0; i < lineNames.length; i++) {
            var lineName = lineNames[i];
            var sortedSubs = substations.filter(s => {
                return lineName === s.substation.lineName;
            }).sort((a, b) => (a.substation.columnNumber > b.substation.columnNumber) ? 1 : ((b.substation.columnNumber > a.substation.columnNumber) ? -1 : 0));
            var points = [];
            for (var j = 0; j < sortedSubs.length; j++) {
                points.push({ lat: parseFloat(sortedSubs[j].substation.location.lat), lng: parseFloat(sortedSubs[j].substation.location.lng) });
            }

            var flightPath = new google.maps.Polyline({
                path: points,
                strokeColor: "violet",
                strokeOpacity: 1.0,
                strokeWeight: 2,
            });
            poly.push(flightPath);
            break;
        }
        for (var i = 0; i < poly.length; i++) {
            poly[i].setMap(map);
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
    for (var i = 0; i < poly.length; i++) {
        poly[i].setMap(null);
    }
}

function showInfo(data, type) {
    if (data.canManage)
        $('.manage-section').removeClass('d-none')
    else
        $('.manage-section').addClass('d-none')

    $('.info').each(function () { $(this).addClass('d-none') });
    $('.' + type).removeClass('d-none');
    var image = '/images/plant-thumb.jpg';
    var editUrl = '';
    if (type === "hydropower-plant") {
        editUrl = '/admin/hydropower/neworedit?id=' + data.hydropowerPlant.id;
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
        if (data.hydropowerPlant.image) {
            image = data.hydropowerPlant.image;
        }
    }

    if (type === "hydropower-dams") {
        editUrl = '/admin/hydropower/neworedit?id=' + data.hydropowerPlant.id;
        var dams = data.hydropowerPlant.hydropowerDams.length > 0 ? data.hydropowerPlant.hydropowerDams[0] : null;

        $('#dams-name').text(dams.name);
        $('#dams-address').text('Địa điểm xây dựng: ' + dams.address);
        var mission = "";
        if (dams.missions) {
            mission = dams.missions.join();
        }
        $('#dams-missions').text('Nhiệm vụ chính: ' + mission);
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
        if (dams.image) {
            image = dams.image;
        }
    }

    if (type === "substation") {
        var sub = data.substation;
        editUrl = '/admin/substation/neworedit?id=' + sub.id;
        $('#substation-name').text(sub.lineName);
        $('#substation-address').text('Địa chỉ: ' + sub.address);
        $('#substation-number').text('Vị trí cột: ' + sub.columnNumber);
        $('#substation-line').text('Tên đường dây: ' + sub.lineName);
        $('#substation-level').text('Cấp điện áp: ' + sub.electricLevel);
        image = '/images/sub-thumb.jpg';
    }
    $('.dams-img').attr("src", image);
    $(".edit-link").attr("href", editUrl);
}

function getMarkerIcon(type) {
    if (type === "hydropower-plant") {
        return {
            url: "/images/plant-32.png",
            // This marker is 20 pixels wide by 32 pixels high.
            size: new google.maps.Size(32, 32),
            // The origin for this image is (0, 0).
            origin: new google.maps.Point(0, 0),
            // center
            anchor: new google.maps.Point(16, 16),
        };
    }
    if (type === "hydropower-dams") {
        return {
            url: "/images/dams-32.png",
            size: new google.maps.Size(32, 32),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(16, 16),
        };
    }
    if (type === "substation") {
        return {
            url: "/images/sub-16.png",
            size: new google.maps.Size(16, 16),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(8, 8),
        };
    }
}