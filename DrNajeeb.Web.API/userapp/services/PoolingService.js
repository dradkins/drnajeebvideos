(function (module) {

    var PoolingService = function ($interval, $http, CurrentUserService) {

        PoolingService = {};

        var timeIntervalInSec = 70000;

        function callFnOnInterval(fn, timeInterval) {
            return $interval(fn, 1000 * timeInterval);
        };

        var onIntervalReached = function () {
            if (CurrentUserService.profile.isLoggedIn()) {
                $http.get("/api/user/checkValidity?id=" + CurrentUserService.profile.token)
                    .then(function (response) {
                        if (!response.data.result) {
                            alert("You have been logged out from your account. Someone else is using your account.")
                            //console.log(response);
                            CurrentUserService.logout();
                            location.reload(true);
                        }
                    }, function (error) {
                        console.log(error);
                    });
            }
        }

        PoolingService.StartPooling = function () {
            callFnOnInterval(onIntervalReached, timeIntervalInSec);
        }

        return PoolingService;
    }

    PoolingService.$inject = ["$interval", "$http", "CurrentUserService"]
    module.factory("PoolingService", PoolingService);

}(angular.module("DrNajeebUser")))