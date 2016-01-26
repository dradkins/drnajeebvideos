'use strict';

(function (app) {

    var UserActivityController = function ($scope, $routeParams, toastr, ReportsService) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            userId:''
        };
        $scope.userName = '';

        $scope.activities = [];

        var onError = function (error) {
            console.info("Error in UserActivityController");
            console.error(error);
            toastr.error("Unable to load report at this time");
        }

        var onActivities = function (data) {
            $scope.activities = data.data;
            $scope.userName = data.userName;
        }

        var loadActivities = function () {
            $scope.pagingInfo.userId = $routeParams.userId;
            ReportsService.getUserActivityReport($scope.pagingInfo).then(onActivities, onError);
        }

        loadActivities();
    };

    UserActivityController.$inject = ["$scope", "$routeParams", "toastr", "ReportsService"];

    app.controller("UserActivityController", UserActivityController);

}(angular.module("DrNajeebAdmin")));