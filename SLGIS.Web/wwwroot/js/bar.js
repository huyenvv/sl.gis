var dateFormat = "yy-mm-dd";

$(document).ready(function () {
    getData('', function (series) {
        drawChart(series);
        reDrawChart(series);
    });

    autoRefreshAfterMiute(1);

    $("#btnfilter").click(function () {
        var year = $("#year").val();
        refreshChart(year);
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
        for (var i = 0; i < response.length; i++) {
            var seri = {
                data: response[i]
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
    window.chart = Highcharts.chart('bar-container', {
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
            data: series[0]

        }, {
            name: 'Tổng lượng nước qua Tuabin (m3)',
            data: series[1]

        }, {
            name: 'Số giờ phát điện (giờ)',
            data: series[2]
        }]
    });
}

function autoRefreshAfterMiute(minute) {
    setInterval(() => {
        var year = $("#year").val();

        refreshChart(year);

    }, minute * 60 * 1000);
}

function refreshChart(year) {
    var chart = Highcharts.charts[0];

    chart.showLoading('Loading data from server...');
    getData(year, function (res) {
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
        return moment(dateTime).format("YYYYMMDD");

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