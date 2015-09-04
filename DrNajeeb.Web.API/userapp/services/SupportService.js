(function (module) {

    var SuportService = function ($http) {

        var SuportService = {};

        SuportService.sendMessage = function (support) {
            return $http.post("/api/support/supportMessage", support)
                    .then(function (response) {
                        return response.data;
                    });
        }


        SuportService.getMessages = function () {
            return $http.get("/api/support/loadUserMessages")
                        .then(function (response) {
                            return response.data;
                        });
        }

        return SuportService;
    }

    SuportService.$inject = ["$http"];
    module.factory("SuportService", SuportService);

}(angular.module("DrNajeebUser")));