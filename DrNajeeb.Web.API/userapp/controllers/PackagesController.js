(function (app) {

    var PackagesController = function ($scope, SuportService) {

        $scope.packages = [];

        $scope.buyPackage = function (pid, is2CheckOut) {
            if (is2CheckOut) {
                var url = 'https://www.2checkout.com/checkout/spurchase?sid=1432125&quantity=1&product_id=' + pid;
                window.location.href = url;
            }
            else {
                var url = 'https://secure.avangate.com/order/checkout.php?PRODS=4677771&QTY=1&CART=1&CARD=2&DESIGN_TYPE=1&ORDERSTYLE=nLWo5Za5nLo=&BACK_REF=http%3A%2F%2Fwww.drnajeeblectures.com%2Faccount-activation%2F';
                window.location.href = url;
            }
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