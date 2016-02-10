'use strict';

(function (app) {

    var ManagerActivitiesController = function ($scope, $routeParams, toastr, ReportsService) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            userId: ''
        };
        $scope.userName = '';

        $scope.activities = [];

        var onError = function (error) {
            console.info("Error in ManagerActivitiesController");
            console.error(error);
            toastr.error("Unable to load report at this time");
        }

        var onActivities = function (data) {
            $scope.activities = data.data;
            $scope.userName = data.userName;
        }

        var loadActivities = function () {
            $scope.pagingInfo.userId = $routeParams.userId;
            ReportsService.getManagerActivityReport($scope.pagingInfo).then(onActivities, onError);
        }

        loadActivities();
    };

    ManagerActivitiesController.$inject = ["$scope", "$routeParams", "toastr", "ReportsService"];

    app.controller("ManagerActivitiesController", ManagerActivitiesController);

}(angular.module("DrNajeebAdmin")));