angular.module("BuildScreenApp")
    .controller("ModalController", [
        "$scope", "$location", "Search",
        function ($scope, $location, Search) {
            var search = Search;

            var copyDateInput;

            $scope.dateFilter = search.dateFilter;

            angular.copy(search.dateFilter, copyDateInput);

            $scope.TeamProjectTags = search.TeamProjectTags;

            $scope.BuilddefinitionTags = search.BuildNameTags;

            $scope.subtractFromDate = function (string) {
                switch (string) {
                    case "week":
                        if (search.dateFilter.week > 0)
                            search.dateFilter.week--;
                        break;
                    case "month":
                        if (search.dateFilter.month > 0)
                            search.dateFilter.month--;
                        break;
                }
                search.updateDateFilter();
            }
            $scope.addToDate = function (string) {
                switch (string) {
                    case "week":
                        search.dateFilter.week++;
                        break;
                    case "month":
                        search.dateFilter.month++;
                        break;
                }
                search.updateDateFilter();
            }
            $scope.updateDateElement = function () {
                if (isNaN($scope.dateFilter.month) || $scope.dateFilter.month < 0 || $scope.dateFilter.month ==="") {
                    $scope.dateFilter.month = 0;
                } else if (isNaN($scope.dateFilter.week) || $scope.dateFilter.week < 0 || $scope.dateFilter.week ==="") {
                    $scope.dateFilter.week = 0;
                } 
                search.updateDateFilter();
            }

            $scope.ListChanged = function () {
                search.updateTeamProjectTags($scope.TeamProjectTags);
            }
            $scope.ListBuildNameTagsChanged = function () {
                search.updateBuildNameTags($scope.BuilddefinitionTags);
            }
            $scope.possibleTeamProjectTags = function (query) {
                return search.getArrayOfTeamProjectNames(query);
            }
            $scope.possibleBuildNameTags = function (query) {
                return search.getArrayOfBuildNames(query);
            }
        }]);