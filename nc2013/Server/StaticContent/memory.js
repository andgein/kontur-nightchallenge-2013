var Memory = Base.extend({
	constructor: function(options) {
		var cellCount = options.cellCount;
		this.cells = [];
		for (var i = 0; i < cellCount; ++i) {
			var cell = new Cell(i);
			this.cells.push(cell);
		}
		
		var that = this;
		options.$map.find("div.mapCell").each(function(index) {
			that.cells[index].attachMapCell($(this));
		});
		options.$listing.find("div.listingItem").each(function(index) {
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
			this.cells[i].reset();
	}
});

var Cell = Base.extend({
	constructor: function(address) {
		this.address = address;
		this.lastModifiedClass = "";
		this.lastContentClass = "";
		this.listingItemContent = this._calcListingItemContent();
		this.activeInstructionPointers = {};
	},
	getListingItemContent: function() {
		return this.listingItemContent;
	},
	attachMapCell: function($mapCell) {
		this.$mapCell = $mapCell;
		var that = this;
		this.$mapCell.click(function () {
			that.scrollIntoView();
			return false;
		});
	},
	attachListingItem: function($listingItem) {
		this.$listingItem = $listingItem;
		var that = this;
		this.$listingItem.click(function () {
			that.scrollIntoView();
			return false;
		});
	},
	setCellState: function(cellState) {
		this.cellState = cellState;
		this._refreshState();
	},
	setNextCommand: function () {
		if (Cell._lastCommand)
			Cell._lastCommand.$listingItem.removeClass("nextCommand");
		this.$listingItem.addClass("nextCommand");
		Cell._lastCommand = this;
	},
	scrollIntoView: function () {
		if (Cell._lastScrolledInto)
			Cell._lastScrolledInto.$listingItem.removeClass("justScrolled");
		this.$listingItem.addClass("justScrolled");
		if (!this.$listing) {
			this.$listing = this.$listingItem.parent();
			this.listingHeight = this.$listing.height();
			this.listingItemHeight = this.$listingItem.height();
		}
		if (!this.$listingPivot)
			this.$listingPivot = $("div.listingItem:eq(5)", this.$listing);
		var top = this.$listingItem.position().top;
		var listingTop = this.$listing.position().top;
		if (top < listingTop || top > listingTop + this.listingHeight - this.listingItemHeight)
			this.$listing.scrollTop(top - this.$listingPivot.position().top);
		Cell._lastScrolledInto = this;
	},
	reset: function () {
		this.$listingItem.removeClass("justScrolled");
		this.$listingItem.removeClass("nextCommand");
		var instructionPointersToRemove = [];
		for (var i in this.activeInstructionPointers)
			instructionPointersToRemove.push(i);
		for (var i = 0; i < instructionPointersToRemove.length; ++i)
			this.removeInstructionPointer(instructionPointersToRemove[i]);
		this.setCellState(null);
	},
	removeInstructionPointer: function (programIndex) {
		delete this.activeInstructionPointers[programIndex];
		this.$mapCell.removeClass("ip" + programIndex);
		this.$listingItem.removeClass("ip" + programIndex);
	},
	setInstructionPointer: function (programIndex) {
		this.$mapCell.addClass("ip" + programIndex);
		this.$listingItem.addClass("ip" + programIndex);
		this.activeInstructionPointers[programIndex] = true;
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
}, {
	_lastScrolledInto: null,
	_lastCommand: null
});