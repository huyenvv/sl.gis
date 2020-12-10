var pageSize = 100;

function exportCSV() {
    var min = getDateStr($("#SearchModel_StartDate").val());
    var max = getDateStr($("#SearchModel_End").val());

    var rows = getDataForExport(min, max);
    exportToCsv('history.csv', rows);
}

function getData(start, end, page, callback) {
    var factoryId = $("#hydropowerPlant").data('id');
    var url = "/api/postData/" + factoryId + "/csv";

    if (!start) {
        start = getDateStr(new Date())
    }

    if (!end) {
        end = getDateStr(new Date())
    }

    url += "?start=" + start + "&end=" + end + "&page=" + page

    $.getJSON(url, function (response) {
        callback(response);
    });
}

function getDataForExport(min, max) {
    $.ajaxSetup({
        async: false
    });

    var rows = []
    var measures = []
    var page = 1
    do {
        getData(min, max, page, function (response) {
            if (page === 1) {
                rows = generateHeaderCsv(response.items);
            }

            measures = generateDataForCSV(response, rows.length - 1);

            measures.forEach(element => {
                rows.push(element);
            });
        });
        page++;
    } while (measures.length === pageSize);

    $.ajaxSetup({
        async: true
    });

    return rows;
}

function generateHeaderCsv(headers) {

    var rows = [];
    var row = ['#'];
    for (var i = 0; i < headers.length; i++) {
        row.push(headers[i].title + ' (' + headers[i].unit + ')');
    }
    row.push('Thời gian');
    rows.push(row);
    return rows;
}

function generateDataForCSV(response, startIndex) {
    var headers = response.items;

    var rows = [];
    $.each(response.data, function (index, item) {
        row = [(startIndex + index + 1)];
        for (var i = 0; i < headers.length; i++) {
            var value = item.values.find(m => m.id === headers[i].id);
            row.push(value.value);
            row.push(moment(value.time).format("YYYY-MM-DD HH:mm:ss"));
        }        
        rows.push(row);
    });

    return rows;
}

function exportToCsv(filename, rows) {
    var processRow = function (row) {
        var finalVal = '';
        for (var j = 0; j < row.length; j++) {
            var innerValue = row[j] ? row[j].toString() : '';
            if (row[j] instanceof Date) {
                innerValue = row[j].toLocaleString();
            };
            var result = innerValue.replace(/"/g, '""');
            if (result.search(/("|,|\n)/g) >= 0)
                result = '"' + result + '"';
            if (j > 0)
                finalVal += ';';
            finalVal += result;
        }
        return finalVal + '\n';
    };

    var csvFile = '';
    for (var i = 0; i < rows.length; i++) {
        csvFile += processRow(rows[i]);
    }

    var blob = new Blob(["\uFEFF" + csvFile], {
        type: 'text/csv; charset=utf-18'
    });
    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(blob, filename);
    } else {
        var link = document.createElement("a");
        if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
}

function getDateStr(dateTime) {
    if (dateTime)
        return moment(dateTime).format("yyyyMMDD");

    return '';
}