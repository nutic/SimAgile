function drawTimeDistr(placeholder, data) {
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

    var chart = nv.models.multiBarChart()
        .showLegend(true)
        .showControls(false);
    
    chart.xAxis.tickFormat(d3.format('#.#'));

    function getFor(state){
        var histogram = d3.layout.histogram()(data.done.map(function (d) {
            return d.timeInState[state];
        }));

        return {
            key: state + " time distribution",
            values: histogram.map(function (d) {
                return {x: d.x, y: d.y}
            })
        };
        
    }

    createSvg().datum([getFor("In Dev"), getFor("Testing")]).call(chart);
}