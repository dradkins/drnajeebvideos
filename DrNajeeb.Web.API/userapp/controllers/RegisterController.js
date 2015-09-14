(function (module) {

    var RegisterController = function ($scope, $timeout, $location, $rootScope, Facebook, OAuthService, CurrentUserService, CountryService, SuportService, toastr) {

        $scope.registerModel = {
            email: null,
            password: null,
            confirmPassword: null,
            fullName: null,
            countryId: null,
            subscriptionId: 2
        }
        $scope.countries = [];
        $scope.country;
        $scope.selectedCountry = null;


        $scope.register = function (form) {
            if (form.$valid) {
                //var re = /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).*$/;
                //if (re.test($scope.registerModel.password) == false) {
                //    toastr.warning("Password must contain at least one upper case letter, one lower case letter and one number and minimum 6 characters long.");
                //    return false;
                //}
                if ($scope.registerModel.password !== $scope.registerModel.confirmPassword) {
                    toastr.warning("Password and confirm password not matched.");
                    return false;
                }
                $scope.registerModel.countryId = $scope.selectedCountry.id;
                OAuthService.register($scope.registerModel)
                            .then(onRegister, onRegisterError);
            }
            else {
                toastr.error("The data you provided is incorrect, please verify your provided data and register again.")
            }
        }

        var onRegister = function (data) {
            toastr.success("Registered successfully, Please login with your email and password.")
            window.location.href="/home/checkout/" + data;
            //$location.path("/login");
        }

        var onRegisterError = function (error) {
            console.log(error);
            if (error.status == 400) {
                toastr.error("Email already in use. Please try a different email address");
            }
            else {
                toastr.error("Unable to register user at this time");
            }
        }

        var onIpAddress = function (data) {
            console.log(data);
            CountryService.getCountryByIP(data).then(function (response) {
                console.log(response);
                $scope.country = response;
                angular.forEach($scope.countries, function (c) {
                    if (c.isO2Name === $scope.country.country) {
                        console.log(c);
                        $scope.selectedCountry = c;
                        $scope.registerModel.countryId == c.id;
                        return false;
                    }
                })
            })
        }

        function init() {
            CountryService.getAll().then(onCountries, onError);
        }

        var onError = function (error) {
            console.log(error);
        }

        var onCountries = function (response) {
            $scope.countries = null;
            $scope.countries = response;
            SuportService.getIpAddress().then(onIpAddress, onError);
        }



        /****** Facebook Region **********/

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

        /****** End Facebook Region ********/
        init();
    }

    RegisterController.$inject = ["$scope", "$timeout", "$location", "$rootScope",  "Facebook", "OAuthService", "CurrentUserService", "CountryService", "SuportService", "toastr"];
    module.controller("RegisterController", RegisterController);

}(angular.module("DrNajeebUser")));