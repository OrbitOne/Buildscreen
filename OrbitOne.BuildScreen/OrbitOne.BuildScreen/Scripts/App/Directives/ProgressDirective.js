angular.module("BuildScreenApp.directives")
    .directive("progressDirective", function () {

        var getSucceededTestPercentage = function (build) {
            return (build.PassedNumberOfTests / build.TotalNumberOfTests) * 100;
        };

        return {
            restrict: "AE",
            scope: {
                build: "="
            },
            templateUrl: "Scripts/App/Templates/ProgressDirectiveTemplate.html",
            link: function (scope, elem, attrs) {
                scope.percentage = getSucceededTestPercentage(scope.build);
            }
        }
    });
