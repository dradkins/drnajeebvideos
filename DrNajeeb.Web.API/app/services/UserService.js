'use strict';

(function (app) {

    var UserService = function ($http) {

        var UserService = {};

        UserService.getUsers = function (pagingInfo) {
            return $http.get("/api/user/getall", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.addUser = function (user) {
            return $http.post("/api/user/addUser", user)
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.editUser = function (user) {
            return $http.post("/api/user/updateUser", user)
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.deleteUser = function (id) {
            console.log(id);
            return $http.post("/api/user/deleteUser",'"'+id+'"')
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.showUser = function (id) {
            return $http.get("/api/user/getSingle", { params: id })
                        .then(function (response) {
                            return response.data;
                        });
        };

        UserService.getUsersCount = function () {
            return $http.get("/api/user/getUserChartData")
                        .then(function (response) {
                            return response.data;
                        });
        };

        return UserService;
    }

    UserService.$inject = ["$http"];
    app.factory("UserService", UserService);

}(angular.module("DrNajeebAdmin")));