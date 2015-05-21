angular.module("BuildscreenApp.services")
    .factory("Configuration", function ($resource) {
        return $resource("/api/configuration/:id", { id: "@Id" },
        {
            update: { method: "PUT" }
        });
    }
);