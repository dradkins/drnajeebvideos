(function (app) {

    var DashboardController = function ($scope, DashboardService) {

        $scope.favoriteVideos = [];
        $scope.newVideos = [];
        $scope.totalVideos = 0;
        $scope.totalFavorites = 0;

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 4,
            search: '',
            totalItems: 0
        };

        var onDashboardData = function (data) {
            $scope.totalVideos = data[0].count;
            $scope.newVideos = data[0].data;
            $scope.totalFavorites = data[1].count;
            $scope.favoriteVideos = data[1].data;
        }

        var onError = function (error) {
            console.log(error);
        }

        var displayWeather = function () {
            /* ANIMATED WEATHER */
            var skycons = new Skycons({ "color": "#03a9f4" });
            // on Android, a nasty hack is needed: {"resizeClear": true}

            // you can add a canvas by it's ID...s
            skycons.add("current-weather", Skycons.RAIN);

            // start animation!
            skycons.play();
        }

        function init() {
            displayWeather();
            DashboardService.getDashboardData($scope.pagingInfo).then(onDashboardData, onError);
        }

        init();

    };

    DashboardController.$inject = ["$scope", "DashboardService"];
    app.controller("DashboardController", DashboardController);

}(angular.module("DrNajeebUser")));