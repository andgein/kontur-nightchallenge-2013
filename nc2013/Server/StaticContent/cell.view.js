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
			if (this.cell)
				this.cell.attachView(this);
		}
	},
	scrollIntoView: function () { },
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
	Builder: Base.extend({
		constructor: function (options) {
			this.$container = options.$container;
			this.cellClass = options.cellClass;
			this.useText = options.useText;
			var viewClass = options.viewClass || CellView;
			this.viewFactory = options.viewFactory || function (cellCreationOptions) { return new viewClass(cellCreationOptions); };
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
				var cellClass = this.cellClass + " " + cell.getViewClass();
				cellsHtml += "<div class='" + cellClass + "'>" + (this.useText ? cell.getViewText() : "") + "</div>";
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