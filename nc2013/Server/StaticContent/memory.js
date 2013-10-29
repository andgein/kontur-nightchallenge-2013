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
		this.lastClass = "";
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
		var newClass;
		if (this._isEmptyState())
			newClass = "";
		else
			newClass = "modified" + this.cellState.lastModifiedByProgram;
		if (newClass != this.lastClass) {
			if (this.lastClass) {
				this.$mapCell.removeClass(this.lastClass);
				this.$listingItem.removeClass(this.lastClass);
			}
			this.lastClass = newClass;
			if (this.lastClass) {
				this.$mapCell.addClass(this.lastClass);
				this.$listingItem.addClass(this.lastClass);
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
			return this.address + " DAT 0 0";
		return this.address + " " + this.cellState.instruction;
	},
	_isEmptyState: function () {
		return !this.cellState || this.cellState.cellType == "Data";
	}
});