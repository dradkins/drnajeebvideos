'use strict';

(function (app) {

    var SupportService = function ($http) {

        var SupportService = {};

        SupportService.loadUsers = function (pagingInfo) {
            return $http.get("/api/support/getContactRequestUsers", { params: pagingInfo })
                .then(function (response) {
                    return response.data;
                })
        };

        SupportService.loadUserMessage = function (userId) {
            return $http.get("/api/support/getUserMessages?userId=" + userId)
                        .then(function (response) {
                            return response.data;
                        });
        }

        SupportService.sendMessage = function (support) {
            return $http.post("/api/support/supportMessageReply", support)
                    .then(function (response) {
                        return response.data;
                    });
        }

        SupportService.sendMessageToAll = function (support) {
            return $http.post("/api/support/sendMessageToAll", support)
                    .then(function (response) {
                        return response.data;
                    });
        }

        return SupportService;
    }

    SupportService.$inject = ["$http"];
    app.factory("SupportService", SupportService);

}(angular.module("DrNajeebAdmin")));