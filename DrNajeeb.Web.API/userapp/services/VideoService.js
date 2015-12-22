(function (app) {

    var VideoService = function ($http, $q) {

        var VideoService = {};

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

            //return $http.jsonp('http://vimeo.com/api/v2/video/' + videoId + '.json?callback=JSON_CALLBACK&_=' + (new Date().getTime()))
            //    .success(function (r) {
            //        console.info("Success: " + r);
            //    })
            //    .error(function (e) {
            //        console.info("Error: " + e);
            //    });


            //var deferred = $q.defer();

            //$http.jsonp('http://vimeo.com/api/v2/video/' + videoId + '.json').success(function (data) {
            //    console.log(data);
            //    var thumbs = data[0]['thumnail_small'];
            //    deferred.resolve(thumbs);
            //}).error(function (error) {
            //    console.log(error);
            //    deferred.reject();
            //});
            //return deferred.promise;
        }


        VideoService.downloadVideo = function (id) {
            return $http.get("http://webservice.drnajeebvideos.com/service.php?id=" + id)
                .then(function (response) {
                    return response.data;
                })
        }

        return VideoService;

    }

    VideoService.$inject = ["$http", "$q"];
    app.factory("VideoService", VideoService);

}(angular.module("DrNajeebUser")));