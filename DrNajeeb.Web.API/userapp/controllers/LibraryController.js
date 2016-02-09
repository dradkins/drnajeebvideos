(function (app) {

    var LibraryController = function ($scope, $log, $routeParams, $location, LibraryService, VideoService, $timeout) {

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
            angular.forEach($scope.videos, function (v) {
                v.thumbnailURL = "";
                VideoService.getVideoThumbnail(v.vzaarVideoId).then(function (t) {
                    v.thumbnailURL = t;
                })
            })
            //VideoService.getVideoThumbnails(data).then(function (v) {
            //    $scope.videos = v;
            //})
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
            //VideoService.getVideoTotalDownloads(video.id).then(function (data) {
            //    if (data == 2) {
            //        toastr.info("The video reached the maximum download limit. Please contact customer support for assistance.")
            //    }
            //    else {
            VideoService.saveDownloadStats(video.id).then(function (data) {
                console.log(data);
            }, function (err) {
                console.log(err);
            });
            VideoService.downloadVideo(video.vzaarVideoId).then(function (data) {
                var link = document.createElement("a");
                link.download = video.name + ".mp4";
                link.href = data;
                document.body.appendChild(link);
                link.click();
            })
            //    }

            //}, function (err) {
            //    console.log(err);
            //})
        }

        $scope.getThumbnailURL = function (videoId) {
            return VideoService.getVideoThumbnail(videoId).then(function (data) {
                return data;
            })
        }

        init();

    };

    LibraryController.$inject = ["$scope", "$log", "$routeParams", "$location", "LibraryService", "VideoService", "$timeout"];
    app.controller("LibraryController", LibraryController);

}(angular.module("DrNajeebUser")));