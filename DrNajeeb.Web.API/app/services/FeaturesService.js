'use strict';

(function (app) {

    var FeaturesService = function ($http) {

        var FeaturesService = {};

        FeaturesService.getFeatures = function () {
            return $http.get("/api/newFeatures/getAll")
                        .then(function (response) {
                            return response.data;
                        });
        };

        FeaturesService.addFeature = function (category) {
            return $http.post("/api/newFeatures/addFeature", category)
                        .then(function (response) {
                            return response.data;
                        });
        };

        FeaturesService.deleteFeature = function (id) {
            return $http.delete("/api/newFeatures/deleteFeature/" + id)
                        .then(function (response) {
                            return response.data;
                        });
        };

        return FeaturesService;
    }

    FeaturesService.$inject = ["$http"];
    app.factory("FeaturesService", FeaturesService);

}(angular.module("DrNajeebAdmin")));