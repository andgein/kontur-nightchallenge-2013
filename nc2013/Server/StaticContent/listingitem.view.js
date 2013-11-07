var ListingItemView = CellView.extend({
	scrollIntoView: function () {
		var top = this.$view.position().top;
		var listingTop = this.$container.position().top;
		var listingHeight = this.$container.height();
		var listingItemHeight = this.$view.height();
		if (top < listingTop || top > listingTop + listingHeight - listingItemHeight)
			this.$container.scrollTop(top - $("div:eq(5)", this.$container).position().top);
	}
});