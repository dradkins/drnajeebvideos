﻿(function (app) {

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
                showDownloadOption: false,
                guid: "",
                isAccountExpire:false
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
                user.showDownloadOption = currentUser.showDownloadOption;
                user.guid = currentUser.guid
                user.isAccountExpire = currentUser.isAccountExpire
            }
            return user;
        }

        CurrentUserService.profile = initialize();

        CurrentUserService.setProfile = function (username, token, fullName, profilePic, isSocialMediaLogin, isFreeUser, showDownloadOption, guid, isAccountExpire) {
            CurrentUserService.profile.username = username;
            CurrentUserService.profile.token = token;
            CurrentUserService.profile.fullName = fullName;
            CurrentUserService.profile.profilePic = profilePic;
            CurrentUserService.profile.isSocialMediaLogin = isSocialMediaLogin;
            CurrentUserService.profile.isFreeUser = isFreeUser;
            CurrentUserService.profile.showDownloadOption = showDownloadOption;
            CurrentUserService.profile.guid = guid;
            CurrentUserService.profile.isAccountExpire = isAccountExpire;
            $rootScope.USER_NAME = username;
            $rootScope.FULL_NAME = fullName;
            $rootScope.facebookProfilePic = profilePic;
            $rootScope.isFreeUser = isFreeUser;
            $rootScope.showDownloadOption = showDownloadOption;
            localStorageService.set(USERKEY, angular.toJson(CurrentUserService.profile));
            console.log(CurrentUserService.profile);
        }

        CurrentUserService.logout = function (email) {
            localStorageService.remove(USERKEY);
            CurrentUserService.setProfile(null, null, null, null, false);
            $location.path("/login");
        }

        CurrentUserService.externalLogin = {
            token: null,
            name: null,
            subscriptionId: null,
            isFreeUser: null,
            selectedSubscription: null,
        };

        CurrentUserService.clearLocal = function () {
            localStorageService.remove(USERKEY);
        }

        return CurrentUserService;
    }

    CurrentUserService.$inject = ["$location", "$rootScope", "localStorageService"];
    app.factory("CurrentUserService", CurrentUserService);

}(angular.module("DrNajeebUser")));