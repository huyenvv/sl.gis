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

function getData(start, end, callback) {
    var factoryId = $("#hydropowerPlant").data('id');
    var url = "/api/postData/" + factoryId;
    if (start && end) {
        url += "?start=" + start + "&end=" + end + ""
    }

    var series = [];

    $.getJSON(url, function (items) {
        for (var i = 0; i < items.length; i++) {
            var item = items[i].item;
            var data = items[i].data.map(m => {
                var created = Date.parse(m.time);
                return [created, m.value];
            });

            var seri = {
                name: item.title,
                data: data,
                tooltip: {
                    valueSuffix: " " + item.unit,
                    xDateFormat: '%Y-%m-%d %H:%M:%S'
                },
                dataGrouping: {
                    enabled: false
                }
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
    window.chart = Highcharts.stockChart('chart-container', {
        rangeSelector: {
            enabled: false,
        },
        title: {
            text: ''
        },
        series: series,
        navigator: {
            adaptToUpdatedData: false,
            enabled: false
        },
        legend: {
            enabled: true
        },
        xAxis: {
            type: 'datetime',
            title: {
                text: 'Thời gian'
            },
            ordinal: false,
            labels: {
                formatter: function () {
                    return Highcharts.dateFormat('%m/%d %H:%M', this.value);
                }
            },
        },
        yAxis: {
            title: {
                text: 'Thông số'
            }
        },
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