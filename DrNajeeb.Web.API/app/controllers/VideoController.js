﻿'use strict';

(function (app) {

    var VideosController = function ($scope, $routeParams, $modal, $log, VideoService) {

        $scope.video = {
            id: 0,
            name: null,
            description: null,
            duration: 0,
            releaseYear: null,
            dateLive: null,
            isEnabled: true,
            standardVideoId: null,
            fastVideoId: null,
            categories:[],
            isFreeVideo: false,
        };

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            sortBy: 'name',
            reverse: false,
            search: '',
            totalItems: 0
        };

        $scope.addOrEditModal = function (video) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'addoreditvideo.html',
                controller: 'AddOrEditVideoController',
                //size: 'lg',
                resolve: {
                    video: function () {
                        if (video) {
                            return angular.copy(video);
                        }
                        else {
                            return angular.copy($scope.video);
                        }
                    }
                }
            });

            modalInstance.result.then(function () {
                loadVideos();
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.videos = [];

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

        $scope.deleteVideo = function (vid) {
            if (confirm("Are you sure to delete this video..?")) {
                VideoService.deleteVideo(vid.id).then(function (response) {
                    $scope.videos.splice($scope.videos.indexOf(vid), 1);
                }, onError);
            }
        }

        var onError = function (error) {
            console.info("Error in videoController");
            console.error(error);
        }

        var onadd = function (data) {
            loadVideos();
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
            VideoService.getVideos($scope.pagingInfo).then(onvideos, onError);
        }

        loadVideos();
    };

    VideosController.$inject = ["$scope", "$routeParams", "$modal", "$log", "VideoService"];



    var AddOrEditVideoController = function ($scope, $modalInstance, $filter, VideoService, CategoriesService, video, NotificationsService) {

        $scope.video = video;
        $scope.categories = [];
        $scope.liveDate;

        var onVideoCategories = function (data) {
            $scope.categories = data;
        }

        $scope.checkAll = function () {
            $scope.video.categories = $scope.categories.map(function (item) { return item.id; });
        };
        $scope.uncheckAll = function () {
            $scope.video.categories = [];
        };

        var onError = function (error) {
            console.log(error);
        }

        function init() {
            if (video.dateLive) {
                $scope.liveDate = $filter('date')(video.dateLive, 'dd-MMMM-yyyy');
            }
            if (video.categoryVideos) {
                angular.forEach(video.categoryVideos, function (item) {
                    $scope.video.categories.push(item.categoryId);
                })
                console.log($scope.video);
            }

            CategoriesService.getVideoCategories().then(onVideoCategories, onError);
        }

        init();

        $scope.ok = function () {
            if ($scope.liveDate) {
                $scope.video.dateLive = $filter('date')($scope.liveDate, 'yyyy-MM-ddTHH:mm:ss.sssZ');
            }
            if (video.id != 0) {
                VideoService.editVideo($scope.video).then(function (response) {
                    $modalInstance.close();
                })
            }
            else {
                VideoService.addVideo($scope.video).then(function (response) {
                    NotificationsService.newVideoAdded(response.id, response.name);
                    $modalInstance.close();
                })
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }

    AddOrEditVideoController.$inject = ["$scope", "$modalInstance", "$filter", "VideoService","CategoriesService", "video", "NotificationsService"]


    app.controller("VideosController", VideosController);

    app.controller('AddOrEditVideoController', AddOrEditVideoController);

}(angular.module("DrNajeebAdmin")));