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

        return ReportsService;
    }

    ReportsService.$inject = ["$http"];
    app.factory("ReportsService", ReportsService);

}(angular.module("DrNajeebAdmin")));