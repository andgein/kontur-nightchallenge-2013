var BreakpointsCollection = Base.extend({
	constructor: function (options) {
		this.$container = options.$container;
		this.$editContainer = options.$editContainer;
		this.$add = options.$add;
		this.$clear = options.$clear;
		this.$editApply = options.$editApply;
		this.$editCancel = options.$editCancel;
		this.$editCell = options.$editCell;
		this.editControls$ = options.editControls$;
		this.memory = options.memory;
		this.breakpointViews = [];
		this.breakpointsMap = {};
		this.onError = options.onError;
		var that = this;
		this.$clear.on("mousedown", function (event) {
			if (event.which == 1) {
				that._clearBreakpoints();
				return false;
			}
		});
		this.$add.on("mousedown", function (event) {
			if (event.which == 1) {
				that._editBreakpoint(that.memory.getJustScrolledCell());
				return false;
			}
		});
		this.editCell = new CellView({ $view: this.$editCell, useText: true });
	},
	setBreakpoints: function (breakpoints) {
		breakpoints = breakpoints || [];
		this.breakpointsMap = {};
		var breakpointsMapArray = [];
		for (var i = 0; i < breakpoints.length; ++i) {
			var breakpointInfo = this.breakpointsMap[breakpoints[i].address];
			if (!breakpointInfo) {
				breakpointInfo = this.breakpointsMap[breakpoints[i].address] = { breakpoints: [], address: breakpoints[i].address };
				breakpointsMapArray.push(breakpointInfo);
			}
			breakpointInfo.breakpoints.push(breakpoints[i]);
		}
		var oldBreakpointsCount = this.breakpointViews.length;
		for (var i = 0; i < oldBreakpointsCount; ++i) {
			if (i < breakpointsMapArray.length) {
				this.breakpointViews[i].setCell(this.memory.getCell(breakpointsMapArray[i].address));
				this.breakpointViews[i].attachBreakpoints(this);
				breakpointsMapArray[i].breakpointView = this.breakpointViews[i];
			}
			else {
				this.breakpointViews[i].setCell(null);
				this.breakpointViews[i].detachBreakpoints();
			}
		}
		if (oldBreakpointsCount < breakpointsMapArray.length) {
			var builder = new CellView.Builder({
				$container: this.$container,
				useText: true,
				viewClass: BreakpointView,
				cellClass: "listingItem"
			});
			for (var i = oldBreakpointsCount; i < breakpointsMapArray.length; ++i)
				builder.addCell(this.memory.getCell(breakpointsMapArray[i].address));
			this.breakpointViews = this.breakpointViews.concat(builder.build());
			for (var i = oldBreakpointsCount; i < breakpointsMapArray.length; ++i) {
				this.breakpointViews[i].attachBreakpoints(this);
				breakpointsMapArray[i].breakpointView = this.breakpointViews[i];
			}
		}
	},
	_editBreakpoint: function (cell) {
		if (!cell) {
			this.onError && this.onError("At first please select instruction from the listing");
			return;
		}
		this.onError && this.onError(null);
		this.editCell.setCell(cell);

		var breakpoints = [];
		var address = cell.getAddress();
		var breakpointInfo = this.breakpointsMap[address];
		var breakpointView = null;
		if (breakpointInfo) {
			breakpoints = breakpointInfo.breakpoints;
			breakpointView = breakpointInfo.breakpointView;
		}

		for (var i = 0; i < this.editControls$.length; ++i) {
			this.editControls$[i].Execution.removeAttr("checked");
			this.editControls$[i].MemoryChange.removeAttr("checked");
		}
		for (var i = 0; i < breakpoints.length; ++i) {
			var checkbox = this.editControls$[breakpoints[i].program][breakpoints[i].breakpointType];
			checkbox.attr("checked", "checked");
		}
		this.$add.addClass("hidden");
		this.$clear.addClass("hidden");
		this.$container.addClass("hidden");
		this.$editContainer.removeClass("hidden");

		var that = this;
		function hide() {
			that.$add.removeClass("hidden");
			that.$clear.removeClass("hidden");
			that.$container.removeClass("hidden");
			that.$editContainer.addClass("hidden");
			that.$editCancel.unbind("click.editBreakpoint");
			that.$editApply.unbind("click.editBreakpoint");
		}

		function apply() {
			var queue = $.when();
			for (var i = 0; i < that.editControls$.length; ++i) {
				for (var breakpointType in that.editControls$[i]) {
					if (that.editControls$[i][breakpointType].is(":checked")) {
						(function (breakpoint) {
							queue = queue.pipe(function () {
								return server.get("debugger/breakpoints/add", breakpoint)
									.done(function () {
										for (var b = 0; b < breakpoints.length; ++b)
											if (breakpoints[b].breakpointType == breakpoint.breakpointType && breakpoints[b].program == breakpoint.program)
												return;
										breakpoints.push(breakpoint);
									});
							});
						})({ address: address, breakpointType: breakpointType, program: i });
					}
					else {
						(function (breakpoint) {
							queue = queue.pipe(function () {
								return server.get("debugger/breakpoints/remove", breakpoint)
									.done(function () {
										for (var b = 0; b < breakpoints.length; ++b)
											if (breakpoints[b].breakpointType == breakpoint.breakpointType && breakpoints[b].program == breakpoint.program) {
												breakpoints.splice(b, 1);
												return;
											}
									});
							});
						})({ address: address, breakpointType: breakpointType, program: i });
					}
				}
			}
			queue
				.pipe(function () {
					if (breakpoints.length == 0) {
						if (breakpointInfo) {
							for (var i = 0; i < that.breakpointViews.length; ++i) {
								if (that.breakpointViews[i] == breakpointView) {
									delete that.breakpointsMap[that.breakpointViews[i].getAddress()];
									that.breakpointViews[i].setCell(null);
									that.breakpointViews[i].detachBreakpoints();
									if (i < that.breakpointViews.length - 1) {
										that.breakpointViews[i].remove$();
										that.breakpointViews.splice(i, 1);
									}
									break;
								}
							}
							delete that.breakpointsMap[address];
						}
					} else {
						if (!breakpointInfo) {
							that.breakpointsMap[address] = breakpointInfo = { address: address, breakpoints: breakpoints };
							var builder = new CellView.Builder({
								$container: that.$container,
								useText: true,
								viewClass: BreakpointView,
								cellClass: "listingItem"
							});
							builder.addCell(cell);
							breakpointView = builder.build()[0];
							breakpointInfo.breakpointView = breakpointView;
							breakpointView.attachBreakpoints(that);
							that.breakpointViews.push(breakpointView);
						}
					}
				})
				.fail(function (err) {
					that.onError && that.onError(err);
				})
				.done(function () {
					that.onError && that.onError(null);
					hide();
				});
		}

		this.$editCancel.bind("click.editBreakpoint", hide);
		this.$editApply.bind("click.editBreakpoint", apply);
	},
	_clearBreakpoints: function () {
		var that = this;
		return server.get("debugger/breakpoints/clear")
			.pipe(function () {
				that.breakpointsMap = {};
				for (var i = 0; i < that.breakpointViews.length; ++i) {
					that.breakpointViews[i].setCell(null);
					that.breakpointViews[i].detachBreakpoints();
				}
			})
			.fail(function (err) {
				that.onError && that.onError(err);
			})
			.done(function () {
				that.onError && that.onError(null);
			});
	},
	_editBreakpointView: function (breakpointView) {
		this._editBreakpoint(breakpointView.getCell());
	},
	_removeBreakpointView: function (breakpointView) {
		var that = this;
		var address = breakpointView.getAddress();
		var breakpoints = [];
		if (this.breakpointsMap[address])
			breakpoints = this.breakpointsMap[address].breakpoints;
		var queue = $.when();
		for (var i = 0; i < breakpoints.length; i++) {
			(function (breakpoint) {
				queue = queue.pipe(function () {
					return server.get("debugger/breakpoints/remove", breakpoint);
				});
			})(breakpoints[i]);
		}
		return queue
			.pipe(function () {
				for (var i = 0; i < that.breakpointViews.length; ++i) {
					if (that.breakpointViews[i] == breakpointView) {
						delete that.breakpointsMap[that.breakpointViews[i].getAddress()];
						that.breakpointViews[i].setCell(null);
						that.breakpointViews[i].detachBreakpoints();
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
			})
			.done(function () {
				that.onError && that.onError(null);
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
				that._removeBreakpointView();
				return false;
			}
		});
		this.$edit.on("mousedown", function (e) {
			if (e.which == 1) {
				that._editBreakpointView();
				return false;
			}
		});
	},
	attachBreakpoints: function (breakpointsCollection) {
		this.breakpointsCollection = breakpointsCollection;
	},
	detachBreakpoints: function () {
		this.breakpointsCollection = null;
	},
	_removeBreakpointView: function () {
		this.breakpointsCollection && this.breakpointsCollection._removeBreakpointView(this);
	},
	_editBreakpointView: function () {
		this.breakpointsCollection && this.breakpointsCollection._editBreakpointView(this);
	},
	setText: function (text) {
		this.$text.text(text);
	}
}, {
	itemTemplate: "<div><span class='%cellClass%'>%cellText%</span><span class='item-control remove'>del</span><span class='item-control edit'>edit</span></div>"
});