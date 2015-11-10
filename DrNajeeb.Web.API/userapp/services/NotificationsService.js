(function (app) {

    var NotificationsService = function (CurrentUserService, $rootScope) {

        var NotificationsService = {};
        $.connection.hub.qs = { 'access_token': CurrentUserService.profile.token };
        $.connection.hub.logging = true;
        var notificationHub = $.connection.notificationHub;

        NotificationsService.connect = function () {
            $.connection.hub.start().done(function () {
                notificationHub.server.registerClient(CurrentUserService.profile.username);
            });
        }


        NotificationsService.sendMessage = function (message) {
            notificationHub.server.sendUserMessage(message, CurrentUserService.profile.username);
        }

        notificationHub.client.addMessage = function (message) {
            $rootScope.$broadcast('messageReceived', message);
        };

        notificationHub.client.newVideoUploaded = function (videoId, name) {
            $rootScope.$broadcast('newVideoUploaded', { videoId: videoId, name: name });
        };

        notificationHub.client.logout = function (guid) {
            if (CurrentUserService.profile.guid == guid) {
                CurrentUserService.logout();
                alert("You have been logged out from your account. Someone else is using your account.")
                location.reload(true);
            }
        };


        return NotificationsService;

    };

    NotificationsService.$inject = ["CurrentUserService", "$rootScope"]
    app.factory("NotificationsService", NotificationsService);

}(angular.module("DrNajeebUser")))