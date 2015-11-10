'use strict';

(function (app) {

    var UsersService = function ($http) {

        var UsersService = {};

        UsersService.isInstitutionalAccount = function () {
            return $http.get("/api/user/CheckInstitutionalUser")
                        .then(function (response) {
                            return response.data;
                        });
        };

        return UsersService;
    }

    UsersService.$inject = ["$http"];
    app.factory("UsersService", UsersService);

}(angular.module("DrNajeebUser")));