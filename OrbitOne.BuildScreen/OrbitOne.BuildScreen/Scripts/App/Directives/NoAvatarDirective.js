angular.module("BuildScreenApp.directives")
    .directive("noAvatar", function ($compile) {

        var setAlternativeToImage = function (el, initials) {

            var para = document.createElement("p");
            para.setAttribute("initials", "");
            para.setAttribute("user-name", "{{build.RequestedByName}}");
            para.className = "initials";
            el.replaceWith(para);
            return para;


        };

        return {
            restrict: "A",
            link: function (scope, el, attr) {
                var element;
                scope.$watch(function () {
                    return attr.ngSrc;
                }, function () {
                    var src = attr.ngSrc;
                    if (!src) {
                        element = setAlternativeToImage(el);
                        $compile(element)(scope);
                    }
                });

                el.bind("error", function() {
                    element = setAlternativeToImage(el);
                    $compile(element)(scope);
                });

                
            }
        };
    });