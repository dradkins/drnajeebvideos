(function (app) {

    var CurrentUserService = function ($location, $rootScope, localStorageService) {

        var USERKEY = "utoken";
        var CurrentUserService = {};

        var initialize = function () {
            var user = {
                username: "",
                token: "",
                isLoggedIn: function () {
                    return this.token;
                },
            };
            var localUser = localStorageService.get(USERKEY);
            if (localUser) {
                var currentUser = angular.fromJson(localUser);
                user.username = currentUser.username;
                user.token = currentUser.token;
            }
            return user;
        }

        CurrentUserService.profile = initialize();

        CurrentUserService.setProfile = function (username, token) {
            CurrentUserService.profile.username = username;
            CurrentUserService.profile.token = token;
            $rootScope.USER_NAME = username;
            localStorageService.set(USERKEY, angular.toJson(CurrentUserService.profile));
        }

        CurrentUserService.logout = function () {
            localStorageService.remove(USERKEY);
            $location.path("/login");
        }

        CurrentUserService.externalLogin = {
            token: null,
            name: null,
        };

        return CurrentUserService;
    }

    CurrentUserService.$inject = ["$location", "$rootScope", "localStorageService"];
    app.factory("CurrentUserService", CurrentUserService);

}(angular.module("DrNajeebUser")));