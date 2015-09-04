(function () {

    var app = angular.module("DrNajeebAdmin", ["ngRoute", "angular-loading-bar", "ui.bootstrap", "720kb.datepicker", "color.picker"]);

    app.config(function ($routeProvider) {
        $routeProvider
            .when("/subscription", {
                templateUrl: "scripts/app/views/subscription.html",
                controller: "SubscriptionController"
            })
            .when("/categories", {
                templateUrl: "scripts/app/views/categories.html",
                controller: "CategoriesController"
            })
            .when("/videos", {
                templateUrl: "scripts/app/views/videos.html",
                controller: "VideosController"
            })
            .when("/users", {
                templateUrl: "scripts/app/views/users.html",
                controller: "UsersController"
            })
        .otherwise({ redirectTo: "/subscription" })
    });

}());