'use strict';

(function (app) {

    var DashboardService = function ($http, $q, UserService, VideoService) {

        var DashboardService = {};

        function getUsersCount() {
            return $http.get("/api/user/getUserCount").then(function (response) {
                return response;
            });
        }

        function getVideosCount() {
            return $http.get("/api/videos/getVideosCount").then(function (response) {
                return response;
            });
        }

        function getLatestUsers() {
            return $http.get("/api/user/getLatestUsers").then(function (response) {
                return response;
            });
        }

        function getUserChartData() {
            return $http.get("/api/user/getUserChartData").then(function (response) {
                console.log(response);
                return response;
            });
        }

        function getTotalMessages() {
            return $http.get("/api/support/getTotalUnreadMessagesForAdmin").then(function (response) {
                return response;
            });
        }

        DashboardService.getDashboardData = function () {

            return $q.all([getUsersCount(), getVideosCount(), getLatestUsers(), getTotalMessages(), getUserChartData()]).then(function (results) {
                return results;
            });
        }

        return DashboardService;
    }

    DashboardService.$inject = ["$http", "$q", "UserService", "VideoService"];
    app.factory("DashboardService", DashboardService);

}(angular.module("DrNajeebAdmin")));