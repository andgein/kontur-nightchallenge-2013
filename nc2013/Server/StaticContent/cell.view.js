var CellView = Base.extend({
	constructor: function (options) {
		this.useText = options.useText;
		this.$view = options.$view;
		this.$container = options.$container;
		var that = this;
		this.$view.on("mousedown", function () {
			if (that.cell)
				that.cell.scrollIntoView();
		});
		this.cell = options.cell;
		if (this.cell)
			this.cell.attachReadyView(this);
	},
	setCell: function (cell) {
		if (this.cell != cell) {
			if (this.cell)
				this.cell.detachView(this);
			this.cell = cell;
			if (this.cell) {
				this.cell.attachView(this);
				this.$view.removeClass("hidden");
			} else
				this.$view.addClass("hidden");
		}
	},
	scrollIntoView: function () { },
	remove$: function () {
		this.$view.remove();
	},
	addClass: function (viewClass) {
		this.$view.addClass(viewClass);
	},
	removeClass: function (viewClass) {
		this.$view.removeClass(viewClass);
	},
	setText: function (text) {
		if (this.useText)
			this.$view.text(text);
	}
}, {
	itemTemplate: "<div class='%cellClass%'>%cellText%</div>",
	Builder: Base.extend({
		constructor: function (options) {
			this.$container = options.$container;
			this.cellClass = options.cellClass;
			this.useText = options.useText;
			var viewClass = options.viewClass || CellView;
			var itemTemplate = options.itemTemplate || viewClass.itemTemplate || CellView.itemTemplate;
			var that = this;
			this.viewFactory = options.viewFactory || function (cellCreationOptions) { return new viewClass(cellCreationOptions); };
			this.itemFactory = options.itemFactory || function (cell) {
				var cellClass = that.cellClass + " " + cell.getViewClass();
				var cellText = that.useText ? cell.getViewText() : "";
				return itemTemplate
					.replace("%cellClass%", cellClass)
					.replace("%cellText%", cellText);
			};
			this.cells = [];
		},
		addCell: function (cell) {
			this.cells.push(cell);
		},
		addMemory: function (memory) {
			for (var address = 0; address < memory.getSize() ; ++address)
				this.addCell(memory.getCell(address));
		},
		build: function () {
			if (this.cells.length == 0)
				return [];
			var cellsHtml = "";
			for (var i = 0; i < this.cells.length; ++i) {
				var cell = this.cells[i];
				cellsHtml += this.itemFactory(cell);
			}
			var result = [];
			var that = this;
			$(cellsHtml).appendTo(this.$container).each(function (index, div) {
				result.push(that.viewFactory({
					cell: that.cells[index],
					$view: $(div),
					useText: that.useText,
					$container: that.$container
				}));
			});
			return result;
		}
	})
});