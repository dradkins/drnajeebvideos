(function (app) {

    var CurrentUserService = function ($location, $rootScope, localStorageService) {

        var USERKEY = "utoken";
        var CurrentUserService = {};

        var initialize = function () {
            var user = {
                username: "",
                token: "",
                fullName:"",
                isLoggedIn: function () {
                    return this.token;
                },
            };
            var localUser = localStorageService.get(USERKEY);
            if (localUser) {
                var currentUser = angular.fromJson(localUser);
                user.username = currentUser.username;
                user.token = currentUser.token;
                user.fullName = currentUser.fullName;

                $rootScope.FullName = currentUser.fullName;
            }
            return user;
        }

        CurrentUserService.profile = initialize();

        CurrentUserService.setProfile = function (username, token, fullName) {
            CurrentUserService.profile.username = username;
            CurrentUserService.profile.token = token;
            CurrentUserService.profile.fullName = fullName;
            $rootScope.USER_NAME = username;
            $rootScope.FullName = fullName;
            localStorageService.set(USERKEY, angular.toJson(CurrentUserService.profile));
        }

        CurrentUserService.logout = function () {
            localStorageService.remove(USERKEY);
            $location.path("/login");
        }

        return CurrentUserService;
    }

    CurrentUserService.$inject = ["$location", "$rootScope", "localStorageService"];
    app.factory("CurrentUserService", CurrentUserService);

}(angular.module("DrNajeebAdmin")));