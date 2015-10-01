(function (app) {

    var LibraryController = function ($scope, $log, $routeParams, $location, LibraryService, VideoService) {

        $scope.categories = [];
        $scope.videos = [];
        $scope.selectedCategory = null;
        $scope.search = "";

        $scope.categorySelected = function (cat) {
            loadVideos(cat);
        }

        $scope.showVideo = function (video) {
            $location.path('/video/' + video.id);
        }

        $scope.addToFavorites = function (video) {
            VideoService.addToFavorites(video.id).then(onFavoritesAdd, onError);
        }

        $scope.removeFromFavorites = function (video) {
            VideoService.removeFromFavorites(video.id).then(onFavoritesRemove, onError);
        }

        var onFavoritesAdd = function (data) {
            angular.forEach($scope.videos, function (vid) {
                if (vid.id == data) {
                    vid.isFavoriteVideo = true;
                    return false;
                }
            })
        }

        var onFavoritesRemove = function (data) {
            angular.forEach($scope.videos, function (vid) {
                if (vid.id == data) {
                    vid.isFavoriteVideo = false;
                    return false;
                }
            })
        }

        var onCategoryVideos = function (data) {
            $scope.videos = null;
            $scope.videos = data;
        }

        var onCategories = function (data) {
            $scope.categories = data;
            loadVideos($scope.categories[0]);
        }

        var loadVideos = function (cat) {
            $scope.selectedCategory = cat;
            LibraryService.getVideos(cat.id).then(onCategoryVideos, onError);
        }

        var onError = function (error) {
            $log.info("Error in LibraryController");
            $log.error(error);
        }

        function init() {
            LibraryService.getCategories().then(onCategories, onError);
        }

        $scope.downloadVideo = function (video) {
            console.log(video);
            VideoService.downloadVideo(video.vzaarVideoId)
                    .then(function (data) {

                        var link = document.createElement("a");
                        link.download = video.name + ".mp4";
                        link.href = data;
                        link.click();
                    })
        }

        init();

    };

    LibraryController.$inject = ["$scope", "$log", "$routeParams", "$location", "LibraryService", "VideoService"];
    app.controller("LibraryController", LibraryController);

}(angular.module("DrNajeebUser")));