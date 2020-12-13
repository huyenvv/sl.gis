var exportCsvDetail = function () {
    var pageSize = 100;

    exportCSV();

    function exportCSV() {
        var min = getDateStr($("#SearchModel_StartDate").val());
        var max = getDateStr($("#SearchModel_End").val());

        var rows = getDataForExport(min, max);
        exportToCsv('history-detail.csv', rows);
    }

    function getData(start, end, page, callback) {
        var factoryId = $("#hydropowerPlant").data('id');
        var url = "/api/postData/csv-detail";

        if (!start) {
            start = getDateStr(new Date())
        }

        if (!end) {
            end = getDateStr(new Date())
        }

        url += "?start=" + start + "&end=" + end + "&page=" + page
        if (factoryId) {
            url += "&hydropowerPlantId=" + factoryId;
        }

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
        row.push('Thủy điện');
        rows.push(row);
        return rows;
    }

    function generateDataForCSV(response, startIndex) {
        var headers = response.items;

        var rows = [];
        $.each(response.data, function (index, data) {
            for (var i = 0; i < data.postDataDetails.length; i++) {
                var item = data.postDataDetails[i];
                row = [(startIndex + index + i + 1)];
                for (var j = 0; j < headers.length; j++) {
                    var value = item.values.find(m => m.code === headers[j].id);
                    row.push(value.value);
                }
                row.push(moment(value.time).format("YYYY-MM-DD HH:mm:ss"));
                row.push(data.plantName);
                rows.push(row);
            }
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
            return moment(dateTime).format("YYYYMMDD");

        return '';
    }
}