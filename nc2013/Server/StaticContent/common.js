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

(function($) {
	$.fn.FormCache = function(options) {
		var settings = $.extend({
			
		}, options);

		function on_change(event) {
			var input = $(event.target);
			var key = input.parents('form:first').attr('name');
			var data = JSON.parse(localStorage[key]);

			if (input.attr('type') == 'radio' || input.attr('type') == 'checkbox') {
				data[input.attr('name')] = input.is(':checked');
			} else {
				data[input.attr('name')] = input.val();
			}

			localStorage[key] = JSON.stringify(data);
		}

		return this.each(function() {
			var element = $(this);

			if (typeof(Storage) !== "undefined") {
				var key = element.attr('name');

				var data = false;
				if (localStorage[key]) {
					data = JSON.parse(localStorage[key]);
				}

				if (!data) {
					localStorage[key] = JSON.stringify({});
					data = JSON.parse(localStorage[key]);
				}
				element.find('input, select').change(on_change);

				element.find('input, select').each(function() {
					var input = $(this);
					if (input.attr('type') != 'submit' && input.attr('type') != 'password') {
						var value = data[input.attr('name')];
						if (input.attr('type') == 'radio' || input.attr('type') == 'checkbox') {
							if (value) {
								input.attr('checked', input.is(':checked'));
							} else {
								input.removeAttr('checked');
							}
						} else {
							input.val(value);
						}
					}
				});


			} else {
				alert('local storage is not available');
			}
		});
	};
}(jQuery));

$.fn.asJsonString = function () {
	var o = {};
	var a = this.serializeArray();
	$.each(a, function () {
		if (o[this.name] !== undefined) {
			if (!o[this.name].push) {
				o[this.name] = [o[this.name]];
			}
			o[this.name].push(this.value || '');
		} else {
			o[this.name] = this.value || '';
		}
	});
	return JSON.stringify(o);
};