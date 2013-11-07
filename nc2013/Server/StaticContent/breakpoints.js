var Breakpoints = Base.extend({
	constructor: function (options) {
		this.$container = options.$container;
		this.memory = options.memory;
		this.breakpointViews = [];
		this.onError = options.onError;
	},
	setBreakpoints: function (breakpoints) {
		breakpoints = breakpoints || [];
		var oldBreakpointsCount = this.breakpointViews.length;
		for (var i = 0; i < oldBreakpointsCount; ++i) {
			if (i < breakpoints.length) {
				this.breakpointViews[i].setCell(this.memory.getCell(breakpoints[i].address));
				this.breakpointViews[i].attachBreakpoint(this, breakpoints[i]);
			}
			else {
				this.breakpointViews[i].setCell(null);
				this.breakpointViews[i].detachBreakpoint();
			}
		}
		if (oldBreakpointsCount < breakpoints.length) {
			var builder = new CellView.Builder({
				$container: this.$container,
				useText: true,
				viewClass: BreakpointView,
				cellClass: "listingItem"
			});
			for (var i = oldBreakpointsCount; i < breakpoints.length; ++i)
				builder.addCell(this.memory.getCell(breakpoints[i].address));
			this.breakpointViews = this.breakpointViews.concat(builder.build());
			for (var i = oldBreakpointsCount; i < breakpoints.length; ++i)
				this.breakpointViews[i].attachBreakpoint(this, breakpoints[i]);
		}
	},
	removeBreakpoint: function (breakpoint) {
		var that = this;
		return server.get("debugger/breakpoints/remove", breakpoint)
			.pipe(function () {
				for (var i = 0; i < that.breakpointViews.length; ++i) {
					if (that.breakpointViews[i].breakpoint == breakpoint) {
						that.breakpointViews[i].setCell(null);
						that.breakpointViews[i].detachBreakpoint();
						if (i < that.breakpointViews.length - 1) {
							that.breakpointViews[i].remove$();
							that.breakpointViews.splice(i, 1);
						}
						break;
					}
				}
			})
			.fail(function (err) {
				that.onError && that.onError(err);
			});
	}
});

var BreakpointView = CellView.extend({
	constructor: function (options) {
		this.base(options);
		this.$text = this.$view.find("span:eq(0)");
		this.$edit = this.$view.find("span.item-control.edit");
		this.$remove = this.$view.find("span.item-control.remove");
		var that = this;
		this.$remove.on("mousedown", function (e) {
			if (e.which == 1) {
				that._removeBreakpoint();
				return false;
			}
		});
	},
	attachBreakpoint: function (breakpoints, breakpoint) {
		this.breakpoints = breakpoints;
		this.breakpoint = breakpoint;
	},
	detachBreakpoint: function () {
		this.breakpoints = null;
		this.breakpoint = null;
	},
	_removeBreakpoint: function () {
		this.breakpoints && this.breakpoints.removeBreakpoint(this.breakpoint);
	},
	setText: function (text) {
		this.$text.text(text);
	}
}, {
	itemTemplate: "<div><span class='%cellClass%'>%cellText%</span><span class='item-control remove'>del</span><span class='item-control edit'>edit</span></div>"
});