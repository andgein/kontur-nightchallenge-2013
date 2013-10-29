var server = {
	root: "/corewar/",
	post: function (url, data) {
		var that = this;
		return $.ajax({
			cache: false,
			type: "POST",
			url: this.root + url,
			data: JSON.stringify(data),
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
			url: this.root + url,
			data: params
		}).pipe(null, function (err) {
			return that._getErrorMessage(err);
		});
	},
	_getErrorMessage: function(err) {
		return err && err.responseText || "Unknown error";
	}
}