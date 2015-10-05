(function () {

    var app = angular.module("DrNajeebAdmin",
        [
            "ngRoute",
            "angular-loading-bar",
            "ui.bootstrap",
            "720kb.datepicker",
            "color.picker",
            "checklist-model",
            "ngAnimate",
            "ui.sortable",
            "toastr",
            "jcs-autoValidate",
            "LocalStorageModule"
        ]);

    app.config(function ($routeProvider) {
        $routeProvider
            .when("/dashboard", {
                templateUrl: "/app/views/dashboard.html",
                controller: "DashboardController"
            })
            .when("/subscription", {
                templateUrl: "/app/views/subscription.html",
                controller: "SubscriptionController"
            })
            .when("/categories", {
                templateUrl: "/app/views/categories.html",
                controller: "CategoriesController"
            })
            .when("/videos", {
                templateUrl: "/app/views/videos.html",
                controller: "VideosController"
            })
            .when("/videos/:searchTerm", {
                templateUrl: "/app/views/videos.html",
                controller: "VideosController"
            })
            .when("/users", {
                templateUrl: "/app/views/users.html",
                controller: "UserController"
            })
            .when("/login", {
                templateUrl: "/app/views/login.html",
                controller: "LoginController"
            })
            .when("/support", {
                templateUrl: "/app/views/support.html",
                controller: "SupportController"
            })
            .when("/categories/:categoryId/:categoryName/videos", {
                templateUrl: "/app/views/CategoryVideos.html",
                controller: "CategoryVideoController"
            })
        .otherwise({ redirectTo: "/dashboard" })
    });

    app.run(function ($rootScope, $location, CurrentUserService) {
        $rootScope.location = $location;
        $rootScope.USER_NAME = CurrentUserService.profile.username;
        $rootScope.FullName = CurrentUserService.profile.fullName;
        $rootScope.LOG_OUT = function () {
            CurrentUserService.logout();
        }
        $rootScope.SEARCH_TERM = "";
        $rootScope.SEARC_VIDEO = function () {
            console.log($rootScope.SEARCH_TERM);
            if ($rootScope.SEARCH_TERM && $rootScope.SEARCH_TERM.length > 2) {
                $location.path("/videos/" + $rootScope.SEARCH_TERM);
            }
            else {
                alert("Input no valid, Min. length is 3 characters");
            }
        }
    });

}());