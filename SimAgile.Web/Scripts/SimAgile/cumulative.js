function drawCumulativeFlow(placeholder, data) {
    var width = $(placeholder).width();
    var height = 400;
    
    function createSvg() {
        return d3.select(placeholder)
            .append("div")
            .attr("class", "pull-left")
            .append("svg")
            .attr("width", width)
            .attr("height", height);
    }

    var svg = createSvg();

    function getCumulativeData(){
        var stack = d3.layout.stack()
            .values(function(d) { return d.values; });

        return stack(data.states.reverse().map(function(key) {
            return {
                key: key,
                values: $.map(data.history[key], function(d, i) {
                    return { day: i, y: d };
                })
            };
        }));
    }

    var cumulativeData = getCumulativeData();
    
    var dateRange = d3.extent(cumulativeData[0].values, function (d) { return d.day; });
    var dateRangeLength = dateRange[1] - dateRange[0];
    var forecastLength = d3.round(dateRangeLength / 3);
    var instantaneousDays = d3.round(dateRangeLength / 10);

    function getForecast() {
        var forecastSource = cumulativeData.map(function (d, i) {
            var values = d.values.map(function (v) {
                return v.y0 + v.y;
            });
            var trend = (values[values.length - 1] - values[0]) / dateRangeLength;

            return {
                key: d.key,
                index: i,
                start: values[0],
                mid: values[values.length - 1],
                end: values[0] + trend * (dateRangeLength + forecastLength)
            };
        });

        var backlog = forecastSource.filter(function (d, _) { return d.key == data.states[data.states.length - 1]; })[0];

        forecastSource.push({
            key: backlog.key,
            index: backlog.index,
            start: backlog.mid,
            mid: backlog.mid,
            end: backlog.mid
        });

        return forecastSource;
    }

    var forecast = getForecast();

    function createChart() {
        var chart = nv.models.stackedAreaChart()
            .x(function (d) {
                return d.day;
            })
            .y(function (d) {
                return d.y;
            })
            .clipEdge(true)
            .showLegend(true)
            .showControls(false)
            .tooltipContent(function ($1, day) {
                var states = '';
                for (var i = data.states.length - 1; i >= 0; i--) {
                    states += '<tr>' +
                        '<td>' + data.states[i] + '</td>' +
                        '<td>' + data.history[data.states[i]][day] + '</td>' +
                        '<td>units</td>' +
                    '</tr>';
                }
                var doneState = data.states[0];
                var inst = d3.max([day - instantaneousDays, 0]);
                var instPeriod = day - inst;
                var instThroughput = (data.history[doneState][day] - data.history[doneState][inst]) / (instPeriod);
                var done = data.done.filter(function (unit) {
                    return unit.endDate <= day && unit.endDate >= instPeriod;
                });
                var cycleTime = done.map(function (unit) {
                    return unit.cycleTime;
                });
                var instCycleTime = d3.mean(cycleTime);
                return '<h4>Day '+day+'</h4><table class="table table-condensed">' +
                    '<tr>' +
                        '<td>WIP</td>' +
                        '<td>' + data.history['WIP'][day] + '</td>' +
                        '<td>units</td>' +
                    '</tr>'+
                    states +
                    '<tr>' +
                        '<td>Throughput</td>' +
                        '<td>' + instThroughput + '</td>' +
                        '<td>units/d (for last ' + (instPeriod) + ' days)</td>' +
                    '</tr>' +
                    '<tr>' +
                        '<td>Avg. cycle time</td>' +
                        '<td>' + (instCycleTime || 'n/a') + '</td>' +
                        '<td>d (for last ' + (instPeriod) + ' days)</td>' +
                    '</tr>' +
                '</table>';
            });

        chart.xDomain([0, dateRangeLength + forecastLength]);
        chart.yDomain([0, forecast[forecast.length - 2].end]);
        chart.yAxis.tickFormat(d3.format('d'));

        return chart;
    }

    var cfd = createChart();
    svg.datum(cumulativeData).call(cfd);

    function drawForecast(source) {
        var x = cfd.xScale();
        var y = cfd.yScale();

        svg
            .select('.nv-stackedWrap')
            .selectAll("line.forecast")
            .data(source)
            .enter()
            .append("line")
            .attr("class", "forecast")
            .attr("stroke", function(d, _) {
                return cfd.color()(d, d.index);
            })
            .attr("x1", x(0))
            .attr("x2", x(dateRangeLength + forecastLength))
            .attr("y1", function(d) {
                return y(d.start);
            })
            .attr("y2", function(d) {
                return y(d.end);
            });
    }
    
    drawForecast(forecast);
}