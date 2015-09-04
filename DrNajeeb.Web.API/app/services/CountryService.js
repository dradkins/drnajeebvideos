'use strict';

(function (app) {

    var CountryService = function ($http) {

        var CountryService = {};

        CountryService.getAll = function () {
            return $http.get("/api/country/getall")
                        .then(function (response) {
                            return response.data;
                        });
        };

        return CountryService;
    }

    CountryService.$inject = ["$http"];
    app.factory("CountryService", CountryService);

}(angular.module("DrNajeebAdmin")));