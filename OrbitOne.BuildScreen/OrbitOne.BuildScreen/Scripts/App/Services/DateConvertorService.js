angular.module("BuildscreenApp.services").
factory("DateConvertor", function () {
    var convertToTicks = function (dateString) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(dateString);
        return parseInt(results[1]);
    }
    return {
        convertDatesToTicks: function (builds) {
            for (var i = 0; i < builds.length; i++) {
                builds[i].FinishBuildDateTime = parseInt(convertToTicks(builds[i].FinishBuildDateTime));
                builds[i].StartBuildDateTime = parseInt(convertToTicks(builds[i].StartBuildDateTime));
            }
            return builds;
        }
    }
});

