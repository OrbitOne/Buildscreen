angular.module("BuildScreenApp.directives")
    .directive("timeSince", ["$interval" , "$timeout", function ($interval, $timeout) {
        return {
            scope: true,
            link: function (scope, elem, attrs) {

                var setDateSince = function()
                {
                    elem.text(moment(attrs.timeSince, "x").fromNow());
                }

                setDateSince();

                var timeoutId = $interval(function () {
                    setDateSince();
                }, 2000);

                elem.on("$destroy", function () {
                    $timeout.cancel(timeoutId);
                });
                
            }
        }
    }]);