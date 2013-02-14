var baseSelectAll = d3.selection.prototype.selectAll;
d3.selection.prototype.selectAll = function () {
    var result = baseSelectAll.apply(this, arguments);
    result.delay = function () {
        return result;
    };
    return result;
};

var baseTransition = d3.transition;
d3.transition = function () {
    var result = baseTransition.apply(this, arguments);
    result.delay = function () {
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