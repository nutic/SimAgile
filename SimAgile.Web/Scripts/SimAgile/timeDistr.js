function drawDevTimeDistr(placeholder, data) {
    var width = $(placeholder).width();
    var height = 400;

    var histogram = d3.layout.histogram()(data.done.map(function(d) {
        return d.timeInState["In Dev"];
    }));

    function createSvg() {
        return d3.select(placeholder)
            .append("div")
            .attr("class", "pull-left")
            .append("svg")
            .attr("width", width)
            .attr("height", height);
    }

    var chart = nv.models.multiBarChart()
        .showLegend(true)
        .showControls(false);
    
    chart.xAxis.tickFormat(d3.format('#.#'));

    var devTimeDistr = {
        key: "Dev time distribution",
        values: histogram
    };

    var baseSelectAll = d3.selection.prototype.selectAll;
    d3.selection.prototype.selectAll = function () {
        var result = baseSelectAll.apply(this, arguments);
        result.delay = function() {
            return result;
        };
        return result;
    };

    var baseTransition = d3.transition;
    d3.transition = function() {
        var result = baseTransition.apply(this, arguments);
        result.delay = function() {
            return result;
        };
        var baseEach = result.each;
        result.each = function (str, func) {
            if (typeof (str) == 'string') {
                return baseEach.apply(this, [func]);
            } else {
                return baseEach.apply(this, arguments);
            }
        };
        return result;
    };

    createSvg().datum([devTimeDistr]).call(chart);
}