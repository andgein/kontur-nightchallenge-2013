var server = {
	basePath: function () {
		return $.cookie('basePath');
	},
	post: function (url, jsonString) {
		var that = this;
		return $.ajax({
			cache: false,
			type: "POST",
			url: this.basePath() + url,
			data: jsonString,
			dataType: 'json',
			contentType: 'application/json; charset=utf-8'
		}).pipe(null, function (err) {
			return that._getErrorMessage(err);
		});
	},
	get: function (url, params) {
		var that = this;
		return $.ajax({
			cache: false,
			type: "GET",
			url: this.basePath() + url,
			data: params
		}).pipe(null, function (err) {
			return that._getErrorMessage(err);
		});
	},
	_getErrorMessage: function (err) {
		var errorMessage = err && err.responseText || "";
		var userErrorMessage = errorMessage.match(/\[\[([\s\S]*)\]\]/);
		return userErrorMessage && userErrorMessage[1] || "Internal Server Error";
	}
}