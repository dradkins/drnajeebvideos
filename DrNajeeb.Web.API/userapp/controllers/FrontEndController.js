(function (app) {

    var FrontEndController = function ($scope, LibraryService, VideoService) {

        $scope.categories = [];
        $scope.videos = [];
        $scope.selectedCategory = null;

        $scope.categorySelected = function (cat) {
            loadVideos(cat);
        }

        $scope.getThumbnailURL = function (videoId) {
            return VideoService.getVideoThumbnail(videoId).then(function (data) {
                return data;
            })
        }

        var onCategoryVideos = function (data) {
            $scope.videos = null;
            $scope.videos = data;
            angular.forEach($scope.videos, function (v) {
                v.thumbnailURL = "";
                VideoService.getVideoThumbnail(v.vzaarVideoId).then(function (t) {
                    v.thumbnailURL = t;
                })
            })
        }

        var onCategories = function (data) {
            $scope.categories = data;
            loadVideos($scope.categories[0]);
        }

        var loadVideos = function (cat) {
            $scope.selectedCategory = cat;
            LibraryService.getFrontEndVideos(cat.id).then(onCategoryVideos, onError);
        }

        var onError = function (error) {
            console.log(error);
        }

        function init() {
            LibraryService.getCategories().then(onCategories, onError);
        }

        init();

    };

    FrontEndController.$inject = ["$scope", "LibraryService", "VideoService"];
    app.controller("FrontEndController", FrontEndController);

}(angular.module("DrNajeebUser")));