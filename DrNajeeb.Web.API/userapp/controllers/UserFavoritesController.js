﻿(function (app) {

    var UserFavoritesController = function ($scope, $location, VideoService) {

        $scope.videos = [];

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 10,
            search: '',
            totalItems: 0
        };

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadVideos();
        };

        $scope.sort = function (sortBy) {
            if (sortBy === $scope.pagingInfo.sortBy) {
                $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
            } else {
                $scope.pagingInfo.sortBy = sortBy;
                $scope.pagingInfo.reverse = false;
            }
            $scope.pagingInfo.page = 1;
            loadVideos();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadVideos();
        };

        $scope.showVideo = function (video) {
            $location.path('/video/' + video.id);
        }

        $scope.removeFromFavorites = function (video) {
            VideoService.removeFromFavorites(video.id).then(function (data) {
                $scope.videos.splice($scope.videos.indexOf(video), 1);
            }, onError);
        }

        var onVideos = function (data) {
            $scope.videos = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadVideos = function (cat) {
            VideoService.getFavoritesVideos($scope.pagingInfo).then(onVideos, onError);
        }

        var onError = function (error) {
            $log.info("Error in UserFavoritesController");
            $log.error(error);
        }

        loadVideos();

    };

    UserFavoritesController.$inject = ["$scope", "$location", "VideoService"];
    app.controller("UserFavoritesController", UserFavoritesController);

}(angular.module("DrNajeebUser")));