var server = {
	root: "/corewar/",
	post: function (url, data) {
		return $.ajax({
			cache: false,
			type: "POST",
			url: this.root + url,
			data: JSON.stringify(data),
			contentType: 'application/json; charset=utf-8'
		});
	},
	get: function (url, params) {
		return $.ajax({
			cache: false,
			type: "GET",
			url: this.root + url,
			data: params
		});
	}
}