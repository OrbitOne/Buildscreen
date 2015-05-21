angular.module("BuildscreenApp.services")
    .factory("Build", function($resource) {
            return $resource("/buildscreenapi/:urlString/:since");
        }
    );