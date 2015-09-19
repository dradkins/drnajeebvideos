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

        notificationHub.client.logout = function () {
            CurrentUserService.logout();
            aler("Your account used for loin to another device.")
            location.reload(true);
        };


        return NotificationsService;

    };

    NotificationsService.$inject = ["CurrentUserService", "$rootScope"]
    app.factory("NotificationsService", NotificationsService);

}(angular.module("DrNajeebUser")))