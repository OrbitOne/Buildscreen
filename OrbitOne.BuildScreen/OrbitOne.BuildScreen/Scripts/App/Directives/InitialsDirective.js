angular.module("BuildScreenApp.directives")
    .directive("initials", function () {

        var updateInitials = function(elem, userName) {
            var initialArray = userName.split(" ");
            var initials = (initialArray[0].charAt(0) + initialArray[initialArray.length - 1].charAt(0)).toUpperCase();

            var colours = ["#1abc9c", "#2ecc71", "#3498db", "#9b59b6", "#34495e", "#16a085",
           "#27ae60", "#2980b9", "#8e44ad", "#2c3e50", "#f1c40f", "#e67e22", "#e74c3c",
           "#95a5a6", "#f39c12", "#d35400", "#c0392b", "#bdc3c7", "#7f8c8d"];

            var charIndex = initials.charCodeAt(0) - 65,
            colourIndex = charIndex % 19;

            elem.parent().css("backgroundColor", colours[colourIndex]);
            elem.text(initials);
        }

        return {
            restrict: "A",
            scope: {
                userName: "@"
            },
            link: function (scope, elem, attr) {
                scope.$watch("userName", function (newValue) {
                    updateInitials(elem, newValue);
                });
            }
        };
    });