(function (app) {

    var NotificationsController = function ($scope, $location, NotificationsService, toastr, VideoService) {

        //Event fire when message received
        $scope.notifications = [];
        $scope.videoNotifications = [];
        $scope.totalNewVideos = 0;

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
            VideoService.getVideoNotifications().then(onVideoNotification, onError);
        }

        var onVideoNotification = function (data) {
            $scope.videoNotifications = data.videosList;
            $scope.totalNewVideos = data.toNotificationVideos;
        }
        
        var onError = function (error) {
            console.log(error);
        }

        init();

    };

    NotificationsController.$inject = ["$scope", "$location", "NotificationsService", "toastr", "VideoService"];
    app.controller("NotificationsController", NotificationsController);

}(angular.module("DrNajeebUser")));