(function (app) {

    var PackagesController = function ($scope, SuportService) {

        $scope.packages = [];

        $scope.buyPackage = function (pid) {
            var url = 'https://www.2checkout.com/checkout/spurchase?sid=1432125&quantity=1&product_id=' + pid;
            window.location.href = url;
        }

        var init = function () {
            SuportService.getSubscriptions().then(function (data) {
                $scope.packages = data;
            }, function (error) {
                console.log(error);
            })
        }
        init();

    };

    PackagesController.$inject = ["$scope", "SuportService"];
    app.controller("PackagesController", PackagesController);

}(angular.module("DrNajeebUser")));