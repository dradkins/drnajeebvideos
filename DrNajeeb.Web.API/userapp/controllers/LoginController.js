(function (app) {

    var LoginController = function ($scope, $timeout, $location, Facebook, OAuthService, CurrentUserService, loginRedirect, toastr) {

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

        var onLogin = function (data) {
            toastr.success("Welcome To DrNajeebLectures");
            loginRedirect.redirectPostLogin();
        }

        var onError = function (response) {
            toastr.error(response.data.error_description);
        }

        var onExternalAccounts = function (data) {
            $scope.facebookLogin = data[0];
        }

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
                console.log("In Login");
                console.log(response);
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
            Facebook.api('/me', function (response) {
                /**
                 * Using $scope.$apply since this happens outside angular framework.
                 */
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
            CurrentUserService.setProfile(data.userName, data.access_token)
            toastr.success("Login successfully,")
            $location.path("/dashboard");
        }

        var onExternalLoginError = function (error) {
            console.log(error);
            if (error.status === 404) {
                CurrentUserService.externalLogin.token = $scope.facebookToken;
                CurrentUserService.externalLogin.name = $scope.user1.name;
                $location.path("/register-external")
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


    LoginController.$inject = ["$scope", "$timeout", "$location", "Facebook", "OAuthService", "CurrentUserService", "loginRedirect", "toastr"];
    app.controller("LoginController", LoginController);

}(angular.module("DrNajeebUser")));