(function (app) {

    var FormEncodeService = function () {

        return function (data) {

            var pairs = [];
            for (var name in data) {
                pairs.push(encodeURIComponent(name) + "=" + encodeURIComponent(data[name]));
            }
            return pairs.join("&").replace(/%20/g, "+");
        };

    };

    app.factory("FormEncodeService", FormEncodeService);

}(angular.module("DrNajeebUser")))