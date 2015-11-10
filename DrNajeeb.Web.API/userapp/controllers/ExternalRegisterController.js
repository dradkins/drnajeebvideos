(function (module) {

    var ExternalRegisterController = function ($scope, $location, OAuthService, CountryService, toastr, CurrentUserService) {

        $scope.registerModel = {
            email: null,
            token:null,
            provider:"Facebook",
            fullName: null,
            countryId: 418,
        }
        $scope.countries = [];

        $scope.register = function (form) {
            if (form.$valid) {
                OAuthService.registerExternal($scope.registerModel)
                            .then(onRegister, onRegisterError);
            }
            else {
                toastr.error("The data you provided is incorrect, please verify your provided data and register again.")
            }
        }

        var onRegister = function (data) {
            //CurrentUserService.setProfile(data.userName, data.access_token)
            toastr.success("Registered successfully,")
            window.location.href = "/home/checkout/" + data;
        }

        var onRegisterError = function (error) {
            if (error.status == 400) {
                toastr.error("Email already in use. Please try a different email address");
            }
            else {
                toastr.error("Unable to register user at this time");
            }
        }

        function init() {
            CountryService.getAll().then(onCountries, onError);
            $scope.registerModel.fullName = CurrentUserService.externalLogin.name;
            $scope.registerModel.token = CurrentUserService.externalLogin.token;
            $scope.registerModel.email = CurrentUserService.externalLogin.email;
        }

        var onError = function (error) {
            console.log(error);
        }

        var onCountries = function (response) {
            $scope.countries = null;
            $scope.countries = response;
        }

        init();
    }

    ExternalRegisterController.$inject = ["$scope", "$location", "OAuthService", "CountryService", "toastr", "CurrentUserService"];
    module.controller("ExternalRegisterController", ExternalRegisterController);

}(angular.module("DrNajeebUser")));