(function (app) {

    var NotificationsService = function (CurrentUserService, $rootScope) {

        var NotificationsService = {};
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


        return NotificationsService;

    };

    NotificationsService.$inject = ["CurrentUserService", "$rootScope"]
    app.factory("NotificationsService", NotificationsService);

}(angular.module("DrNajeebAdmin")))