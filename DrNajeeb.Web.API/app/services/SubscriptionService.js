'use strict';

(function (app) {

    var SubscriptionService = function ($http) {

        var SubscriptionService = {};

        SubscriptionService.getSubscriptions = function (pagingInfo) {
            return $http.get("/api/subscription/getall", { params: pagingInfo })
                        .then(function (response) {
                            return response.data;
                        });
        };

        SubscriptionService.addSubscription = function (subscription) {
            return $http.post("/api/subscription/addSubscription", subscription)
                        .then(function (response) {
                            return response.data;
                        });
        };

        SubscriptionService.editSubscription = function (subscription) {
            return $http.post("/api/subscription/updateSubscription", subscription)
                        .then(function (response) {
                            return response.data;
                        });
        };

        SubscriptionService.deleteSubscription = function (id) {
            return $http.delete("/api/subscription/deleteSubscription/" + id)
                        .then(function (response) {
                            return response.data;
                        });
        };

        SubscriptionService.showSubscription = function (SubscriptionId) {
            return $http.get("/api/subscription/getSingle", { params: SubscriptionId })
                        .then(function (response) {
                            return response.data;
                        });
        };

        return SubscriptionService;
    }

    SubscriptionService.$inject = ["$http"];
    app.factory("SubscriptionService", SubscriptionService);

}(angular.module("DrNajeebAdmin")));