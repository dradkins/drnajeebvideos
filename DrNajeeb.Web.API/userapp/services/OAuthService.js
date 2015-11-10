(function (app) {

    var OAuthService = function ($http, FormEncodeService, CurrentUserService) {

        var OAuthService = {};

        OAuthService.login = function (username, password) {

            var config = {
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                }
            }

            var data = FormEncodeService({
                username: username,
                password: password,
                grant_type: "password",
            });

            return $http.post("/token", data, config)
                        .then(function (response) {
                            console.log(response);
                            var isFreeUser = (response.data.isFreeUser === "True")
                            var showDownloadOption = (response.data.showDownloadOption === "True")
                            CurrentUserService.setProfile(username, response.data.access_token, response.data.fullName, null, false, isFreeUser, showDownloadOption, response.data.guid);
                            //saveToken(response.data.access_token);
                            return response.data.fullName;
                        });

        }

        var saveToken = function (token) {
            $http.post("/api/user/SaveUserToken", { fullName: token }).then(function (response) { return response;})
        }

        OAuthService.changePassword = function (changePasswordModel) {
            return $http.post("/api/account/changePassword", changePasswordModel)
                   .then(function (response) {
                       return response.data;
                   });
        }

        OAuthService.register = function (registerModel) {
            return $http.post("/api/account/register", registerModel)
                   .then(function (response) {
                       return response.data;
                   });
        }

        OAuthService.registerExternal = function (registerModel) {
            return $http.post("/api/account/registerExternalToken", registerModel)
                        .then(function (response) {
                            return response.data;
                        });
        }

        OAuthService.loginExternal = function (signinModel) {
            return $http.post("/api/account/externalSignin", signinModel)
                        .then(function (response) {
                            return response.data;
                            saveToken(response.data.access_token);
                        });
        }

        OAuthService.getGoogleUserInfo = function (token) {
            var url = "https://www.googleapis.com/plus/v1/people/me";
            var config = {
                headers: {
                    "Authorization": "Bearer " + token,
                }
            }
            $http.get(url).then(function (response, config) {
                console.log(response);
            })
        }


        OAuthService.setFullName = function (fullNameModel) {
            return $http.post("/api/user/setFullName", fullNameModel)
                        .then(function (response) {
                            var uname = CurrentUserService.profile.username;
                            var atoken = CurrentUserService.profile.token;
                            var fullName = fullNameModel.fullName;
                            var freeUser = CurrentUserService.profile.isFreeUser;
                            var showDownloadOption = CurrentUserService.profile.showDownloadOption;
                            CurrentUserService.clearLocal();
                            CurrentUserService.setProfile(uname, atoken, fullName, null, false, freeUser, showDownloadOption);
                        });
        }

        OAuthService.resetPassword = function (email) {
            return $http.post("/api/account/forgotPassword", email);
        }

        return OAuthService;

    }

    OAuthService.$inject = ["$http", "FormEncodeService", "CurrentUserService"];
    app.factory("OAuthService", OAuthService);

}(angular.module("DrNajeebUser")));