'use strict';

(function (app) {

    var RolesService = function ($http) {

        var RolesService = {};

        RolesService.getAll = function () {
            return $http.get("/api/role/getall")
                        .then(function (response) {
                            return response.data;
                        });
        };

        return RolesService;
    }

    RolesService.$inject = ["$http"];
    app.factory("RolesService", RolesService);

}(angular.module("DrNajeebAdmin")));