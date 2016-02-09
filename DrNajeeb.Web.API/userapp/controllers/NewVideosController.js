(function (app) {

    var NewVideosController = function ($scope, $log, $routeParams, $rootScope, $location, VideoService, toastr) {


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

        $scope.addToFavorites = function (video) {
            VideoService.addToFavorites(video.id).then(onFavoritesAdd, onError);
        }

        $scope.removeFromFavorites = function (video) {
            VideoService.removeFromFavorites(video.id).then(onFavoritesRemove, onError);
        }

        $scope.getThumbnailURL = function (videoId) {
            return VideoService.getVideoThumbnail(videoId).then(function (data) {
                return data;
            })
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

        var onVideos = function (data) {
            $scope.videos = null;
            $scope.videos = data.data;
            angular.forEach($scope.videos, function (v) {
                v.thumbnailURL = "";
                VideoService.getVideoThumbnail(v.vzaarVideoId).then(function (t) {
                    v.thumbnailURL = t;
                })
            })
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadVideos = function (cat) {
            if ($routeParams.searchTerm) {
                $scope.pagingInfo.search = $routeParams.searchTerm;
                $rootScope.SEARCH_TERM = "";
            }
            VideoService.getNewVideos($scope.pagingInfo).then(onVideos, onError);
        }

        var onError = function (error) {
            $log.info("Error in NewVideosController");
            $log.error(error);
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

        loadVideos();

    };

    NewVideosController.$inject = ["$scope", "$log", "$routeParams", "$rootScope", "$location", "VideoService", "toastr"];
    app.controller("NewVideosController", NewVideosController);

}(angular.module("DrNajeebUser")));