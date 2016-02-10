(function (app) {

    var ManagersController = function ($scope, UserService, toastr) {

        $scope.managers = [];

        var onManagers = function (data) {
            $scope.managers = null;
            $scope.managers = data;
        }

        var onError = function (err) {
            console.info("Error in managers controller");
            console.error(err);

            toastr.error('inable to load managers at this time');
        }

        var loadManagers = function () {
            UserService.getManagers().then(onManagers, onError);
        }

        loadManagers();

    }

    ManagersController.$inject = ["$scope", "UserService", "toastr"];
    app.controller('ManagersController', ManagersController);

}(angular.module("DrNajeebAdmin")))