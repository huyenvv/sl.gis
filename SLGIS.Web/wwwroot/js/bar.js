var dateFormat = "yy-mm-dd";

$(document).ready(function () {
    Highcharts.setOptions({
        lang: {
            rangeSelectorZoom: '',
            rangeSelectorFrom: 'Từ',
            rangeSelectorTo: 'đến'
        },
        time: {
            useUTC: false
        },
    });

    $('.datepick').val(moment().format("YYYY-MM-DD"));

    getData('', '', drawChart);

    autoRefreshAfterMiute(1);

    var from = $("#from")
        .datepicker()
        .on("change", function () {
            to.datepicker("option", "minDate", getDate(this));
        }),
        to = $("#to").datepicker()
            .on("change", function () {
                from.datepicker("option", "maxDate", getDate(this));
            });

    // Set the datepicker's date format
    $.datepicker.setDefaults({
        autoOpen: false,
        dateFormat: dateFormat,
        showOtherMonths: true,
        dayNamesMin: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    });

    $("#btnfilter").click(function () {
        var min = getDateStr($("#from").datepicker("getDate"));
        var max = getDateStr($("#to").datepicker("getDate"));

        refreshChart(min, max);
    });
})

function getData(year, callback) {
    var factoryId = $("#hydropowerPlant").data('id');
    var url = "/api/postData/" + factoryId + "/detail";
    if (year) {
        url += "?year=" + year
    }

    var series = [];

    $.getJSON(url, function (response) {
        for (var i = 0; i < items.length; i++) {
            var item = items[i].item;
            var data = items[i].data

            var seri = {
                name: item.title,
                data: data
            };
            series.push(seri);
        }

        callback(series);
    });
}

function reDrawChart(series) {
    for (let i = 0; i < series.length; i++) {
        const data = series[i].data;
        chart.series[i].setData(data);
    }
}

function drawChart(series) {
    Highcharts.chart('bar-container', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Biểu đồ trung bình tháng'
        },
        subtitle: {
            text: ''
        },
        xAxis: {
            categories: [
                'Th1',
                'Th2',
                'Th3',
                'Th4',
                'Th5',
                'Th6',
                'Th7',
                'Th8',
                'Th9',
                'Th10',
                'Th11',
                'Th12'
            ],
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: ''
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true,
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: [{
            name: 'Sản lượng ngày (MWh)',
            data: [49.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, 54.4]

        }, {
            name: 'Tổng lượng nước qua Tuabin (m3)',
            data: [83.6, 78.8, 98.5, 93.4, 106.0, 84.5, 105.0, 104.3, 91.2, 83.5, 106.6, 92.3]

        }, {
            name: 'Số giờ phát điện (giờ)',
            data: [48.9, 38.8, 39.3, 41.4, 47.0, 48.3, 59.0, 59.6, 52.4, 65.2, 59.3, 51.2]
        }]
    });
}

function autoRefreshAfterMiute(minute) {
    setInterval(() => {
        var min = getDateStr($("#from").datepicker("getDate"));
        var max = getDateStr($("#to").datepicker("getDate"));

        refreshChart(min, max);

    }, minute * 60 * 1000);
}

function refreshChart(min, max) {
    var chart = Highcharts.charts[0];

    chart.showLoading('Loading data from server...');
    getData(min, max, function (res) {
        reDrawChart(res);
        chart.hideLoading();
    });
}

function isLessThanEqualMonths(start, end, months) {
    const diffTime = Math.abs(end - start);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= months * 30;
}

function getDateStr(dateTime) {
    if (dateTime)
        return moment(dateTime).format("yyyyMMDD");

    return '';
}

function getDate(element) {
    var date;
    try {
        date = $.datepicker.parseDate(dateFormat, element.value);
    } catch (error) {
        date = null;
    }

    return date;
}

function toggleAllChartSeries(e) {
    var isShowing = $(e).data('show') == '1';

    if (!window.chart || !window.chart.series || window.chart.series.length == 0) {
        return;
    }

    for (var index = 0; index < window.chart.series.length; index++)  // iterate through each series
    {
        if (window.chart.series[index].name == 'Navigator')
            continue;

        if (isShowing) {
            window.chart.series[index].hide();
        } else {
            window.chart.series[index].show();
        }
    }

    $(e).data('show', (isShowing ? '0' : '1'));
    $(e).text(isShowing ? 'Hiện thông số' : 'Ẩn thông số');
}