angular.module("BuildscreenApp.services")
    .factory("ConfigurationChecker", ["$location", "$q", "Configuration", function($location, $q, Configuration) {
        return {
            checkIfEmpty: function () {
                var deferred = $q.defer();
                Configuration.query(function (configs) {
                    if (configs.length === 0) {
                        $location.path("/config");
                    } else {
                        deferred.resolve();
                    }
                });
                return deferred.promise;
            }
        }
    }
]);