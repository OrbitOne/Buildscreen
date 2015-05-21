angular.module("BuildscreenApp.services").
service("Isotope", ["Search", function (Search) {

    var iso;
    var regex;
    var textProp = document.documentElement.textContent !== undefined ? 'textContent' : 'innerText';
    function getText(elem) {
        return elem[textProp];
    }

    var search = Search;

    var isBuildSince = function (itemElem) {
        if (search.dateFilter.month === 0 && search.dateFilter.week === 0) return true;
        if (itemElem.querySelector(".inProgress") || itemElem.querySelector(".notStarted")) {
            return true;
        } else {
            var date = new Date();
            date.setMonth(date.getMonth() - search.dateFilter.month);
            date.setDate(date.getDate() - (search.dateFilter.week * 7));
            if (new Date(parseInt(itemElem.querySelector("#dateFilter").getAttribute("data-date"))) >= date) {
                return true;
            } else return false;
        }
    }
    var filterAdvancedFns = function (itemElem) {
        var name = itemElem.querySelector(".item-subtitle").getAttribute("title");
        if (search.TeamProjectTags.length === 0 && search.BuildNameTags.length === 0 && isBuildSince(itemElem)) return true;
        for (var i = 0; i < search.TeamProjectTags.length; i++) {
            if (search.TeamProjectTags[i].text === name) {
                if (isBuildSince(itemElem)) return true;
            }
        }
        var buildDefinition = itemElem.querySelector(".item-title").getAttribute("title");
        for (var j = 0; j < search.BuildNameTags.length; j++) {
            if (search.BuildNameTags[j].text === buildDefinition) {
                if (isBuildSince(itemElem)) return true;
            }
        }
        return false;
    }

    var filterFns = function (itemElem) {
        var name = itemElem.querySelector(".item-subtitle").getAttribute("title");
        var buildDefinition = itemElem.querySelector(".item-title").getAttribute("title");
        return (name.toLowerCase().trim().indexOf(Search.searchKey.toLowerCase().trim()) !== -1) ?
            true : (buildDefinition.toLowerCase().trim().indexOf(Search.searchKey.toLowerCase().trim()) !== -1) ? true : false;
    }

    this.init = function () {
        var container = document.querySelector(".isotope");

        iso = new Isotope(container,
        {
            transitionDuration: "1s",
            itemSelector: ".item",
            layoutMode: "packery",
            getSortData: {
                type: function (itemElem) {
                    if (itemElem.className.indexOf("notStarted") >= 0) {
                        return 0;
                    } else if (itemElem.className.indexOf("inProgress") >= 0) {
                        return 1;
                    } else if (itemElem.className.indexOf("failed") >= 0) {
                        return 2;
                    } else if (itemElem.className.indexOf("partiallySucceeded") >= 0) {
                        return 3;
                    } else if (itemElem.className.indexOf("stopped") >= 0) {
                        return 4;
                    } else {
                        return 5;
                    }
                },
                date: function (itemElem) {
                    return itemElem.querySelector("#dateFilter").getAttribute("data-date");
                }
            },
            sortAscending: {
                type: true,
                date: false
            }

        });
        iso.arrange({
            sortBy: ["type", "date"]
        });
        iso.layout();
    };

    this.update = function (added) {
        var container = document.querySelector(".isotope");
        if (added) {
            iso.reloadItems();
        }
        if (iso !== undefined) {
            iso.updateSortData(container.children);
            iso.arrange({
                filter: (search.model.checkValue ? filterAdvancedFns : filterFns),
                sortBy: ["type", "date"]
            });
        }
    };
    Search.addObserver(this);
}]);

