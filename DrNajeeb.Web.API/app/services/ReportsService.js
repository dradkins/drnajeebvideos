'use strict';

(function (app) {

    var ReportsService = function ($http) {

        var ReportsService = {};

        ReportsService.getMostWatchedVideos = function (pagingInfo) {
            return $http.get("/api/reports/getMostWatchedVideos", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getVideoDownloadStats = function (pagingInfo) {
            return $http.get("/api/reports/getVideoDownloadStats", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getUserVideoDownloadStats = function (pagingInfo) {
            return $http.get("/api/reports/getUserVideoDownloadStats", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getUsersStatsReport = function (pagingInfo) {
            return $http.get("/api/reports/getUsersStatsReport", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getRevenue = function (pagingInfo) {
            return $http.get("/api/reports/getRevenue", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getMostActiveUsers = function (pagingInfo) {
            return $http.get("/api/reports/getMostActiveUsers", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getGhostUsers = function (pagingInfo) {
            return $http.get("/api/reports/getGhostUsers", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        ReportsService.getUserActivityReport = function (pagingInfo) {
            return $http.get("/api/reports/getUserActivityReport", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };
        
        
        return ReportsService;
    }

    ReportsService.$inject = ["$http"];
    app.factory("ReportsService", ReportsService);

}(angular.module("DrNajeebAdmin")));