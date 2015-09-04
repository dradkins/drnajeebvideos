'use strict';

(function (app) {

    var UserService = function ($http) {

        var UserService = {};

        UserService.getUser = function (pagingInfo) {
            return $http.get("/api/users/getall", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.addUser = function (user) {
            return $http.post("/api/users/addUsers", user)
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.editUser = function (user) {
            return $http.post("/api/users/updateUser", user)
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.deleteUser = function (id) {
            return $http.delete("/api/users/deleteUser/" + id)
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.showUser = function (id) {
            return $http.get("/api/users/getSingle", { params: id })
                        .then(function (response) {
                            return response.data;
                        });
        };

        return UserService;
    }

    UserService.$inject = ["$http"];
    app.factory("UserService", UserService);

}(angular.module("DrNajeebAdmin")));