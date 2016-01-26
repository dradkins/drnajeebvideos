(function (app) {

    var MostActiveUsersController = function ($scope, ReportsService, toastr) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
        };

        $scope.users = [];

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadUsers();
        };

        var onUsers = function (data) {
            $scope.users = null;
            $scope.users = data.data;
            $scope.pagingInfo.totalItems = data.count;
        }

        var onError = function (error) {
            toastr.error("Unable to load users at this time");
            console.info("Error in UserController");
            console.error(error);
        }

        var loadUsers = function () {
            ReportsService.getMostActiveUsers($scope.pagingInfo).then(onUsers, onError);
        }

        loadUsers();

    };

    MostActiveUsersController.$inject = ["$scope", "ReportsService", "toastr"];
    app.controller("MostActiveUsersController", MostActiveUsersController);

}(angular.module("DrNajeebAdmin")));