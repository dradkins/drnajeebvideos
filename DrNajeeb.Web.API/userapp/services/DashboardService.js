(function (app) {

    var DashboardService = function ($q, VideoService) {

        var DashboardService = {};

        function getNewVideos(pagingInfo) {
            return VideoService.getNewVideos(pagingInfo);
        }

        function getFavoriteVideos(pagingInfo) {
            return VideoService.getFavoritesVideos(pagingInfo);
        }

        DashboardService.getDashboardData = function (pagingInfo) {
            return $q.all([getNewVideos(pagingInfo), getFavoriteVideos(pagingInfo)]).then(function (results) {
                return results;
            });
        }

        return DashboardService;

    }

    DashboardService.$inject = ["$q", "VideoService"];
    app.factory("DashboardService", DashboardService);

}(angular.module("DrNajeebUser")));