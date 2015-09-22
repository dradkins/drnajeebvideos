(function (app) {

    var VideoController = function ($scope, $routeParams, $sce, VideoService, toastr) {

        $scope.video = null;
        $scope.videoId = null;
        $scope.videoSource = null;

        $scope.addToFavorites = function (video) {
            VideoService.addToFavorites(video.id).then(onFavoritesAdd, onError);
        }

        $scope.removeFromFavorites = function (video) {
            VideoService.removeFromFavorites(video.id).then(onFavoritesRemove, onError);
        }

        $scope.backToLibrary = function () {
            window.history.back();
        }

        var onFavoritesAdd = function (data) {
            $scope.video.isFavoriteVideo = true;
        }

        var onFavoritesRemove = function (data) {
            $scope.video.isFavoriteVideo = false;
        }

        var onVideo = function (data) {
            $scope.video = data;
            $scope.videoId = "vzvd-" + $scope.video.vzaarVideoId;
            $scope.videoSource = $sce.trustAsResourceUrl("https://view.vzaar.com/" + $scope.video.vzaarVideoId + "/player?apiOn=true");
        }

        var onError = function (error) {
            if (error.status == 400) {
                toastr.error("you are not allowed to view this video")
                window.history.back();
            }
            console.log(error);
        }

        function init() {
            var videoId = $routeParams.videoId;
            VideoService.getVideo(videoId).then(onVideo, onError);
            setTimeout(window.scrollTo(0, 0), 100);
            window.scrollTo(0, 0);
        }
        init();

    };

    VideoController.$inject = ["$scope", "$routeParams", "$sce", "VideoService", "toastr"];
    app.controller("VideoController", VideoController);

}(angular.module("DrNajeebUser")));