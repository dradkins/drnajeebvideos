(function (app) {

    var CurrentUserService = function ($location, $rootScope, localStorageService) {

        var USERKEY = "utoken";
        var CurrentUserService = {};

        var initialize = function () {
            var user = {
                username: "",
                token: "",
                fullName: "",
                profilePic: "",
                isSocialMediaLogin:false,
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
                user.profilePic = currentUser.profilePic;
                user.isSocialMediaLogin = currentUser.isSocialMediaLogin;
            }
            return user;
        }

        CurrentUserService.profile = initialize();

        CurrentUserService.setProfile = function (username, token, fullName, profilePic, isSocialMediaLogin) {
            CurrentUserService.profile.username = username;
            CurrentUserService.profile.token = token;
            CurrentUserService.profile.fullName = fullName;
            CurrentUserService.profile.profilePic = profilePic;
            CurrentUserService.profile.isSocialMediaLogin = isSocialMediaLogin;
            $rootScope.USER_NAME = username;
            $rootScope.FULL_NAME = fullName;
            $rootScope.facebookProfilePic = profilePic;
            localStorageService.set(USERKEY, angular.toJson(CurrentUserService.profile));
        }

        CurrentUserService.logout = function () {
            localStorageService.remove(USERKEY);
            CurrentUserService.setProfile(null, null, null, null, false);
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