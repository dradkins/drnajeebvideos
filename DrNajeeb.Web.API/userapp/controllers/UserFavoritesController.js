(function (app) {

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

        $scope.getThumbnailURL = function (videoId) {
            return VideoService.getVideoThumbnail(videoId).then(function (data) {
                return data;
            })
        }

        var onVideos = function (data) {
            //$scope.videos = data.data;
            VideoService.getVideoThumbnails(data.data).then(function (v) {
                $scope.videos = v;
            })
            $scope.pagingInfo.totalItems = data.count;
        }

        var loadVideos = function (cat) {
            VideoService.getFavoritesVideos($scope.pagingInfo).then(onVideos, onError);
        }

        var onError = function (error) {
            $log.info("Error in UserFavoritesController");
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

    UserFavoritesController.$inject = ["$scope", "$location", "VideoService"];
    app.controller("UserFavoritesController", UserFavoritesController);

}(angular.module("DrNajeebUser")));