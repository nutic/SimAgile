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
    var forecastLength = dateRangeLength / 4;

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
            .showControls(false);

        chart.xDomain([0, dateRangeLength + forecastLength]);
        chart.yAxis.tickFormat(d3.format('d'));

        return chart;
    }

    var cfd = createChart();

    svg.datum(cumulativeData).call(cfd);

    function drawForecast() {
        var forecast = cumulativeData.map(function(d) {
            var values = d.values.map(function(v) {
                return v.y0 + v.y;
            });

            var trend = (values[values.length - 1] - values[0]) / dateRangeLength;

            return {
                key: d.key,
                start: values[values.length - 1],
                end: values[values.length - 1] + (trend * forecastLength)
            };
        });

        var x = cfd.xScale();
        var y = cfd.yScale();

        svg
            .select('.nv-stackedWrap')
            .selectAll("line.forecast")
            .data(forecast)
            .enter()
            .append("line")
            .attr("class", "forecast")
            .attr("stroke", function(d, i) {
                return cfd.color()(d, i);
            })
            .attr("x1", x(dateRangeLength))
            .attr("x2", x(dateRangeLength + forecastLength))
            .attr("y1", function(d) {
                return y(d.start);
            })
            .attr("y2", function(d) {
                return y(d.end);
            });
    }


    drawForecast();
}