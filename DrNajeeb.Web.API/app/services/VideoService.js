'use strict';

(function (app) {

    var VideoService = function ($http) {

        var VideoService = {};

        VideoService.getByCategory = function (id) {
            return $http.get("/api/videos/getByCategoryForSorting", { params: { id: id } })
                        .then(function (response) {
                            return response.data;
                        });
        };

        VideoService.getVideos = function (pagingInfo) {
            return $http.get("/api/videos/getall", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        VideoService.addVideo = function (video) {
            return $http.post("/api/videos/addVideo", video)
                        .then(function (response) {
                            return response.data;
                        });
        };

        VideoService.editVideo = function (video) {
            return $http.post("/api/videos/updateVideo", video)
                        .then(function (response) {
                            return response.data;
                        });
        };

        VideoService.deleteVideo = function (id) {
            return $http.delete("/api/videos/deleteVideo/" + id)
                        .then(function (response) {
                            return response.data;
                        });
        };

        VideoService.showVideo = function (videoId) {
            return $http.get("/api/videos/getSingle", { params: videoId })
                        .then(function (response) {
                            return response.data;
                        });
        };

        VideoService.updateOrder = function (videoorder) {
            return $http.post("/api/videos/updateOrder", videoorder)
                        .then(function (response) {
                            return response.data;
                        });
        };

        return VideoService;
    }

    VideoService.$inject = ["$http"];
    app.factory("VideoService", VideoService);

}(angular.module("DrNajeebAdmin")));