var Memory = Base.extend({
	constructor: function(options) {
		var cellsHtml = "";
		var listingHtml = "";
		var cellCount = options.cellCount;
		this.cells = [];
		for (var i = 0; i < cellCount; ++i) {
			var cell = new Cell(i);
			this.cells.push(cell);
			cellsHtml += "<div class='mapCell'></div>";
			listingHtml += "<div class='listingItem'>" + cell.getListingItemContent() + "</div>";
		}
		options.$map.html(cellsHtml);
		options.$listing.html(listingHtml);

		var that = this;
		options.$map.find("div").each(function(index) {
			that.cells[index].attachMapCell($(this));
		});
		options.$listing.find("div").each(function(index) {
			that.cells[index].attachListingItem($(this));
		});
	},
	applyDiffs: function(memoryDiffs) {
		for (var i = 0; i < memoryDiffs.length; ++i) {
			var cell = this.cells[memoryDiffs[i].address];
			cell.setCellState(memoryDiffs[i].cellState);
		}
	},
	setCellStates: function(cellStates) {
		for (var i = 0; i < this.cells.length; ++i)
			this.cells[i].setCellState(cellStates[i]);
	},
	getCell: function (address) {
		return this.cells[address];
	},
	reset: function () {
		for (var i = 0; i < this.cells.length; ++i)
			this.cells[i].setCellState(null);
	}
});

var Cell = Base.extend({
	constructor: function (address) {
		this.address = address;
		this.lastModifiedClass = "";
		this.lastContentClass = "";
		this.listingItemContent = this._calcListingItemContent();
	},
	getListingItemContent: function () {
		return this.listingItemContent;
	},
	attachMapCell: function ($mapCell) {
		this.$mapCell = $mapCell;
	},
	attachListingItem: function ($listingItem) {
		this.$listingItem = $listingItem;
	},
	setCellState: function (cellState) {
		this.cellState = cellState;
		this._refreshState();
	},
	_refreshState: function () {
		var modifiedClass;
		if (!this.cellState || this.cellState.lastModifiedByProgram == null)
			modifiedClass = "";
		else
			modifiedClass = "modified" + this.cellState.lastModifiedByProgram;
		if (modifiedClass != this.lastModifiedClass) {
			if (this.lastModifiedClass) {
				this.$mapCell.removeClass(this.lastModifiedClass);
				this.$listingItem.removeClass(this.lastModifiedClass);
			}
			this.lastModifiedClass = modifiedClass;
			if (this.lastModifiedClass) {
				this.$mapCell.addClass(this.lastModifiedClass);
				this.$listingItem.addClass(this.lastModifiedClass);
			}
		}
		var contentClass;
		if (!this.cellState)
			contentClass = "Data";
		else
			contentClass = this.cellState.cellType;
		if (contentClass != this.lastContentClass) {
			if (this.lastContentClass) {
				this.$mapCell.removeClass(this.lastContentClass);
				this.$listingItem.removeClass(this.lastContentClass);
			}
			this.lastContentClass = contentClass;
			if (this.lastContentClass) {
				this.$mapCell.addClass(this.lastContentClass);
				this.$listingItem.addClass(this.lastContentClass);
			}
		}
		var listingItemContent = this._calcListingItemContent();
		if (listingItemContent != this.listingItemContent) {
			this.listingItemContent = listingItemContent;
			this.$listingItem.text(this.listingItemContent);
		}
	},
	_calcListingItemContent: function () {
		if (!this.cellState)
			return "";
		return this.address + " " + this.cellState.instruction;
	}
});