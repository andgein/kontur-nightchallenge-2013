var Memory = Base.extend({
	constructor: function (options) {
		var cellCount = options.cellCount;
		this.cells = [];
		var memoryData = {};
		for (var i = 0; i < cellCount; ++i)
			this.cells.push(new Cell({ address: i, memoryData: memoryData }));
	},
	applyDiffs: function (memoryDiffs) {
		for (var i = 0; i < memoryDiffs.length; ++i) {
			var cell = this.cells[memoryDiffs[i].address];
			cell.setCellState(memoryDiffs[i].cellState);
		}
	},
	setCellStates: function (cellStates) {
		for (var i = 0; i < this.cells.length; ++i)
			this.cells[i].setCellState(cellStates[i]);
	},
	getSize: function () {
		return this.cells.length;
	},
	getCell: function (address) {
		return this.cells[address];
	},
	reset: function () {
		for (var i = 0; i < this.cells.length; ++i)
			this.cells[i].reset();
	}
});