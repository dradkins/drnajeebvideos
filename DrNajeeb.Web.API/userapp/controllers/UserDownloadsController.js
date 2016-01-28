(function (app) {

    var UserDownloadsController = function ($scope, $location, VideoService) {

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
            VideoService.getDownloadedVideos($scope.pagingInfo).then(onVideos, onError);
        }

        var onError = function (error) {
            $log.info("Error in UserDownloadsController");
            $log.error(error);
        }

        $scope.downloadVideo = function (video) {
            VideoService.getVideoTotalDownloads(video.id).then(function (data) {
                if (data == 3) {
                    toastr.info("Unable to download video because your maximum limit for this video download is reached.")
                }
                else {
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
                }

            }, function (err) {
                console.log(err);
            })
        }

        loadVideos();

    };

    UserDownloadsController.$inject = ["$scope", "$location", "VideoService"];
    app.controller("UserDownloadsController", UserDownloadsController);

}(angular.module("DrNajeebUser")));