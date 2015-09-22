(function (app) {

    var NotificationsController = function ($scope, $location, NotificationsService, toastr) {

        //Event fire when message received
        $scope.notifications = [];
        $scope.videoNotifications = [];

        $scope.$on("messageReceived", function (event, data) {
            if ($location.path() === "/support") {
                return false;
            }
            toastr.info("New message received");
            $scope.notifications.push(data);
        })

        $scope.$on("newVideoUploaded", function (event, data) {
            toastr.info("New video has been uploaded");
            $scope.videoNotifications.push({
                videoId:data.videoId,
                name:video.name
            });
        })

        var init = function () {
            NotificationsService.connect();
        }

        init();

    };

    NotificationsController.$inject = ["$scope", "$location", "NotificationsService", "toastr"];
    app.controller("NotificationsController", NotificationsController);

}(angular.module("DrNajeebUser")));