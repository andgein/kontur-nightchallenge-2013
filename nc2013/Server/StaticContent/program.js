var ProgramState = Base.extend({
	constructor: function (options) {
		this.$processCount = options.$processCount;
		this.$last = options.$last;
		this.$next = options.$next;
		this.$win = options.$win;
		this.$source = options.$source;
		this.memory = options.memory;
	},
	applyDiff: function (programStateDiff) {
		switch (programStateDiff.changeType) {
			case "Executed":
				this.programState.lastPointer = this.programState.processPointers.shift();
				this.programState.processPointers.push(programStateDiff.nextPointer);
				break;
			case "Killed":
				this.programState.lastPointer = this.programState.processPointers.shift();
				break;
			case "Splitted":
				this.programState.processPointers.push(programStateDiff.nextPointer);
				break;
			default:
				throw "Invalid changeType: " + programStateDiff.changeType;
		}
		this._refreshState();
	},
	reset: function () {
		this.setProgramState(null);
		this.$win.removeClass("winner");
	},
	win: function () {
		this.$win.addClass("winner");
	},
	setProgramState: function (programState) {
		this.programState = programState;
		this._refreshState();
	},
	setProgramStartInfo: function (programStartInfo) {
		this.$source.val(programStartInfo && programStartInfo.program || "");
	},
	getProgramStartInfo: function () {
		var source = this.$source.val();
		return source && { program: source };
	},
	_refreshState: function () {
		this.$processCount.text(this.programState ? this.programState.processPointers.length : "");

		if (this.programState && this.programState.lastPointer) {
			var lastCell = this.memory.getCell(this.programState.lastPointer);
			this.$last.text(lastCell.getListingItemContent());
		} else
			this.$last.text("");

		if (this.programState && this.programState.processPointers.length > 0) {
			var nextCell = this.memory.getCell(this.programState.processPointers[0]);
			this.$next.text(nextCell.getListingItemContent());
		}
		else
			this.$next.text("");
	}
});