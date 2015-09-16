(function (app) {

    var NotificationsController = function ($scope) {

        var init = function () {
            console.log('Notifications Controller');
        }
        init();

    };

    NotificationsController.$inject = ["$scope"];
    app.controller("NotificationsController", NotificationsController);

}(angular.module("DrNajeebUser")));