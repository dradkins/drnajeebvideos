﻿(function (app) {

    var VideoService = function ($http, $q) {

        var VideoService = {};

        VideoService.lastWatchedChecked = false;

        VideoService.getVideoNotifications = function () {
            return $http.get("/api/videos/videoNotifications")
                   .then(function (response) {
                       return response.data;
                   });
        }

        VideoService.getVideo = function (videoId) {
            return $http.get("/api/videos/getUserVideo/" + videoId)
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getNewVideos = function (pagingInfo) {
            return $http.get("/api/videos/getUserNewVideo/", { params: pagingInfo })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getFreeVideos = function (pagingInfo) {
            return $http.get("/api/videos/getUserFreeVideo/", { params: pagingInfo })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.addToFavorites = function (videoId) {
            return $http.post("/api/videos/addUserVideoToFavorite/", { videoId: videoId, userId: "" })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.removeFromFavorites = function (videoId) {
            return $http.delete("/api/videos/deleteUserVideoToFavorite/" + videoId)
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getFavoritesVideos = function (pagingInfo) {
            return $http.get("/api/videos/getUserFavoriteVideos/", { params: pagingInfo })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getHistoryVideos = function (pagingInfo) {
            return $http.get("/api/videos/getUserVideosHistory/", { params: pagingInfo })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getVideoThumbnail = function (videoId) {

            return $http.get("https://api.vimeo.com/videos/" + videoId + "/pictures?access_token=f2ef11bc8f72e6653d3043cbe243bcb0").then(function (data) {
                var link = data.data.data[0].sizes[0].link;
                return link;
            });
        }

        VideoService.getVimeoVideoDetails = function (id) {
            return $http.get("https://api.vimeo.com/videos/" + id + "?access_token=f2ef11bc8f72e6653d3043cbe243bcb0").then(function (data) {
                return data.data;
            });
        }

        function loadImage(video) {
            return $http.get("https://api.vimeo.com/videos/" + video.vzaarVideoId + "/pictures?access_token=f2ef11bc8f72e6653d3043cbe243bcb0").then(function (data) {
                video.thumbnailURL = data.data.data[0].sizes[0].link;
                return video;
            });
        }

        VideoService.getVideoThumbnails = function (videos) {
            var promises = [];
            var returnVideos = [];
            angular.forEach(videos, function (video) {
                promises.push(loadImage(video));
            })

            return $q.all(promises).then(function (results) {
                angular.forEach(results, function (video) {
                    returnVideos.push(video);
                });
                return returnVideos;
            });
        }

        VideoService.downloadVideo = function (id) {
            return $http.get("https://api.vimeo.com/videos/" + id + "?access_token=f2ef11bc8f72e6653d3043cbe243bcb0").then(function (data) {
                var link = data.data.download[0].link;
                return link;
            });
        }

        VideoService.saveDownloadStats = function (videoId) {
            return $http.get("/api/videos/saveDownloadStats/", { params: { id: videoId } })
                   .then(function (response) {
                       return response.data;
                   });
        }

        VideoService.getDownloadedVideos = function (pagingInfo) {
            return $http.get("/api/videos/getUserDownloadedVideos/", { params: pagingInfo })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getVideoTotalDownloads = function (vId) {
            return $http.get("/api/videos/GetVideoTotalDownloads/", { params: { id: vId } })
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.saveVideoTime = function (vd) {
            return $http.post("/api/videos/saveVideoTime/", vd)
                   .then(function (response) {
                       return response.data;
                   });
        }

        VideoService.getWeeklyTopVideos = function () {
            return $http.get("/api/videos/getWeeklyTopVideos")
                   .then(function (response) {
                       return response.data;
                   });
        };

        VideoService.getAllTimeTopVideos = function () {
            return $http.get("/api/videos/getAllTimeTopVideos")
                   .then(function (response) {
                       return response.data;
                   });
        };


        VideoService.getLastWatchedVideo = function () {
            return $http.get("/api/videos/getLastWatchedVideo")
                   .then(function (response) {
                       return response.data;
                   });
        };

        return VideoService;

    }

    VideoService.$inject = ["$http", "$q"];
    app.factory("VideoService", VideoService);

}(angular.module("DrNajeebUser")));