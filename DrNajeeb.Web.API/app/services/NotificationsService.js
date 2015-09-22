(function (app) {

    var NotificationsService = function (CurrentUserService, $rootScope) {

        var NotificationsService = {};
        $.connection.hub.qs = { 'access_token': CurrentUserService.profile.token };
        $.connection.hub.logging = true;
        var notificationHub = $.connection.notificationHub;

        NotificationsService.connect = function () {
            $.connection.hub.start().done(function () {
                console.log('connected with server');
                notificationHub.server.registerAdmin(CurrentUserService.profile.username);
            });
        }


        NotificationsService.sendMessage = function (message, toUser) {
            notificationHub.server.sendMessageReply(message, toUser);
        }

        notificationHub.client.messageFromUser = function (message, username) {
            console.log(username + " : " + message);
            $rootScope.$broadcast('messageReceived', { message: message, user: username });
        };


        NotificationsService.sendMessageToAll = function (message) {
            notificationHub.server.sendMessageToAll(message);
        }

        NotificationsService.newVideoAdded = function (id, name) {
            notificationHub.server.sendNewVideoNotification(id, name);
        }

        return NotificationsService;

    };

    NotificationsService.$inject = ["CurrentUserService", "$rootScope"]
    app.factory("NotificationsService", NotificationsService);

}(angular.module("DrNajeebAdmin")))