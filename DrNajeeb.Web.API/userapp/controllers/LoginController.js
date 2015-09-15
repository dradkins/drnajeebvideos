(function (app) {

    var LoginController = function ($scope, $timeout, $location, $rootScope, Facebook, OAuthService, CurrentUserService, loginRedirect, toastr) {

        $scope.username = "";
        $scope.password = "";
        $scope.user = CurrentUserService.profile;
        $scope.facebookLogin;

        $scope.login = function (form) {
            if (form.$valid) {
                OAuthService.login($scope.username, $scope.password)
                    .then(onLogin, onError);
                $scope.password = "";
                form.$setPristine(true)
            };
        }

        $scope.register = function () {
            window.location.reload(true);
        }

        var onLogin = function (data) {
            toastr.success("Welcome " + data);
            loginRedirect.redirectPostLogin();
            $rootScope.updateUserImage();
        }

        var onError = function (response) {
            if (response.data.error == "not_active") {
                toastr.error("Payment not made yet please click <a style=\"color:blue\" href=\"/home/checkout/" + response.data.error_description + "\">here</a> to make payement", "", {
                    allowHtml: true
                });
            }
            else {
                toastr.error(response.data.error_description);
            }
        }

        var onExternalAccounts = function (data) {
            $scope.facebookLogin = data[0];
        }


        /****** Google+ Sigin *********/

        //$scope.$on('event:google-plus-signin-success', function (event, authResult) {
        //    var data = {
        //        token: authResult.access_token,
        //        provider: "Google"
        //    };
        //    OAuthService.getGoogleUserInfo(authResult.access_token);
        //    //OAuthService.loginExternal(data).then(onExternalLogin, onExternalLoginError);
        //    console.log(authResult);
        //});

        //$scope.$on('event:google-plus-signin-failure', function (event, authResult) {
        //    toastr.error("unable to login with ")
        //});

        /***** Eng Google+ Signin *******/


        //function init() {
        //    OAuthService.externalAccounts().then(onExternalAccounts, onError);
        //}

        //init();

        // Define user empty data :/
        $scope.user1 = {};

        // Defining user logged status
        $scope.logged = false;

        // And some fancy flags to display messages upon user status change
        $scope.byebye = false;
        $scope.salutation = false;

        /**
         * Watch for Facebook to be ready.
         * There's also the event that could be used
         */
        $scope.$watch(
          function () {
              return Facebook.isReady();
          },
          function (newVal) {
              if (newVal)
                  $scope.facebookReady = true;
          }
        );

        var userIsConnected = false;

        Facebook.getLoginStatus(function (response) {
            if (response.status == 'connected') {
                userIsConnected = true;
            }
        });

        /**
         * IntentLogin
         */
        $scope.IntentLogin = function () {
            if (!userIsConnected) {
                $scope.login1();
            }
            else {
                $scope.me();
            }
        };

        /**
         * Login
         */
        $scope.login1 = function () {
            Facebook.login(function (response) {
                if (response.status == 'connected') {
                    $scope.logged = true;
                    $scope.me();
                }

            }, {
                scope: 'email, user_likes',
                return_scopes: true
            });
        };

        /**
         * me 
         */
        $scope.me = function () {
            Facebook.api('/me', { fields: "id,name,picture,email" }, function (response) {
                /**
                 * Using $scope.$apply since this happens outside angular framework.
                 */
                $rootScope.facebookProfilePic = response.picture.data.url;
                $scope.$apply(function () {
                    $scope.user1 = response;
                    var data = {
                        token: $scope.facebookToken,
                        provider: "Facebook"
                    };
                    OAuthService.loginExternal(data).then(onExternalLogin, onExternalLoginError);
                });

            });
        };


        var onExternalLogin = function (data) {
            CurrentUserService.setProfile(data.userName, data.access_token, data.fullName, $scope.user1.picture.data.url, true);
            toastr.success("Welcome " + data.fullName);
            $location.path("/dashboard");
        }

        var onExternalLoginError = function (error) {
            if (error.status === 404) {
                CurrentUserService.externalLogin.token = $scope.facebookToken;
                CurrentUserService.externalLogin.name = $scope.user1.name;
                CurrentUserService.externalLogin.email = $scope.user1.email;
                $location.path("/register-external")
            }
            if (error.status === 403) {
                //toastr.error("Payment not made yet please click <a style=\"color:blue\" href=\"/home/checkout/" + response.data.error_description + "\">here</a> to make payement", "", {
                //    allowHtml: true
                //});
                console.log(error);
                toastr.error("Payment not made yet.")
            }
            else {
                toastr.error("Unable to login with facebook, Please use a local account.")
            }
        }

        /**
         * Logout
         */
        $scope.logout = function () {
            Facebook.logout(function () {
                $scope.$apply(function () {
                    $scope.user1 = {};
                    $scope.logged = false;
                });
            });
        }

        /**
         * Taking approach of Events :D
         */
        $scope.$on('Facebook:statusChange', function (ev, data) {
            console.log('Status: ', data);
            if (data.status == 'connected') {
                console.log("Connected");
                $scope.$apply(function () {
                    $scope.salutation = true;
                    $scope.byebye = false;
                    $scope.facebookToken = data.authResponse.accessToken;
                });
            } else {
                $scope.$apply(function () {
                    $scope.salutation = false;
                    $scope.byebye = true;

                    // Dismiss byebye message after two seconds
                    $timeout(function () {
                        $scope.byebye = false;
                    }, 2000)
                });
            }
        });
    }


    LoginController.$inject = ["$scope", "$timeout", "$location", "$rootScope", "Facebook", "OAuthService", "CurrentUserService", "loginRedirect", "toastr"];
    app.controller("LoginController", LoginController);

}(angular.module("DrNajeebUser")));