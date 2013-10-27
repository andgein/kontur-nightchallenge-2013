function nav() {
	load("_nav.html");
}
function load(url) {
	$.ajax(url, {
		async: false,
		success: function (res) {
			document.write(res);
		}
	});
}

function MakeJsonController(url) {
	return function($scope, $http) {
		$http
			.get(url)
			.success(function(json) { for (var p in json) $scope[p] = json[p]; });
	};
}

var qs = (function (a) {
	if (a == "") return {};
	var b = {};
	for (var i = 0; i < a.length; ++i) {
		var p = a[i].split('=');
		if (p.length != 2) continue;
		b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
	}
	return b;
})(window.location.search.substr(1).split('&'));

