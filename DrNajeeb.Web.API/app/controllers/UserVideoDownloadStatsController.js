'use strict';

(function (app) {

    var UserVideoDownloadStatsController = function ($scope, $routeParams, toastr, ReportsService) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            search: '',
            totalItems: 0,
            userId:''
        };

        $scope.userName = '';

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
            console.info("Error in UserVideoDownloadStatsController");
            console.error(error);
            toastr.error("Unable to load videos at this time");
        }

        var onvideos = function (data) {
            $scope.videos = null;
            $scope.videos = data.data;
            $scope.pagingInfo.totalItems = data.count;
            $scope.userName = data.userName;
        }

        var loadVideos = function () {
            if ($routeParams.searchTerm) {
                $scope.pagingInfo.search = $routeParams.searchTerm;
            }
            $scope.pagingInfo.userId = $routeParams.userId;
            ReportsService.getUserVideoDownloadStats($scope.pagingInfo).then(onvideos, onError);
        }

        loadVideos();
    };

    UserVideoDownloadStatsController.$inject = ["$scope", "$routeParams", "toastr", "ReportsService"];

    app.controller("UserVideoDownloadStatsController", UserVideoDownloadStatsController);

}(angular.module("DrNajeebAdmin")));