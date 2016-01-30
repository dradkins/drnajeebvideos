'use strict';

(function (app) {

    var MostWatchedVideosController = function ($scope, $routeParams, toastr, ReportsService) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            search: '',
            totalItems: 0,
            dateTo:new Date(),
            dateFrom: new Date(new Date() - (24 * 60 * 60 * 1000)),
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

        $scope.getReport = function () {
            loadVideos();
        }

        var onError = function (error) {
            console.info("Error in MostWatchedVideosController");
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
            ReportsService.getMostWatchedVideos($scope.pagingInfo).then(onvideos, onError);
        }

        loadVideos();
    };

    MostWatchedVideosController.$inject = ["$scope","$routeParams", "toastr", "ReportsService"];

    app.controller("MostWatchedVideosController", MostWatchedVideosController);

}(angular.module("DrNajeebAdmin")));