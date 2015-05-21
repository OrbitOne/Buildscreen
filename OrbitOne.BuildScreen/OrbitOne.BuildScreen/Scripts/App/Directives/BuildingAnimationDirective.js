angular.module("BuildScreenApp.directives")
    .directive("buildingAnimation", ["$interval", function ($interval) {

        var convertTime = function (counterValue) {
            if (Math.abs(counterValue) > 59) {
                var min = (counterValue >= 0) ? Math.floor(counterValue / 60) : Math.ceil(counterValue / 60);
                var sec = Math.abs((counterValue) % 60);
                return min + ":" + ((sec < 10) ? ("0" + sec) : sec);
            } else {
                return counterValue;
            }
        };
        var cssBuilder = function (element, totalSec, delaySec) {
            var buildingOrbit = element.find("img");
            buildingOrbit.css({
                "-webkit-animation": "spinning " + totalSec + "s linear infinite",
                "-moz-animation": "spinning " + totalSec + "s linear infinite",
                "-ms-animation": "spinning " + totalSec + "s linear infinite",
                "-o-animation": "spinning " + totalSec + "s linear infinite",
                "animation": "spinning " + totalSec + "s linear infinite",
                "-webkit-animation-delay": "-" + delaySec + "s",
                "-moz-animation-delay": "-" + delaySec + "s",
                "-ms-animation-delay": "-" + delaySec + "s",
                "-o-animation-delay": "-" + delaySec + "s",
                "animation-delay": "-" + delaySec + "s"
            });
        }
        return {
            restrict: "AE",
            scope: {
                lastbuildtime: "@",
                starttime: "@"
            }, 
            replace: true,
            templateUrl: "/Scripts/App/Templates/BuildingAnimationTemplate.html",
            link: function (scope, elem, attrs) {
                var jsonTimeObject = JSON.parse(scope.lastbuildtime);
                var lastBuildTimeInSeconds = jsonTimeObject.TotalSeconds;
                var tijdelijkVoorDummy = new Date(parseInt(scope.starttime));
                var timePassed = (new Date()).getTime() - tijdelijkVoorDummy.getTime();
                var timePassedSeconds = timePassed / 1000;
                var remainingTime = Math.floor(lastBuildTimeInSeconds - (timePassedSeconds));
                scope.counter = remainingTime;
                var delayPositive = lastBuildTimeInSeconds - remainingTime;
                cssBuilder(elem, (lastBuildTimeInSeconds === 0) ? 60 : lastBuildTimeInSeconds, delayPositive);

                var interval = $interval(function () {
                    scope.counter--;
                    var element = elem.find("p");
                    element.text(convertTime(scope.counter));
                    if(scope.counter < 0) element.css("color", "#960E00");
                }, 1000);
                elem.on("$destroy", function () {
                    $interval.cancel(interval);
                });
                interval.then();
            }
        }
    }]);
