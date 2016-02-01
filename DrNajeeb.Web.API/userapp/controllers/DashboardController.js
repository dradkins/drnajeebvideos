﻿(function (app) {

    var DashboardController = function ($scope, $location, CurrentUserService, DashboardService, VideoService) {

        $scope.favoriteVideos = [];
        $scope.newVideos = [];
        $scope.weeklyVideos = [];
        $scope.allTimeVideos = [];
        $scope.totalVideos = 0;
        $scope.totalFavorites = 0;
        $scope.showData = false;
        $scope.totalUnreadMessages = 0;
        $scope.userSubscription = {
            description: "Loading.."
        };
        $scope.newFeatures = [];

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 4,
            search: '',
            totalItems: 0
        };

        $scope.goto = function (url) {
            $location.path(url);
        }

        var onDashboardData = function (data) {
            $scope.totalVideos = data[0].count;
            $scope.newVideos = data[0].data;
            angular.forEach($scope.newVideos, function (v) {
                v.thumbnailURL = "";
                VideoService.getVideoThumbnail(v.vzaarVideoId).then(function (t) {
                    v.thumbnailURL = t;
                })
            })
            $scope.totalFavorites = data[1].count;
            $scope.favoriteVideos = data[1].data;
            $scope.totalUnreadMessages = data[2];
            $scope.userSubscription = data[3];
            $scope.newFeatures = data[4];

            //weekly videos
            $scope.weeklyVideos = data[5].data;
            angular.forEach($scope.weeklyVideos, function (v) {
                v.thumbnailURL = "";
                VideoService.getVideoThumbnail(v.vzaarVideoId).then(function (t) {
                    v.thumbnailURL = t;
                })
            })

            //all time videos
            $scope.allTimeVideos = data[6].data;
            angular.forEach($scope.allTimeVideos, function (v) {
                v.thumbnailURL = "";
                VideoService.getVideoThumbnail(v.vzaarVideoId).then(function (t) {
                    v.thumbnailURL = t;
                })
            })
        }

        $scope.getThumbnailURL = function (videoId) {
            return VideoService.getVideoThumbnail(videoId).then(function (data) {
                return data;
            })
        }

        var onError = function (error) {
            console.log(error);
        }

        function init() {
            if (CurrentUserService.profile.isLoggedIn()) {
                $scope.showData = true;
            }
            else {
                $location.path("/login");
            }
            DashboardService.getDashboardData($scope.pagingInfo).then(onDashboardData, onError);
        }

        init();

    };

    DashboardController.$inject = ["$scope", "$location", "CurrentUserService", "DashboardService", "VideoService"];
    app.controller("DashboardController", DashboardController);

}(angular.module("DrNajeebUser")));