﻿'use strict';

(function (app) {

    var VideoDownloadStatsController = function ($scope, $routeParams, toastr, ReportsService) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            search: '',
            totalItems: 0
        };

        $scope.videos = [];

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadVideos();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadVideos();
        };

        var onError = function (error) {
            console.info("Error in VideoDownloadStatsController");
            console.error(error);
            toastr.error("Unable to load videos at this time");
        }

        var onvideos = function (data) {
            $scope.videos = null;
            $scope.videos = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadVideos = function () {
            if ($routeParams.searchTerm) {
                $scope.pagingInfo.search = $routeParams.searchTerm;
            }
            ReportsService.getVideoDownloadStats($scope.pagingInfo).then(onvideos, onError);
        }

        loadVideos();
    };

    VideoDownloadStatsController.$inject = ["$scope", "$routeParams", "toastr", "ReportsService"];

    app.controller("VideoDownloadStatsController", VideoDownloadStatsController);

}(angular.module("DrNajeebAdmin")));