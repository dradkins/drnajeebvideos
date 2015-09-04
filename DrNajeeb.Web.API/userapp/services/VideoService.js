(function (app) {

    var VideoService = function ($http) {

        var VideoService = {};

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


        return VideoService;

    }

    VideoService.$inject = ["$http"];
    app.factory("VideoService", VideoService);

}(angular.module("DrNajeebUser")));