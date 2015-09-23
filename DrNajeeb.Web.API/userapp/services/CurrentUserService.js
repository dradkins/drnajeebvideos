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
                isFreeUser: false,
                isSocialMediaLogin: false,
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
                user.isFreeUser = currentUser.isFreeUser;
            }
            return user;
        }

        CurrentUserService.profile = initialize();

        CurrentUserService.setProfile = function (username, token, fullName, profilePic, isSocialMediaLogin, isFreeUser) {
            CurrentUserService.profile.username = username;
            CurrentUserService.profile.token = token;
            CurrentUserService.profile.fullName = fullName;
            CurrentUserService.profile.profilePic = profilePic;
            CurrentUserService.profile.isSocialMediaLogin = isSocialMediaLogin;
            CurrentUserService.profile.isFreeUser = isFreeUser;
            $rootScope.USER_NAME = username;
            $rootScope.FULL_NAME = fullName;
            $rootScope.facebookProfilePic = profilePic;
            $rootScope.isFreeUser = isFreeUser;
            localStorageService.set(USERKEY, angular.toJson(CurrentUserService.profile));
        }

        CurrentUserService.logout = function (email) {
            localStorageService.remove(USERKEY);
            CurrentUserService.setProfile(null, null, null, null, false);
            $location.path("/login");
        }

        CurrentUserService.externalLogin = {
            token: null,
            name: null,
        };

        CurrentUserService.clearLocal = function () {
            localStorageService.remove(USERKEY);
        }

        return CurrentUserService;
    }

    CurrentUserService.$inject = ["$location", "$rootScope", "localStorageService"];
    app.factory("CurrentUserService", CurrentUserService);

}(angular.module("DrNajeebUser")));