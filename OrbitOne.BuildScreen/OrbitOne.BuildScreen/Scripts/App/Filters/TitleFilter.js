angular.module("BuildScreenApp.filters", [])
    .filter("titleFilter", function () {
        return function (title, maxLength) {
            return (title.length > maxLength) ? title.substr(0, maxLength).trim() + "..." : title;
        };
    })
;