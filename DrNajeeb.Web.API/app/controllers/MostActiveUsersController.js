(function (app) {

    var MostActiveUsersController = function ($scope, ReportsService, toastr) {

        $scope.pagingInfo = {
            page: 1,
            itemsPerPage: 20,
            search: '',
            totalItems: 0,
            fromDate: new Date(),
            toDate: new Date()
        };

        $scope.users = [];

        $scope.search = function () {
            $scope.pagingInfo.page = 1;
            loadUsers();
        };

        $scope.selectPage = function (page) {
            $scope.pagingInfo.page = page;
            loadUsers();
        };

        $scope.downloadFile = function (value) {
            if (value) {
                var downloadPath = "/home/GetInactiveUsers/" + $filter('date')($scope.reportDate, "yyyy-MM-dd");
                window.open(downloadPath, '_blank', '');
            }
            else {
                var downloadPath = "/home/GetAllInactiveUsers";
                window.open(downloadPath, '_blank', '');
            }
        }

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

    };

    MostActiveUsersController.$inject = ["$scope", "ReportsService", "toastr"];
    app.controller("MostActiveUsersController", MostActiveUsersController);

}(angular.module("DrNajeebAdmin")));