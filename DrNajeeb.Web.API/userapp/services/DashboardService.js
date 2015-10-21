(function (app) {

    var DashboardService = function ($q, VideoService, SuportService) {

        var DashboardService = {};

        function getNewVideos(pagingInfo) {
            return VideoService.getNewVideos(pagingInfo);
        }

        function getFavoriteVideos(pagingInfo) {
            return VideoService.getFavoritesVideos(pagingInfo);
        }

        function getPackage() {
            return SuportService.getUserPackage();
        }

        function getTotalUnreadMessages() {
            return SuportService.getTotalUnreadMessages();
        }

        function getNewFeatures() {
            return SuportService.getFeatures();
        }

        DashboardService.getDashboardData = function (pagingInfo) {
            return $q.all([getNewVideos(pagingInfo), getFavoriteVideos(pagingInfo), getTotalUnreadMessages(), getPackage(), getNewFeatures()]).then(function (results) {
                return results;
            });
        }

        return DashboardService;

    }

    DashboardService.$inject = ["$q", "VideoService", "SuportService"];
    app.factory("DashboardService", DashboardService);

}(angular.module("DrNajeebUser")));